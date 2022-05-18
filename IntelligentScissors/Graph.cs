using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace IntelligentScissors
{
    internal class Graph
    {
        RGBPixel[,] ImageMatrix;
        //Bitmap ImageBitMap;
        int Height, Width, CurAnchor, StartAnchor, LastAnchor;
        int[] dx, dy, ParentNode;
        double[] ShortestPath;
        double INF = -1, EPS = 1e-9;
        List<KeyValuePair<int, double>>[] AdjacencyList;

        public Graph(RGBPixel[,] ImageMatrix/*, string ImagePath*/)
        {
            this.ImageMatrix = ImageMatrix;
            //ImageBitMap = new Bitmap(ImagePath);
            Height = ImageMatrix.GetLength(0);
            Width = ImageMatrix.GetLength(1);
            StartAnchor = CurAnchor = LastAnchor = -1;
            dx = new int[] {1, 0, -1, 0};
            dy = new int[] {0, 1, 0, -1};
            ShortestPath = new double[Height * Width + 10];
            ParentNode = new int[Height * Width];
            ConstructGraph();
        }

        public int GetLastAnchor()
        {
            return LastAnchor;
        }
        public int GetStartAnchor()
        {
            return StartAnchor;
        }
        public List<KeyValuePair<int, int>> GetShortestPath(int NewAnchor)
        {
            int CurNode = NewAnchor;
            List<KeyValuePair<int, int>> Path = new List<KeyValuePair<int, int>>();
            while (CurNode != -1)
            {

                Path.Add(GetCoordinates(CurNode));
                CurNode = ParentNode[CurNode];
            }
            return Path;
        }

        public void SetCurAnchor(int x, int y)
        {
            LastAnchor = CurAnchor;
            CurAnchor = GetIndex(x, y);
            RunDijkstra();
            if (StartAnchor == -1)
                StartAnchor = GetIndex(x, y);
        }

        private bool Valid(int x, int y)
        {
            return (x >= 0 && x < Height && y >= 0 && y < Width);
        }

        public int GetIndex(int x, int y)
        {
            return (x * Width) + y;
        }

        private KeyValuePair<int, int> GetCoordinates(int idx)
        {
            return new KeyValuePair<int, int>(idx / Width, idx % Width);
        }

        private void ConstructGraph()
        {
            AdjacencyList = new List<KeyValuePair<int, double>>[Height * Width];
            for (int i = 0; i < Height * Width; i++)
                AdjacencyList[i] = new List<KeyValuePair<int, double>>();

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    Vector2D Energy = ImageOperations.CalculatePixelEnergies(i, j, ImageMatrix);
                    int Idx = GetIndex(i, j);
                    int RightIdx = GetIndex(i, j + 1), BottomIdx = GetIndex(i + 1, j);
                    if (j + 1 < Width)
                    {
                        double w = Energy.X;
                        if (w == 0)
                            w = 1e16;
                        else
                            w = 1 / w;
                        AdjacencyList[Idx].Add(new KeyValuePair<int, double>(RightIdx, w));
                        AdjacencyList[RightIdx].Add(new KeyValuePair<int, double>(Idx, w));
                    }
                    if (i + 1 < Height)
                    {
                        double w = Energy.Y;
                        if (w == 0)
                            w = 1e16;
                        else
                            w = 1 / w;
                        AdjacencyList[Idx].Add(new KeyValuePair<int, double>(BottomIdx, w));
                        AdjacencyList[BottomIdx].Add(new KeyValuePair<int, double>(Idx, w));
                    }
                }
            }
        }

        private void Reset()
        {
            for (int i = 0; i < Height * Width; i++)
            {
                ShortestPath[i] = INF;
                ParentNode[i] = -1;
            }
        }

       

        private void RunDijkstra()
        {
            Reset();
            PriorityQueue pq = new PriorityQueue(false);
            pq.Enqueue(new KeyValuePair<double,int>(0, CurAnchor));
            ShortestPath[CurAnchor] = 0;
            ParentNode[CurAnchor] = -1;
         
            while (pq.Count > 0)
            {
                var Node = pq.Dequeue();
                double CurDist = Node.Key;
                int NodeIdx = Node.Value;
                foreach (var Child in AdjacencyList[NodeIdx])
                {
                    if (ShortestPath[Child.Key] > CurDist + Child.Value || ShortestPath[Child.Key] == -1)
                    {
                        ParentNode[Child.Key] = NodeIdx;
                        ShortestPath[Child.Key] = CurDist + Child.Value;
                        pq.Enqueue(new KeyValuePair<double, int>(CurDist + Child.Value, Child.Key));
                    }
                }
            }
            
        }
    }
}
