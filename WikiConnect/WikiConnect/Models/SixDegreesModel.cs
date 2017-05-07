using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiConnect.Models
{
    public class SixDegreesModel
    {
        public int submit_token { get; set; }
        public string start_article { get; set; }
        public string end_article { get; set; }
        public int degrees { get; set; }
        public uint memory_usage { get; set; }
        public double seconds_to_generate { get; set; }
        public List<string> steps { get; set; }

        public SixDegreesModel ()
        {
            steps = new List<string>();
        }
    }
}
