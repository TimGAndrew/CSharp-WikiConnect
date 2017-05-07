using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WikiParser
{
    public class FetchData
    {
        private static readonly string baseURL = "https://en.wikipedia.org/w/api.php?action=query&prop=revisions&rvprop=content&format=json&titles=";
        private static readonly string linksUrl = @"https://en.wikipedia.org/w/api.php?action=query&format=json&prop=links&titles={0}&plnamespace=0&pllimit=500";
        private static readonly string linksContUrl = @"https://en.wikipedia.org/w/api.php?action=query&format=json&prop=links&titles={0}&plnamespace=0&pllimit=500&plcontinue={1}";
        private static readonly string linkshereUrl = @"https://en.wikipedia.org/w/api.php?action=query&format=json&prop=linkshere&titles={0}&lhprop=title&lhnamespace=0&lhshow=!redirect&lhlimit=500";
        private static readonly string linkshereContUrl = @"https://en.wikipedia.org/w/api.php?action=query&format=json&prop=linkshere&titles={0}&lhprop=title&lhnamespace=0&lhshow=!redirect&lhlimit=500&lhcontinue={1}";

        private static List<string> fetchedLinksCache;

        public static List<string> GetAssociatedPagesLimit500(string pageTitle) {
            if (fetchedLinksCache != null) {
                return fetchedLinksCache;
            }

            fetchedLinksCache = new List<string>();

            var hc = new HttpClient();

            string urlTitle = WebUtility.UrlEncode(pageTitle);

            var json = new WebClient().DownloadString(linksUrl + urlTitle);

            WikipediaResult objects = JsonConvert.DeserializeObject<WikipediaResult>(json);

            return objects.query.pages.First().Value.links.Select(x => x.title).ToList();
        }

        public static List<string> GetAllAssociatedPages(string pageTitle) {
            List<string> fetched = new List<string>();

            var hc = new HttpClient();

            string urlTitle = WebUtility.UrlEncode(pageTitle);

            var json = new WebClient().DownloadString(baseURL + urlTitle);
            //string json = SampleString.getSample();

            string pattern = @"\[\[([^\]\#\|\:]+)[(\]\])|\|]";

            MatchCollection matches = Regex.Matches(json, pattern);

            //Console.WriteLine(matches.Count);

            foreach (Match match in matches) {
                //Console.WriteLine(match.Groups[1].Value);
                fetched.Add(match.Groups[1].Value);
            }

            fetchedLinksCache = fetched.Distinct().ToList();

            return fetchedLinksCache;
        }

        public static List<WikiNode> GetForwardWikiNodes(WikiNode parent) {

            List<WikiNode> edges = new List<WikiNode>();
            string urlTitle = WebUtility.UrlEncode(parent.Name);

            using (var client = new WebClient()) {
                var json = client.DownloadString(string.Format(linksUrl, urlTitle));
                WikipediaResult objects = JsonConvert.DeserializeObject<WikipediaResult>(json);

                if (objects.query.pages.First().Value.links != null) {
                    foreach (Link link in objects.query.pages.First().Value.links) {
                        if (!link.title.Equals(parent.Name)) {
                            edges.Add(new WikiNode(link.title, parent));
                        }
                    }

                    string plcontinue = objects.Continue?.Plcontinue;

                    // Are there more links to retrieve?
                    while (!string.IsNullOrEmpty(plcontinue)) {

                        json = client.DownloadString(string.Format(linksContUrl,
                            urlTitle, objects.Continue.Plcontinue));
                        objects = JsonConvert.DeserializeObject<WikipediaResult>(json);
                        plcontinue = objects.Continue?.Plcontinue;

                        foreach (Link link in objects.query.pages.First().Value.links) {
                            if (!link.title.Equals(parent.Name)) {
                                edges.Add(new WikiNode(link.title, parent));
                            }
                        }
                    }
                }
            }
            return edges;
        }

        public static List<WikiNode> GetBackwardWikiNodes(WikiNode parent) {

            List<WikiNode> edges = new List<WikiNode>();
            string urlTitle = WebUtility.UrlEncode(parent.Name);

            using (var client = new WebClient()) {
                var json = client.DownloadString(string.Format(linkshereUrl, urlTitle));
                WikipediaResult objects = JsonConvert.DeserializeObject<WikipediaResult>(json);

                if (objects.query.pages.First().Value.linkshere != null) {
                    foreach (Link link in objects.query.pages.First().Value.linkshere) {
                        if (!link.title.Equals(parent.Name)) {
                            edges.Add(new WikiNode(link.title, parent));
                        }
                    }

                    string lhcontinue = objects.Continue?.lhcontinue;

                    // Are there more links to retrieve?
                    while (!string.IsNullOrEmpty(lhcontinue)) {

                        json = client.DownloadString(string.Format(linkshereContUrl,
                            urlTitle, objects.Continue.lhcontinue));
                        objects = JsonConvert.DeserializeObject<WikipediaResult>(json);
                        lhcontinue = objects.Continue?.lhcontinue;

                        foreach (Link link in objects.query.pages.First().Value.linkshere) {
                            if (!link.title.Equals(parent.Name)) {
                                edges.Add(new WikiNode(link.title, parent));
                            }
                        }
                    }
                }
            }
            return edges;
        }
    }
}
