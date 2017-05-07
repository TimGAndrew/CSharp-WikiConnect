using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiParser
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> pageList = new List<string>();

            //pageList = FetchData.GetAllAssociatedPages("France");
            //pageList.Sort();

            foreach (string title in pageList)
            {
                Console.WriteLine(title);
            }

            Console.ReadKey();
        }
    }
}
