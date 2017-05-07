using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using WikiParser;

namespace WikiCrawler {

    public class Crawler {
        // How deep does the rabit hole goes?
        private static readonly int maxDistance = 6;
        private int fDistance = 1;
        private int bDistance = 1;
        private BlockingCollection<WikiNode> forwardQueue = new BlockingCollection<WikiNode>();
        private BlockingCollection<WikiNode> backwardQueue = new BlockingCollection<WikiNode>();
        private bool stop = false;

        public uint NodesProcessed { get; set; }
        public uint LinksRetrieved { get; set; }

        public CrawlResult Search(WikiNode start, WikiNode end) {
            Thread f = new Thread(() => { SearchForward(start, end); });
            Thread b = new Thread(() => { SearchBackward(end, start); });
            f.Start();
            b.Start();

            while(true) {
                foreach (WikiNode fNode in forwardQueue) {
                    foreach (WikiNode bNode in backwardQueue) {
                        if (fNode.Name.ToLower().Equals(bNode.Name.ToLower())) {
                            CrawlResult result = new CrawlResult(
                                fDistance + bDistance,
                                start.Name,
                                end.Name,
                                fNode.Name,
                                "https://en.wikipedia.org/wiki/" + end.Name.Replace(" ", "_"));

                            stop = true;
                            return result;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Starts search for path to end node
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public void SearchForward (WikiNode start, WikiNode end) {

            Queue<WikiNode> queue = new Queue<WikiNode>();
            start.Children = FetchData.GetForwardWikiNodes(start);
            queue.Enqueue(start);

            while(queue.Count > 0 && fDistance < maxDistance && !stop) {
                //Console.WriteLine("Distance: " + fDistance);
                
                Queue<WikiNode> newQueue = new Queue<WikiNode>();

                // Loop through children first to see if a link to the end node is there
                foreach(WikiNode node in queue) {

                    if (stop) {
                        return;
                    }

                    foreach (WikiNode child in node.Children) {

                        if (stop) {
                            return;
                        }

                        NodesProcessed++;
                        child.Children = FetchData.GetForwardWikiNodes(child);
                        LinksRetrieved += (uint)child.Children.Count;
                        newQueue.Enqueue(child);
                        forwardQueue.Add(child);
                    }            
                }

                queue = newQueue;
                fDistance++;
            }
        }

        public void SearchBackward(WikiNode end, WikiNode start) {

            Queue<WikiNode> queue = new Queue<WikiNode>();
            end.Children = FetchData.GetBackwardWikiNodes(end);
            queue.Enqueue(end);

            while (queue.Count > 0 && bDistance < maxDistance && !stop) {
                //Console.WriteLine("Distance: " + bDistance);

                Queue<WikiNode> newQueue = new Queue<WikiNode>();

                // Loop through children first to see if a link to the end node is there
                foreach (WikiNode node in queue) {

                    if (stop) {
                        return;
                    }

                    foreach (WikiNode child in node.Children) {

                        if (stop) {
                            return;
                        }

                        NodesProcessed++;
                        child.Children = FetchData.GetBackwardWikiNodes(child);
                        LinksRetrieved += (uint)child.Children.Count;
                        newQueue.Enqueue(child);
                        backwardQueue.Add(child);
                    }
                }

                queue = newQueue;
                bDistance++;
            }
        }
    }
}
