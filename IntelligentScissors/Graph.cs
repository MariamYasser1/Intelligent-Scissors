using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentScissors
{
    internal class Graph
    {
        RGBPixel[,] ImageMatrix;
        //Bitmap ImageBitMap;
        int Height, Width, CurAnchor, StartAnchor, LastAnchor;
        int[] dx, dy, ParentNode;
        double[] ShortestPath;
        double INF = 1e15, EPS = 1e-9;
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
            ShortestPath = new double[Height * Width];
            ParentNode = new int[Height * Width];
            ConstructGraph();
        }

        public int GetLastAnchor()
        {
            return LastAnchor;
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

        private int GetIndex(int x, int y)
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

            for (int i = 0; i + 1 < Height; i++)
            {
                for (int j = 0; j + 1 < Width; j++)
                {
                    Vector2D Energy = ImageOperations.CalculatePixelEnergies(i, j, ImageMatrix);
                    int Idx = GetIndex(i, j);
                    int RightIdx = GetIndex(i, j + 1), BottomIdx = GetIndex(i + 1, j);
                    AdjacencyList[Idx].Add(new KeyValuePair<int, double>(RightIdx, Energy.X));
                    AdjacencyList[RightIdx].Add(new KeyValuePair<int, double>(Idx, Energy.X));
                    AdjacencyList[Idx].Add(new KeyValuePair<int, double>(BottomIdx, Energy.Y));
                    AdjacencyList[BottomIdx].Add(new KeyValuePair<int, double>(Idx, Energy.Y));
                }
            }
        }

        private void Reset()
        {
            for (int i = 0; i < Height * Width; i++)
            {
                ShortestPath[i] = INF;
                ParentNode[i] = i;
            }
        }

        public class KvpKeyComparer<TKey, TValue> : IComparer<KeyValuePair<TKey, TValue>>
        where TKey : IComparable
        {
            public int Compare(KeyValuePair<TKey, TValue> x,
                               KeyValuePair<TKey, TValue> y)
            {
                if (x.Key == null)
                {
                    if (y.Key == null)
                        return 0;
                    return -1;
                }

                if (y.Key == null)
                    return 1;

                return x.Key.CompareTo(y.Key);
            }
        }

        private void RunDijkstra()
        {
            Reset();
            SortedSet<KeyValuePair<double, int>> EdgesSet = new SortedSet<KeyValuePair<double, int>>(new KvpKeyComparer<double, int>());

            EdgesSet.Add(new KeyValuePair<double, int>(0, CurAnchor));
            ShortestPath[CurAnchor] = 0;
            ParentNode[CurAnchor] = -1;

            while (EdgesSet.Count > 0)
            {
                double CurDist = EdgesSet.ElementAt<KeyValuePair<double, int>>(0).Key;
                int NodeIdx = EdgesSet.ElementAt<KeyValuePair<double, int>>(0).Value;
                EdgesSet.Remove(new KeyValuePair<double, int>(CurDist, NodeIdx));

                foreach (var Child in AdjacencyList[NodeIdx])
                {
                    if (ShortestPath[Child.Key] - CurDist - Child.Value > EPS)
                    {
                        ParentNode[Child.Key] = NodeIdx;
                        ShortestPath[Child.Key] = CurDist + Child.Value;
                        EdgesSet.Add(new KeyValuePair<double, int>(Child.Value + CurDist, Child.Key));
                    }
                }
            }
        }
    }
}
