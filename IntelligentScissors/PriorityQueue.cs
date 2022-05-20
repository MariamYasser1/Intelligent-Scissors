using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentScissors
{
    public class PriorityQueue 
    {
        private List<KeyValuePair<double, int>> list;
        public int Count { get { return list.Count; } }
        public readonly bool IsDescending;

        public PriorityQueue()
        {
            list = new List<KeyValuePair<double,int>>();
        }

        public PriorityQueue(bool isdesc)
            : this()
        {
            IsDescending = isdesc;
        }

        public PriorityQueue(int capacity)
            : this(capacity, false)
        { }

        public PriorityQueue(IEnumerable<KeyValuePair<double,int>> collection)
            : this(collection, false)
        { }

        public PriorityQueue(int capacity, bool isdesc)
        {
            list = new List<KeyValuePair<double, int>>(capacity);
            IsDescending = isdesc;
        }

        public PriorityQueue(IEnumerable<KeyValuePair<double,int>> collection, bool isdesc)
            : this()
        {
            IsDescending = isdesc;
            foreach (var item in collection)
                Enqueue(item);
        }

        public void Enqueue(KeyValuePair<double,int> x)
        {
            list.Add(x);
            int i = Count - 1;

            while (i > 0)
            {
                int p = (i - 1) / 2;
                if ((IsDescending ? -1 : 1) * Compare(list[p], x) <= 0) break;

                list[i] = list[p];
                i = p;
            }

            if (Count > 0) list[i] = x;
        }

        public KeyValuePair<double,int> Dequeue()
        {
            KeyValuePair<double,int> target = Peek();
            KeyValuePair<double,int> root = list[Count - 1];
            list.RemoveAt(Count - 1);

            int i = 0;
            while (i * 2 + 1 < Count)
            {
                int a = i * 2 + 1;
                int b = i * 2 + 2;
                int c = b < Count && (IsDescending ? -1 : 1) * Compare(list[b], list[a]) < 0 ? b : a;

                if ((IsDescending ? -1 : 1) * Compare(list[c], root) >= 0) break;
                list[i] = list[c];
                i = c;
            }

            if (Count > 0) list[i] = root;
            return target;
        }

        public static int Compare( KeyValuePair<double,int> A, KeyValuePair<double, int> B)
        {
            if (A.Key < B.Key) return -1;
            else if (A.Key > B.Key) return 1;
            else if (A.Value < B.Value) return -1;
            else if(A.Value > B.Value) return 1;
            return 0;
        }

        public KeyValuePair<double,int> Peek()
        {
            if (Count == 0) throw new InvalidOperationException("Queue is empty.");
            return list[0];
        }

        public void Clear()
        {
            list.Clear();
        }
        
    }
}
