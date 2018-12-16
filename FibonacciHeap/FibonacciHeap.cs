using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibonacciHeap
{
    /// <summary>
    /// Fibonacci Heap class.
    /// </summary>
    /// <typeparam name="T">Identifier of nodes - must be equatable.</typeparam>
    /// <typeparam name="E">Priority (key) of nodes - must be comparable.</typeparam>
    class FibonacciHeap<T, E> where T : IEquatable<T>
                              where E : IComparable<E>
    {
        public LinkedList<T, E> Roots = new LinkedList<T, E>();
        public int NodesCount { get; private set; } = 0;
        public Node<T, E> Minimum { get; private set; }
        public int LastOperationSteps { get; private set; } = 0;
        public bool Naive { get; set; } = false;

        #region Auxiliary methods
        /// <summary>
        /// Checks if the given node is new minimum.
        /// </summary>
        /// <param name="node">Potential new minimum.</param>
        public void CheckMinimum(Node<T, E> node)
        {
            if (Minimum == null) { Minimum = node; }
            else if (node.Key.CompareTo(Minimum.Key) < 0)
            {
                Minimum = node;
            }
        }

        /// <summary>
        /// Consolidation method - merge trees in roots until no two trees have the same order.
        /// </summary>
        private void Consolidation()
        {
            ConsolidationArray<T, E> array = new ConsolidationArray<T, E>(100);            
            foreach (var node in Roots)
            {
                Roots.SafeDeleteNode(node);
                array.Consolidate(node);
            }
            Roots = array.Heapify();
            LastOperationSteps += array.Steps;
            Minimum = array.NewMinimum;
        }

        /// <summary>
        /// Called in DecreaseKey standard variant - checks marked parents up to the root. 
        /// </summary>
        /// <param name="parent">First parent to check.</param>
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

        /// <summary>
        /// Moves the given node to roots.
        /// </summary>
        /// <param name="old">Node to move.</param>
        private void MoveToRoots(Node<T, E> old)
        {
            // remove from children
            old.Parent.Children.SafeDeleteNode(old);
            old.SetRoot();
            LastOperationSteps ++;
            // add to roots
            Roots.Insert(old);
            CheckMinimum(old);
        }
        #endregion

        #region Required heap operations
        /// <summary>
        /// Insert node to the heap.
        /// </summary>
        /// <param name="node">Node to insert.</param>
        public void Insert(Node<T, E> node)
        {
            Roots.Insert(node);
            NodesCount++;
            CheckMinimum(node);
        }

        /// <summary>
        /// Deletes and returns minimum of heap.
        /// </summary>
        /// <returns>Deleted minimum (or null if empty).</returns>
        public Node<T,E> DeleteMinimum()
        {            
            if (Roots.Head == null) { return null; }
            LastOperationSteps = 0;

            var oldMin = Minimum;
            var childrenList = Minimum.Children;
            Minimum.ReleaseChildren();
            Roots.SafeDeleteNode(Minimum);
            NodesCount--;
            Roots.Merge(childrenList);

            LastOperationSteps = childrenList.NodesCount;
            Consolidation();
            return oldMin;
        }

        /// <summary>
        /// Decreases key of given node to new key.
        /// </summary>
        /// <param name="newkey">New key of the node, must be less than current key.</param>
        /// <param name="old">Node, whose key is to be decreased.</param>
        /// <returns>Whether the operation succeeded or not (e.g. node is null OR newkey is greater than old key)</returns>
        public bool DecreaseKey(E newkey, Node<T, E> old)
        {
            // no node with that id
            if (old == null) { return false; }
            // new key is not less than old key
            if (newkey.CompareTo(old.Key) >= 0) { return false; }
            
            LastOperationSteps = 0;
            old.Key = newkey;
            CheckMinimum(old);
            Node<T, E> par = old.Parent;
            // it's already root
            if (old.IsRoot()) { return true; }
            MoveToRoots(old);
            if (!Naive) { CheckParents(par); }
            return true;
        }
        #endregion
    }

    /// <summary>
    /// Auxiliary class for consolidation (in delete min).
    /// </summary>
    /// <typeparam name="T">Identifier of nodes.</typeparam>
    /// <typeparam name="E">Priority (key) of nodes.</typeparam>
    class ConsolidationArray<T, E> where T : IEquatable<T>
                                   where E : IComparable<E>
    {
        Node<T, E>[] nodes;
        public int Steps { get; private set; } = 0;
        public Node<T, E> NewMinimum { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="size">Size of consolidation array.</param>
        public ConsolidationArray(int size)
        {
            nodes = new Node<T, E>[size];
        }

        #region Methods
        /// <summary>
        /// Consolidates the given node, until there is no other node with that order.
        /// </summary>
        /// <param name="n">Node to consolidate.</param>
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

        /// <summary>
        /// Merges two heaps, and returns the resulting heap.
        /// </summary>
        /// <param name="first">First heap to merge.</param>
        /// <param name="second">Second heap to merge.</param>
        /// <returns>Root node of merged heap.</returns>
        private Node<T, E> MergeHeaps(Node<T, E> first, Node<T, E> second)
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

        /// <summary>
        /// Creates heap (e.g. roots list) out of consolidation array. Main output method of this class.
        /// </summary>
        /// <returns>Roots list of new heap.</returns>
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

        /// <summary>
        /// Checks if the given node is minimum of the new heap.
        /// </summary>
        /// <param name="node">Potential new minimum.</param>
        private void CheckMinimum(Node<T, E> node)
        {
            if (NewMinimum == null) { NewMinimum = node; }
            else if (NewMinimum.Key.CompareTo(node.Key) > 0) { NewMinimum = node; }
        }
        #endregion
    }
}
