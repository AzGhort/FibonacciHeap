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
        public BidirectionalCyclicLinkedList<T, E> Roots = new BidirectionalCyclicLinkedList<T, E>();
        public int NodesCount { get; private set; } = 0;
        public Node<T, E> Minimum { get; private set; }
        public int LastOperationSteps { get; private set; } = 0;
        public bool Naive { get; set; } = false;
        private void Consolidation()
        {
            ConsolidationArray<T, E> array = new ConsolidationArray<T, E>(3 * (int)Math.Log(NodesCount, 2), Roots);
            var curN = Roots.Handle;
            int count = Roots.NodesCount;
            for (int i = 0; i < count; i++)
            {
                var oldH = Roots.Handle;
                var nextCur = curN.Right;
                array.Consolidate(curN);
                curN = nextCur;
            }
            LastOperationSteps += array.Steps;
            Minimum = array.NewMinimum;
        }
        private void CheckParents(Node<T, E> parent)
        {
            var curN = parent;
            while (curN.LostSon)
            {
                var oldPar = curN.Parent;
                MoveToRoots(curN);
                curN = oldPar;
            }
            if (curN.Parent != null) curN.LostSon = true;
        }
        private void MoveToRoots(Node<T, E> old)
        {
            // remove from children
            old.Parent.Children.SafeDeleteNode(old);
            old.Parent = null;
            old.LostSon = false;
            LastOperationSteps += 1;
            // add to roots
            Roots.Insert(old);

            if (Minimum.Key.CompareTo(old.Key) >= 0) { Minimum = old; }
        }

        #region Required heap operations
        public void Insert(Node<T, E> node)
        {
            LastOperationSteps = 1;
            Roots.Insert(node);
            NodesCount++;
            if (NodesCount == 1) { Minimum = node; }
            else if (node.Key.CompareTo(Minimum.Key) < 0)
            {
                Minimum = node;
            }

        }
        public void DeleteMinimum()
        {
            var childrenList = Minimum.Children;
            Minimum.ReleaseChildren();
            Roots.SafeDeleteNode(Minimum);
            Roots.Merge(childrenList);
            LastOperationSteps = childrenList.NodesCount;
            if (Roots.NodesCount > 1) Consolidation();
            NodesCount--;
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
            if (par == null) { return; }

            MoveToRoots(old);
            if (!Naive) CheckParents(par);
        }       
        #endregion 
    }
    class ConsolidationArray<T, E> where T : IEquatable<T>
                                   where E : IComparable<E>
    {
        Node<T, E>[] nodes;
        private BidirectionalCyclicLinkedList<T, E> roots;
        public int Steps { get; private set; } = 0;
        public int TimesSinceLastCollision { get; private set; } = 0;
        public Node<T, E> NewMinimum { get; private set; }
        public ConsolidationArray(int size, BidirectionalCyclicLinkedList<T, E> list)
        {
            nodes = new Node<T, E>[size];
            roots = list;
        }
        public void Consolidate(Node<T, E> node)
        {
            var orderNode = nodes[node.Order];
            CheckMinimum(node);
            if (orderNode == null)
            {
                nodes[node.Order] = node;
                TimesSinceLastCollision++;
            }
            else
            {
                TimesSinceLastCollision = 0;
                Steps++;
                nodes[node.Order] = null;
                if (orderNode.Key.CompareTo(node.Key) < 0)
                {
                    roots.SafeDeleteNode(node);
                    orderNode.InsertIntoChildren(node);
                    Consolidate(orderNode);
                }
                else
                {
                    roots.SafeDeleteNode(orderNode);
                    node.InsertIntoChildren(orderNode);
                    Consolidate(node);
                }
            }
        }
        private void CheckMinimum(Node<T, E> node)
        {
            if (NewMinimum == null) { NewMinimum = node; }
            else if (NewMinimum.Key.CompareTo(node.Key) > 0) { NewMinimum = node; }
        }
    }
}
