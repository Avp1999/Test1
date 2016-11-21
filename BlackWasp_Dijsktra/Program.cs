using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackWasp_Dijsktra
{
    class Program
    {
        internal class Node
        {

            IList<NodeConnection> _connections;

            internal string Name { get; private set; }

            internal double DistanceFromStart { get; set; }

            internal IEnumerable<NodeConnection> Connections
            {
                get { return _connections; }
            }

            internal Node(string name)
            {
                Name = name;
                _connections = new List<NodeConnection>();
            }

            internal void AddConnection(Node targetNode, double distance, bool twoWay)
            {
                if (targetNode == null) throw new ArgumentNullException("targetNode");
                if (targetNode == this)
                    throw new ArgumentException("Node may not connect to itself.");
                if (distance <= 0) throw new ArgumentException("Distance must be positive.");

                _connections.Add(new NodeConnection(targetNode, distance));
                if (twoWay) targetNode.AddConnection(this, distance, false);
            }
        }

        internal class NodeConnection
        {
            internal Node Target { get; private set; }
            internal double Distance { get; private set; }

            internal NodeConnection(Node target, double distance)
            {
                Target = target;
                Distance = distance;
            }
        }

        public class Graph
        {
            internal IDictionary<string, Node> Nodes { get; private set; }

            public Graph()
            {
                Nodes = new Dictionary<string, Node>();
            }

            public void AddNode(string name)
            {
                var node = new Node(name);
                Nodes.Add(name, node);
            }

            public void AddConnection(string fromNode, string toNode, int distance, bool twoWay)
            {
                Nodes[fromNode].AddConnection(Nodes[toNode], distance, twoWay);
            }
        }

        public class DistanceCalculator
        {
            private void InitialiseGraph(Graph graph, string startingNode)
            {
                foreach (Node node in graph.Nodes.Values)
                    node.DistanceFromStart = double.PositiveInfinity;
                graph.Nodes[startingNode].DistanceFromStart = 0;
            }

            private void ProcessGraph(Graph graph, string startingNode)
            {
                bool finished = false;
                var queue = graph.Nodes.Values.ToList();
                while (!finished)
                {
                    Node nextNode = queue.OrderBy(n => n.DistanceFromStart).FirstOrDefault(
                        n => !double.IsPositiveInfinity(n.DistanceFromStart));
                    if (nextNode != null)
                    {
                        ProcessNode(nextNode, queue);
                        queue.Remove(nextNode);
                    }
                    else
                    {
                        finished = true;
                    }
                }
            }

            private void ProcessNode(Node node, List<Node> queue)
            {
                var connections = node.Connections.Where(c => queue.Contains(c.Target));
                foreach (var connection in connections)
                {
                    double distance = node.DistanceFromStart + connection.Distance;
                    if (distance < connection.Target.DistanceFromStart)
                        connection.Target.DistanceFromStart = distance;
                }
            }

            private IDictionary<string, double> ExtractDistances(Graph graph)
            {
                return graph.Nodes.ToDictionary(n => n.Key, n => n.Value.DistanceFromStart);
            }


            public IDictionary<string, double> CalculateDistances(Graph graph, string startingNode)
            {
                if (!graph.Nodes.Any(n => n.Key == startingNode))
                {
                    throw new ArgumentException("Starting node must be in graph.");
                }
                InitialiseGraph(graph, startingNode);
                ProcessGraph(graph, startingNode);
                return ExtractDistances(graph);


            }
        }



        static void Main(string[] args)
        {
            Graph graph = new Graph();

            //Nodes
            graph.AddNode("A");
            graph.AddNode("B");
            graph.AddNode("C");
            graph.AddNode("D");
            graph.AddNode("E");
            graph.AddNode("F");
            graph.AddNode("G");
            graph.AddNode("H");
            graph.AddNode("I");
            graph.AddNode("J");
            graph.AddNode("Z");

            //Connections
            graph.AddConnection("A", "B", 14, true);
            graph.AddConnection("A", "C", 10, true);
            graph.AddConnection("A", "D", 14, true);
            graph.AddConnection("A", "E", 21, true);
            graph.AddConnection("B", "C", 9, true);
            graph.AddConnection("B", "E", 10, true);
            graph.AddConnection("B", "F", 14, true);
            graph.AddConnection("C", "D", 9, false);
            graph.AddConnection("D", "G", 10, false);
            graph.AddConnection("E", "H", 11, true);
            graph.AddConnection("F", "C", 10, false);
            graph.AddConnection("F", "H", 10, true);
            graph.AddConnection("F", "I", 9, true);
            graph.AddConnection("G", "F", 8, false);
            graph.AddConnection("G", "I", 9, true);
            graph.AddConnection("H", "J", 9, true);
            graph.AddConnection("I", "J", 10, true);

            var calculator = new DistanceCalculator();
            var distances = calculator.CalculateDistances(graph, "A");  // Start from "G"

            foreach (var d in distances)
            {
                Console.WriteLine("{0}, {1}", d.Key, d.Value);
            }

        }
    }
}
