using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiCrawler {
    public class CrawlResult {
        public int Distance { get; }
        public string Origin { get; }
        public string Destination { get; }
        public string Parent { get; }
        public string PageUrl { get; }

        public CrawlResult(int hops, string origin, string dest, string parent , string url) {
            Distance = hops;
            Origin = origin;
            Destination = dest;
            Parent = parent;
            PageUrl = url;
        }
    }
}
