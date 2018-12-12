using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibonacciHeap
{
    class LinkedList<T, E> : IEnumerable<Node<T,E>> where T : IEquatable<T>
                                                    where E : IComparable<E>
    {
        public Node<T, E> Head { get; private set; }
        public Node<T, E> Tail { get; private set; }
        public int NodesCount { get; private set; } = 0;

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

        public void Merge(LinkedList<T, E> other)
        {
            NodesCount += other.NodesCount;
            // edge cases
            if (other.Head == null)  { return; }
            if (Head == null) { Head = other.Head; }
            else
            {
                other.Head.Left = Tail;
                Tail.Right = other.Head;
                Tail = other.Tail;
            }
        }

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

        public void Validate(Node<T,E> node)
        {
            if (Head == null) return;;
            int counter = 0;
            foreach (var curN in this)
            {
                counter++;
                curN.ValidateNode(node, counter);
            }
            Debug.Assert(counter == NodesCount);
        }

        public IEnumerator<Node<T, E>> GetEnumerator()
        {
            var curN = Head;
            while (curN != null)
            {
                yield return curN;
                curN = curN.Right;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}