using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentScissors
{
    internal class Graph
    {
        RGBPixel[,] ImageMatrix;
        int Height, Width, CurAnchor;
        int[] dx, dy;
        List<KeyValuePair<int, double>>[] AdjacencyList;
        double[] ShortestPath;
        double INF = 1e15, EPS = 1e-9;

        public Graph(RGBPixel[,] ImageMatrix)
        {
            this.ImageMatrix = ImageMatrix;
            Height = ImageMatrix.GetLength(0);
            Width = ImageMatrix.GetLength(1);
            dx = new int[] {1, 0, -1, 0};
            dy = new int[] {0, 1, 0, -1};
            ShortestPath = new double[Height * Width];
            ConstructGraph();
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
                ShortestPath[i] = INF;
        }

        private void RunDijkstra()
        {
            Reset();
            SortedSet<KeyValuePair<double, int>> EdgesSet = new SortedSet<KeyValuePair<double, int>>();

            EdgesSet.Add(new KeyValuePair<double, int>(0, CurAnchor));
            ShortestPath[CurAnchor] = 0;

            while (EdgesSet.Count > 0)
            {
                double CurDist = EdgesSet.ElementAt<KeyValuePair<double, int>>(0).Key;
                int NodeIdx = EdgesSet.ElementAt<KeyValuePair<double, int>>(0).Value;
                EdgesSet.Remove(new KeyValuePair<double, int>(CurDist, NodeIdx));

                KeyValuePair<int, int> NodePosition = GetCoordinates(NodeIdx);
                foreach (var Child in AdjacencyList[NodeIdx])
                {
                    if (ShortestPath[Child.Key] - CurDist - Child.Value > EPS)
                    {
                        ShortestPath[Child.Key] = CurDist + Child.Value;
                        EdgesSet.Add(new KeyValuePair<double, int>(Child.Value + CurDist, Child.Key));
                    }
                }
            }
        }
    }
}
