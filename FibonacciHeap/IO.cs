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
                    int id2 = int.Parse(tokens[1]);
                    Heap.DecreaseKey(id2, int.Parse(tokens[2]), Nodes[id2]);
                    totalDK++;
                    AverageDecreaseKeySteps += Heap.LastOperationSteps;
                    if (Heap.LastOperationSteps > MaxDecreaseKeySteps) { MaxDecreaseKeySteps = Heap.LastOperationSteps; }
                    break;
            }
            //Heap.Roots.Validate(int.MinValue, null);
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
    }
}

