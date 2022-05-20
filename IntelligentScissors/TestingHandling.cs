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
                Output.WriteLine("Graph construction took: " + ExecutionTime);
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
