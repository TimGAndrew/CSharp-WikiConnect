using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikiParser;

namespace WikiCrawler {
    class Program {
        static void Main(string[] args) {

            string origin, destination;

            Console.WriteLine("Welcome to WikiCrawler demo:\n");
            Console.Write("Enter origin article: ");
            origin = Console.ReadLine();
            Console.Write("Enter destination article: ");
            destination = Console.ReadLine();
            WikiNode start = new WikiNode(origin);
            WikiNode end = new WikiNode(destination);

            Crawler crawler = new Crawler();

            Console.WriteLine("Starting Search...\n");

            var watch = System.Diagnostics.Stopwatch.StartNew();
            CrawlResult result = crawler.Search(start, end);
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds / 1000;

            if (result != null) {
                Console.WriteLine("Result found!");
                Console.WriteLine("Origin: " + result.Origin);
                Console.WriteLine("Destination: " + result.Destination);
                Console.WriteLine("Parent: " + result.Parent);
                Console.WriteLine("Distance: " + result.Distance);
                Console.WriteLine("Url: " + result.PageUrl);
            }
            else {
                Console.WriteLine("No results found");
            }

            Console.WriteLine("\nOperation Completed in: " + elapsedMs + "seconds");
            Console.WriteLine("Nodes Processed: " + crawler.NodesProcessed);
            Console.WriteLine("Links Retrieved: " + crawler.LinksRetrieved);

            Console.ReadKey();
        }
    }
}
