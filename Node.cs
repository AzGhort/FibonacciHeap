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
        public BidirectionalCyclicLinkedList<T, E> Children { get; set; } = new BidirectionalCyclicLinkedList<T, E>();
        #endregion

        public Node(T id, E key)
        {
            Identifier = id;
            Key = key;
        }
        public void InsertNewRight(Node<T, E> right)
        {
            var oldRight = Right;
            oldRight.Left = right;

            right.Right = oldRight;
            right.Left = this;
            Right = right;

            // we had not any sibling
            if (Left.Identifier.Equals(Identifier))
            {
                Left = Right;
            }
        }
        public bool Destroy()
        {
            // we are the last node
            if (Right == Left && Right == this && Left == this) return false;
          
            Right.Left = Left;
            Left.Right = Right;
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
            if (Children.Handle != null)
            {
                var curN = Children.Handle;
                curN.SetRoot();
                curN = curN.Right;
                while (curN != Children.Handle)
                {
                    curN.SetRoot();
                    curN = curN.Right;
                }
            }
        }
        public void InsertIntoChildren(Node<T, E> newnode)
        {
            newnode.Parent = this;
            Children.Insert(newnode);
        }
        public void ValidateNode(E fatherVal, Node<T, E> father)
        {
            Debug.Assert(Left.Right == Right.Left && Left.Right == this && Right.Left == this);
            Debug.Assert(Parent == father);
            Debug.Assert(Key.CompareTo(fatherVal) >= 0);
            if (father == null)
            {
                Debug.Assert(!LostSon);
            }
            if (Children.Handle != null)
            {
                Children.Validate(Key, this);
            }
        }
    }
}
