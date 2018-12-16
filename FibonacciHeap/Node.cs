using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibonacciHeap
{
    /// <summary>
    /// Node of the heap/linked list.
    /// </summary>
    /// <typeparam name="T">Identifier of node.</typeparam>
    /// <typeparam name="E">Key (priority) of node.</typeparam>
    class Node<T, E> where T : IEquatable<T>
                     where E : IComparable<E>
    {
        #region Values of node
        public T Identifier { get; private set; }
        public E Key { get; set; }
        public bool LostSon { get; set; } = false;
        public int Order { get { return Children.NodesCount; } }
        #endregion

        #region Structure pointers
        public Node<T, E> Left { get; set; }
        public Node<T, E> Right { get; set; }
        public Node<T, E> Parent { get; set; }
        public LinkedList<T, E> Children { get; set; } = new LinkedList<T, E>();
        #endregion

        public Node(T id, E key)
        {
            Identifier = id;
            Key = key;
        }

        #region Methods
        /// <summary>
        /// Inserts given node as next, sets it's succesor to null.
        /// </summary>
        /// <param name="right">Next node.</param>
        public void InsertRight(Node<T, E> right)
        {
            right.Right = null;
            right.Left = this;
            Right = right;
        }

        /// <summary>
        /// Destroys the current node, e.g. handles neighbours' pointers.
        /// </summary>
        /// <returns>Whether the operation was succesful - e.g. this node is NOT the last node of list.</returns>
        public bool Destroy()
        {
            // we are the last node
            if (Right == null && Left == null) return false;

            if (Right != null) Right.Left = Left;
            if (Left != null) Left.Right = Right;
            Right = null;
            Left = null;
            return true;
        }

        /// <summary>
        /// Checks whether the node is root (no parent).
        /// </summary>
        /// <returns>Whether the node is root.</returns>
        public bool IsRoot()
        {
            return Parent == null;
        }

        /// <summary>
        /// Sets "root attributes" on this node - parent is null, LostSon is false.
        /// </summary>
        public void SetRoot()
        {
            Parent = null;
            LostSon = false;
        }

        /// <summary>
        /// Releases all children of node - sets them all to root.
        /// </summary>
        public void ReleaseChildren()
        {
            foreach (var child in Children)
            {
                child.SetRoot();
            }
        }
                
        /// <summary>
        /// Inserts new node into children of this node.
        /// </summary>
        /// <param name="newnode">New node to be inserted.</param>
        public void InsertIntoChildren(Node<T, E> newnode)
        {
            newnode.Parent = this;
            Children.Insert(newnode);
        }

        /// <summary>
        /// String version of this object.
        /// </summary>
        /// <returns>(Identifier, Key)</returns>
        public override string ToString()
        {
            return String.Format("({0},{1})", Identifier, Key);
        }
        #endregion
    }
}
