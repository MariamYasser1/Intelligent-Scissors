using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace IntelligentScissors
{
    public class Graph
    {
        RGBPixel[,] ImageMatrix;
        int Height, Width, CurAnchor, StartAnchor;
        int LastAnchor, id, WindowSize, Frequency, PathMaxLength;
        int[] ParentNode , Visited;
        double[] ShortestPath;
        double INF = -1;
        bool[] Fixed;
        List<KeyValuePair<int, double>>[] AdjacencyList;
        List<int> Anchors;

        public Graph(RGBPixel[,] ImageMatrix)
        {
            id = 0; 
            WindowSize = 130;
            Anchors = new List<int>();
            this.ImageMatrix = ImageMatrix;
            Width = ImageMatrix.GetLength(0);
            Height = ImageMatrix.GetLength(1);
            Visited = new int[Height * Width];
            StartAnchor = CurAnchor = LastAnchor = -1;
            PathMaxLength = 70;
            Frequency = 20;
            ShortestPath = new double[Height * Width + 10];
            ParentNode = new int[Height * Width + 10];
            Fixed = new bool[Height * Width + 10];
            ConstructGraph();
        }

        public int GetPathMaxLength()
        {
            return PathMaxLength;
        }

        public int GetFrequency()
        {
            return Frequency;
        }

        public void SetFrequency(int value)
        {
            Frequency = value;
        }

        public void SetPathMaxLength(int value)
        {
            PathMaxLength = value;
        }

        public List<KeyValuePair<int, double>>[] GetAdjacencyList()
        {
            return AdjacencyList;
        }

        public int GetNodeSize()
        {
            return Height * Width;
        }

        public void SetWindowSize(int size)
        {
            WindowSize = size;
        }

        public int GetLastAnchor()
        {
            return LastAnchor;
        }

        public int GetStartAnchor()
        {
            return StartAnchor;
        }

        public List<int> GetAnchors()
        {
            return Anchors;
        }

        public List<KeyValuePair<int, int>> GetShortestPath(int NewAnchor, bool is_live)
        {
            int CurNode = NewAnchor;
            List<KeyValuePair<int, int>> Path = new List<KeyValuePair<int, int>>();
            while (CurNode != -1)
            {
                if (!Fixed[CurNode] || is_live)
                    Path.Add(GetCoordinates(CurNode));
                CurNode = ParentNode[CurNode];
            }
            return Path;
        }

        public void SetCurAnchor(int x, int y, bool Is_Anchor)
        {
            if(Is_Anchor)
                Anchors.Add(GetIndex(x, y));
            LastAnchor = CurAnchor;
            CurAnchor = GetIndex(x, y);
            RunDijkstra();
            if (StartAnchor == -1)
                StartAnchor = GetIndex(x, y);
        }

        public int GetIndex(int x, int y)
        {
            return (x * Width) + y;
        }

        public void Fix(int x, int y)
        {
            Fixed[GetIndex(x, y)] = true;  
        }

        public KeyValuePair<int, int> GetCoordinates(int idx)
        {
            return new KeyValuePair<int, int>(idx / Width, idx % Width);
        }

        // O(Height * Width)
        private void ConstructGraph()
        {
            AdjacencyList = new List<KeyValuePair<int, double>>[Height * Width];
            
            // O(Height * Width)
            for (int i = 0; i < Height * Width; i++)
            {
                Fixed[i] = false;
                AdjacencyList[i] = new List<KeyValuePair<int, double>>();
            }
            TestingHandling.SetStartTime(DateTime.Now);

            // O(Height * Width)
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    Vector2D Energy = ImageOperations.CalculatePixelEnergies(i, j, ImageMatrix);
                    int Idx = GetIndex(i, j); // O(1)
                    int RightIdx = GetIndex(i, j + 1), BottomIdx = GetIndex(i + 1, j);
                    
                    // Check if right node exists 
                    if (j + 1 < Width)
                    {
                        // Calculating weight = 1 / G
                        double w = Energy.X;
                        if (w == 0)
                            w = 1e16;
                        else
                            w = 1 / w;

                        // Connect undirected edge between node and right node
                        AdjacencyList[Idx].Add(new KeyValuePair<int, double>(RightIdx, w));
                        AdjacencyList[RightIdx].Add(new KeyValuePair<int, double>(Idx, w));
                    }

                    // Check if bottom node exists
                    if (i + 1 < Height)
                    {
                        // Calculating weight = 1 / G
                        double w = Energy.Y;
                        if (w == 0)
                            w = 1e16;
                        else
                            w = 1 / w;

                        // Connect undirected edge between node and bottom node
                        AdjacencyList[Idx].Add(new KeyValuePair<int, double>(BottomIdx, w));
                        AdjacencyList[BottomIdx].Add(new KeyValuePair<int, double>(Idx, w));
                    }
                }
            }

            // Write the constructed graph into text file
            //if (TestingHandling.GetImageFilePath().IndexOf("Complete") != -1)
            //    TestingHandling.PrintConstructedGraphCompleteTest(this);
            //else
            //    TestingHandling.PrintConstructedGraphSampleTest(this);
            Reset();
        }

        private void Reset()
        {
            for (int i = 0; i < Height * Width; i++)
            {
                ShortestPath[i] = INF;
                ParentNode[i] = -1;
            }
        }

        bool NotValid( int node)
        {
            var A = GetCoordinates(node);
            var B = GetCoordinates(CurAnchor);
            return Math.Max(Math.Abs(A.Key - B.Key), Math.Abs(A.Value - B.Value)) > WindowSize;
        }

        // O(WindowSize^2 * log(WindowSize^2))
        private void RunDijkstra()
        {
            id++; // Counter to reset the values of ShortestPath and ParentNode
            PriorityQueue pq = new PriorityQueue(false);
            pq.Enqueue(new KeyValuePair<double,int>(0, CurAnchor)); // O(log(WindowSize^2))

            ShortestPath[CurAnchor] = 0;
            ParentNode[CurAnchor] = -1;
            Visited[CurAnchor] = id;
            
            while (pq.Count > 0)
            {
                var Node = pq.Dequeue(); // O(log(WindowSize^2))
                double CurDist = Node.Key;
                int NodeIdx = Node.Value;

                // Check if the node is out of the WindowSize
                if (NotValid(NodeIdx))
                    continue;

                foreach (var Child in AdjacencyList[NodeIdx]) // O(log(WindowSize^2))
                {
                    // Check if it is the first time to visit the node or
                    // the current path is shorter than the previous path
                    if (Visited[Child.Key] != id || ShortestPath[Child.Key] > CurDist + Child.Value)
                    {
                        Visited[Child.Key] = id;
                        ParentNode[Child.Key] = NodeIdx;
                        ShortestPath[Child.Key] = CurDist + Child.Value;
                        // O(log(WindowSize^2))
                        pq.Enqueue(new KeyValuePair<double, int>(CurDist + Child.Value, Child.Key));
                    }
                }
            }
        }
    }
}
