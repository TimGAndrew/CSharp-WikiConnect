using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiParser {
    public class WikiNode {
        //public bool Visited { get; set; } = false;
        public WikiNode Parent { get; set; }
        public string Name { get; set; }
        public List<WikiNode> Children { get; set; }

        public WikiNode(string name, WikiNode parent = null) {
            Name = name;
            Parent = parent;
            Children = new List<WikiNode>();
        }
    }
}
