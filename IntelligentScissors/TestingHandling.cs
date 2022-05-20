using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentScissors
{
    public static class TestingHandling
    {
        static string ImageFilePath;
        static DateTime starttime;
        public static void SetImageFilePath(string imageFilePath)
        {
            ImageFilePath = "";
            int idx = imageFilePath.Length-1;
            while (imageFilePath[idx] != '\\')
                idx--;
            for ( int i = 0;  i <= idx; i++)
            {
                ImageFilePath += imageFilePath[i]; 
            }
        }

        public static string GetImageFilePath()
        {
            return ImageFilePath;
        }
        
        public static void PrintShortestPath( Graph ImageGraph)
        {
            using (StreamWriter Output = new StreamWriter(ImageFilePath + "PathOutput.txt"))
            {
                var Anchors = ImageGraph.GetAnchors();
                var pair = ImageGraph.GetCoordinates(Anchors[0]);
                ImageGraph.SetCurAnchor(pair.Key, pair.Value,false);
                TestingHandling.SetStartTime(DateTime.Now);
                for (int i = 1; i < Anchors.Count; i++)
                {
                    Output.Write("The shortest path from node " + Anchors[i - 1] + " at (" + pair.Key + "," + pair.Value + ")");
                    pair = ImageGraph.GetCoordinates(Anchors[i]);
                    Output.Write("to node " + Anchors[i] + " at (" + pair.Key + "," + pair.Value + ")\n");
                    List<KeyValuePair<int,int>> Path = ImageGraph.GetShortestPath(Anchors[i],true);
                    for( int j = Path.Count-1; j >= 0; j--)
                    {
                        Output.Write("NodeIndex = " + ImageGraph.GetIndex(Path[j].Key,Path[j].Value));
                        Output.Write("{X = " + Path[j].Key + ", Y = " + Path[j].Value + "}\n");
                    }
                    ImageGraph.SetCurAnchor(pair.Key, pair.Value,false);
                }
                string ExecutionTime = (DateTime.Now - TestingHandling.starttime).TotalMilliseconds.ToString();
                Output.WriteLine("Path Construction took: " + ExecutionTime + " milliseconds.");
            }
        }

        public static void PrintConstructedGraphCompleteTest( Graph ImageGraph)
        {
            int NodeCount = ImageGraph.getNodeSize();
            var AdjacencyList = ImageGraph.GetAdjacencyList();
            string ExecutionTime = (DateTime.Now - starttime).TotalSeconds.ToString();
            using (StreamWriter Output = new StreamWriter(ImageFilePath + "TestOutput.txt"))
            {
                Output.WriteLine("Constructed Graph: (Format: node_index|edges:(from, to, weight)(from, to, weight)...)");
                for (int node = 0; node < NodeCount; node++)
                {
                    Output.Write(node + "|edges:");
                    foreach (var Neighbor in AdjacencyList[node])
                        Output.Write('(' + node + "," + Neighbor.Key + "," + Neighbor.Value + ')');
                Output.WriteLine();
                }
                Output.WriteLine("Graph construction took: " + ExecutionTime + " seconds.");
            }
        }

        public static void SetStartTime(DateTime Time)
        {
            starttime = Time;
        }

        public static void PrintConstructedGraphSampleTest( Graph ImageGraph)
        {
            int NodeCount = ImageGraph.getNodeSize();
            var AdjacencyList = ImageGraph.GetAdjacencyList();
            using (StreamWriter Output = new StreamWriter(ImageFilePath + "TestOutput.txt"))
            {
                Output.WriteLine("The constructed graph");
                Output.WriteLine();
                for ( int node = 0; node < NodeCount; node++ )
                {
                    Output.WriteLine(" The index node" + node);
                    Output.WriteLine("Edges");
                    foreach (var Neighbor in AdjacencyList[node])
                    {
                        Output.WriteLine("edge from " + node + " To " + Neighbor.Key + " With Weights " + Neighbor.Value);
                    }
                    for (int i = 0; i < 3; i++)
                        Output.WriteLine();
                }
            }
        }
    }
}
