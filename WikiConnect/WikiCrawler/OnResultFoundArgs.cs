using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiCrawler {
    public class OnResultFoundArgs : EventArgs {

        public CrawlResult Result { get; set; }

        public OnResultFoundArgs(CrawlResult result) {
            Result = result;
        }
    }
}
