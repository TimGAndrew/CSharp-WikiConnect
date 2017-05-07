using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiParser
{

    public class WikipediaResult {
        public Query query { get; set; }
        public ContinueInfo Continue { get; set; }
    }

    public class ContinueInfo {
        public string lhcontinue { get; set; }
        public string Plcontinue { get; set; }
        public string Continue { get; set; }
    }

    public class Query
    {
        public Dictionary<string, Page> pages { get; set; }
    }

    public class Page
    {
        public int pageid { get; set; }
        public int ns { get; set; }
        public string title { get; set; }
        public List<Link> links { get; set; }
        public List<Link> linkshere { get; set; }

        public Page() {
            links = new List<Link>();
        }
    }

    public class Link
    {
        public int ns { get; set; }
        public string title { get; set; }
    }

}
