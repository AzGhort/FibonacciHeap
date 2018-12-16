using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibonacciHeap
{
    /// <summary>
    /// Linked list of heap nodes.
    /// </summary>
    /// <typeparam name="T">Identifier of nodes.</typeparam>
    /// <typeparam name="E">Priority (key) of nodes.</typeparam>
    class LinkedList<T, E> : IEnumerable<Node<T, E>> where T : IEquatable<T>
                                                    where E : IComparable<E>
    {
        public Node<T, E> Head { get; private set; }
        public Node<T, E> Tail { get; private set; }
        public int NodesCount { get; private set; } = 0;

        #region Linked list methods
        /// <summary>
        /// Inserts node to linked list.
        /// </summary>
        /// <param name="newnode">Node to be inserted.</param>
        public void Insert(Node<T, E> newnode)
        {
            if (Head == null)
            {
                Head = newnode;
                Tail = newnode;
            }
            else
            {
                Tail.InsertRight(newnode);
                Tail = newnode;
            }
            NodesCount++;
        }

        /// <summary>
        /// Merges another list to this list.
        /// </summary>
        /// <param name="other">List to merge to this list.</param>
        public void Merge(LinkedList<T, E> other)
        {
            NodesCount += other.NodesCount;
            // edge cases
            if (other.Head == null) { return; }
            if (Head == null)
            {
                Head = other.Head;
                Tail = other.Tail;
            }
            else
            {
                other.Head.Left = Tail;
                Tail.Right = other.Head;
                Tail = other.Tail;
            }
        }

        /// <summary>
        /// Safely deletes the given node - e.g. handle pointers, Head/Tail...
        /// </summary>
        /// <param name="node">Node to be deleted.</param>
        public void SafeDeleteNode(Node<T, E> node)
        {
            if (node == Head)
            {
                Head = Head.Right;
            }
            else if (node == Tail)
            {
                Tail = Tail.Left;
            }
            if (!node.Destroy())
            {
                Head = null;
                Tail = null;
            }
            NodesCount--;
        }
        #endregion

        #region Standard methods
        /// <summary>
        /// String version of this object.
        /// </summary>
        /// <returns>(Identifier, Key)->(Identifier, Key)->...</returns>
        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            var curN = Head;
            if (Head == null) { return ""; }
            foreach (var node in this)
            {
                b.Append(String.Format("({0},{1})->", node.Identifier, node.Key));
            }
            return b.ToString();
        }
        
        /// <summary>
        /// Enumerator of the linked list.
        /// </summary>
        /// <returns>Enumerator of this object.</returns>
        public IEnumerator<Node<T, E>> GetEnumerator()
        {
            var curN = Head;
            while (curN != null)
            {
                // robust, what if someone messes up current node (e.g. in consolidation)
                var next = curN.Right;
                yield return curN;
                curN = next;
            }
        }

        /// <summary>
        /// Mandatory IEnumerable method.
        /// </summary>
        /// <returns>Enumerator of the list.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}