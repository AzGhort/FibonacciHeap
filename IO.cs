using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibonacciHeap
{  
    class HeapTester
    {
        public StreamWriter writer;
        public FibonacciHeap<int, int> Heap { get; private set; }
        public int MaxDecreaseKeySteps { get; private set; } = 0;
        public int AverageDecreaseKeySteps { get; private set; } = 0;
        public int MaxDeleteMinimumSteps { get; private set; } = 0;
        public int AverageDeleteMinimumSteps { get; private set; } = 0;
        private Node<int, int>[] Nodes;
        int totalCurNodes = 0;
        int totalDK = 0;
        int totalMin = 0;
        bool naive;

        public HeapTester(string output, bool n)
        {
            writer = new StreamWriter(output);
            naive = n;
        }

        public void CreateHeap()
        {
            using (StreamReader reader = new StreamReader("in.txt"))
            {
                string s = "";
                while ((s = reader.ReadLine()) != null)
                {
                    ExecuteCommand(s);
                }
            }
        }
       
        /// <summary>
        /// Executes one command from the input.
        /// </summary>
        /// <param name="command">Command to be executed.</param>
        private void ExecuteCommand(string command)
        {
            string[] tokens = command.Split(new char[] { ' ' });            
            switch (tokens[0])
            {
                // new heap
                case "#":
                    if (totalCurNodes > 0) FinishCurrentHeap();
                    totalCurNodes = int.Parse(tokens[1]);
                    Heap = new FibonacciHeap<int, int>();
                    Nodes = new Node<int, int>[totalCurNodes];
                    Heap.Naive = naive;
                    break;
                // insert
                case "I":
                    int id = int.Parse(tokens[1]);
                    var n = new Node<int, int>(id, int.Parse(tokens[2]));
                    Heap.Insert(n);
                    Nodes[id] = n;
                    break;
                // delete minimum
                case "M":
                    Nodes[Heap.Minimum.Identifier] = null;
                    Heap.DeleteMinimum();
                    totalMin++;
                    AverageDeleteMinimumSteps += Heap.LastOperationSteps;                    
                    if (Heap.LastOperationSteps > MaxDeleteMinimumSteps) { MaxDeleteMinimumSteps = Heap.LastOperationSteps; }
                    break;
                // decrease key
                case "D":
                    Heap.DecreaseKey(int.Parse(tokens[2]), Nodes[int.Parse(tokens[1])]);
                    totalDK++;
                    AverageDecreaseKeySteps += Heap.LastOperationSteps;
                    if (Heap.LastOperationSteps > MaxDecreaseKeySteps) { MaxDecreaseKeySteps = Heap.LastOperationSteps; }
                    break;
            }
            Heap.Roots.Validate(null);
        }
       
        /// <summary>
        /// Outputs info about the heap built to the output file.
        /// </summary>
        public void FinishCurrentHeap()
        {
            writer.WriteLine("{0} {1} {2} {3} {4}", totalCurNodes, AverageDecreaseKeySteps * 1.0 / totalCurNodes, MaxDecreaseKeySteps, AverageDeleteMinimumSteps * 1.0 / totalCurNodes, MaxDeleteMinimumSteps);
            MaxDeleteMinimumSteps = 0;
            MaxDecreaseKeySteps = 0;
            AverageDeleteMinimumSteps = 0;
            AverageDecreaseKeySteps = 0;
            totalDK = 0;
            totalMin = 0;
        }

        public static void TestHeap()
        {
            var sequenceLength = 1000000;
            var n = 1000;
            var maxPriority = 1000;
            var random = new Random(0);
            var nodes = new Node<int, int>[n];
            var heap1 = new FibonacciHeap<int, int>();

            for (var i = 0; i < sequenceLength; i++)
            {
                var r = random.Next(5);

                if (r != 1 && r != 2) // Insert
                {
                    var id = random.Next(n);
                    var key = random.Next(maxPriority);
                    if (nodes[id] != null)
                    {
                        continue;
                    }
                    Node<int, int> node = new Node<int, int>(id, key);
                    heap1.Insert(node);
                    nodes[id] = node;                 
                }
                else if (r == 1) // Delete min
                {
                    var min1 = heap1.Minimum;
                    if (min1 == null) { continue; }
                    heap1.DeleteMinimum();                 
                    nodes[min1.Identifier] = null;
                }
                else if (r == 2) // Decrease
                {
                    var id = random.Next(n);
                    var key = random.Next(maxPriority);
                    var node1 = nodes[id];
                    heap1.DecreaseKey(key, node1);                 
                }
                heap1.Roots.Validate(null);
            }
        }
    }
}

