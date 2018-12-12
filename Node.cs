using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibonacciHeap
{
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
        public void InsertRight(Node<T, E> right)
        {
            Debug.Assert(Right == null);

            right.Right = null;
            right.Left = this;
            Right = right;
        }

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

        public bool IsRoot()
        {
            return Parent == null;
        }

        public void SetRoot()
        {
            Parent = null;
            LostSon = false;
        }

        public void ReleaseChildren()
        {
           foreach (var child in Children)
            {
                child.SetRoot();
            }
        }

        public void InsertIntoChildren(Node<T, E> newnode)
        {
            newnode.Parent = this;
            Children.Insert(newnode);
        }

        public override string ToString()
        {
            return String.Format("({0},{1})", Identifier, Key);
        }

        public void ValidateNode(Node<T, E> father, int sonIndex)
        {
            Debug.Assert(Parent == father);            
            if (IsRoot()) { Debug.Assert(!LostSon); }
            else 
            {
                // heap invariant
                Debug.Assert(Key.CompareTo(father.Key) >= 0);
                // fibonacci heap invariant
                if (LostSon) { Debug.Assert(Order >= sonIndex - 2); }
                else { Debug.Assert(Order >= sonIndex - 1); }
            }
            if (Children.Head != null)
            {
                Children.Validate(this);
            }
        }
    }
}
