using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibonacciHeap
{
    class BidirectionalCyclicLinkedList<T, E> where T : IEquatable<T>
                                              where E : IComparable<E>
    {
        public Node<T, E> Handle = null;
        public int NodesCount { get; private set; } = 0;
        public void Insert(Node<T, E> newnode)
        {
            if (Handle == null)
            {
                Handle = newnode;
                Handle.Right = Handle;
                Handle.Left = Handle;
            }
            else
            {
                Handle.InsertNewRight(newnode);
            }
            NodesCount++;
        }
        public void Merge(BidirectionalCyclicLinkedList<T, E> other)
        {
            NodesCount += other.NodesCount;
            // edge cases
            if (other.Handle == null)  { return; }
            if (Handle == null) { Handle = other.Handle; }            
            else
            {
                var lowerEndA = Handle.Right;
                var upperEndA = Handle;
                var lowerEndB = other.Handle;
                var upperEndB = other.Handle.Right;
                // upper end (A -> right -> B)
                upperEndA.Right = upperEndB;
                upperEndB.Left = upperEndA;
                // lower end (B -> right -> A)
                lowerEndB.Right = lowerEndA;
                lowerEndA.Left = lowerEndB;
            }
        }
        public void SafeDeleteNode(Node<T, E> node)
        {
            if (node == Handle)
            {
                Handle = Handle.Right;
            }
            if (!node.Destroy())
            {
                Handle = null;
            }
            NodesCount--;
        }
        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            var curN = Handle;
            if (Handle == null) { return ""; }
            while (true)
            {
                //b.Append(String.Format("({0})-{1}-({2}) -> ", curN.Left.Identifier, curN.Identifier, curN.Right.Identifier));
                b.Append(String.Format("({0},{1})->", curN.Identifier, curN.Key));
                curN = curN.Right;
                if (curN.Identifier.Equals(Handle.Identifier)) break;
            }
            return b.ToString();
        }
        public void Validate(E val, Node<T,E> node)
        {
            if (Handle == null) return;
            var curN = Handle;
            curN.ValidateNode(val, node);
            curN = curN.Right;
            int counter = 1;
            while (curN != Handle)
            {
                curN.ValidateNode(val, node);
                curN = curN.Right;
                counter++;
            }
            //Debug.Assert(counter == NodesCount);
        }
    }
}