using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibonacciHeap
{
    class FibonacciHeap<T, E> where T : IEquatable<T>
                              where E : IComparable<E>
    {
        public LinkedList<T, E> Roots = new LinkedList<T, E>();
        public int NodesCount { get; private set; } = 0;
        public Node<T, E> Minimum { get; private set; }
        public int LastOperationSteps { get; private set; } = 0;
        public bool Naive { get; set; } = false;

        public void CheckMinimum(Node<T, E> node)
        {
            if (Minimum == null) { Minimum = node; }
            else if (node.Key.CompareTo(Minimum.Key) < 0)
            {
                Minimum = node;
            }
        }

        private void Consolidation()
        {
            ConsolidationArray<T, E> array = new ConsolidationArray<T, E>(100);
            var node = Roots.Head;
            int bound = Roots.NodesCount;
            for (int i = 0; i < bound; i++)
            {
                var next = node.Right;
                Roots.SafeDeleteNode(node);
                array.Consolidate(node);
                // prepare for next iter
                node = next;
            }
            Roots = array.Heapify();
            LastOperationSteps += array.Steps;
            Minimum = array.NewMinimum;
        }

        private void CheckParents(Node<T, E> parent)
        {
            var curN = parent;
            while (curN.LostSon && !curN.IsRoot())
            {
                var oldPar = curN.Parent;
                MoveToRoots(curN);
                curN = oldPar;
            }
            if (!curN.IsRoot()) curN.LostSon = true;
        }

        private void MoveToRoots(Node<T, E> old)
        {
            // remove from children
            old.Parent.Children.SafeDeleteNode(old);
            old.SetRoot();
            LastOperationSteps += 1;
            // add to roots
            Roots.Insert(old);
            CheckMinimum(old);
        }

        #region Required heap operations
        public void Insert(Node<T, E> node)
        {
            LastOperationSteps = 1;
            Roots.Insert(node);
            NodesCount++;
            CheckMinimum(node);
        }

        public void DeleteMinimum()
        {
            if (Roots.Head == null) { return; }

            var childrenList = Minimum.Children;
            Minimum.ReleaseChildren();
            Roots.SafeDeleteNode(Minimum);
            NodesCount--;
            Roots.Merge(childrenList);

            LastOperationSteps = childrenList.NodesCount;
            if (Roots.NodesCount > 1) Consolidation();
        }

        public void DecreaseKey(T id, E newkey, Node<T, E> old)
        {
            LastOperationSteps = 0;
            // no node with that id
            if (old == null) { return; }
            // new key is not less than old key
            if (newkey.CompareTo(old.Key) >= 0) { return; }

            old.Key = newkey;
            Node<T, E> par = old.Parent;

            // it's already root
            if (old.IsRoot()) { return; }
            MoveToRoots(old);
            if (!Naive) CheckParents(par);
        }
        #endregion
    }
    class ConsolidationArray<T, E> where T : IEquatable<T>
                                   where E : IComparable<E>
    {
        Node<T, E>[] nodes;
        public int Steps { get; private set; } = 0;
        public Node<T, E> NewMinimum { get; private set; }

        public ConsolidationArray(int size)
        {
            nodes = new Node<T, E>[size];
        }

        public void Consolidate(Node<T, E> n)
        {
            var node = n;
            while (nodes[node.Order] != null)
            {
                int oldOrd = node.Order;
                node = MergeHeaps(node, nodes[node.Order]);
                nodes[oldOrd] = null;
                Steps++;
            }

            nodes[node.Order] = node;
        }

        private Node<T, E> MergeHeaps(Node<T,E> first, Node<T, E> second)
        {
            if (first.Key.CompareTo(second.Key) < 0)
            {
                first.InsertIntoChildren(second);
                return first;
            }
            else
            {
                second.InsertIntoChildren(first);
                return second;
            }
        }

        public LinkedList<T, E> Heapify()
        {
            LinkedList<T, E> newHeap = new LinkedList<T, E>();
            foreach (var node in nodes)
            {
                if (node == null) continue;
                newHeap.Insert(node);
                CheckMinimum(node);
            }
            return newHeap;
        }

        private void CheckMinimum(Node<T, E> node)
        {
            if (NewMinimum == null) { NewMinimum = node; }
            else if (NewMinimum.Key.CompareTo(node.Key) > 0) { NewMinimum = node; }
        }
    }
}
