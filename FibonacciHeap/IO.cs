using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibonacciHeap
{
    /// <summary>
    /// Class managing IO and controling heap - executes commands, and holds statistics about the heap.
    /// </summary>
    class HeapTester
    {
        public StreamWriter writer;
        public FibonacciHeap<int, int> Heap { get; private set; }

        #region Statistics counters
        public int MaxDecreaseKeySteps { get; private set; } = 0;
        public long AverageDecreaseKeySteps { get; private set; } = 0;
        public int MaxDeleteMinimumSteps { get; private set; } = 0;
        public long AverageDeleteMinimumSteps { get; private set; } = 0;
        int totalCurNodes = 0;
        int totalDK = 0;
        int totalMin = 0;
        bool naive;
        #endregion

        /// <summary>
        /// All potential nodes of current heap, indexed by identifier.
        /// </summary>
        private Node<int, int>[] Nodes;

        /// <summary>
        /// Constructor of Heap Tester.
        /// </summary>
        /// <param name="output">Name of the output file.</param>
        /// <param name="n">Whether the heap is naive.</param>
        public HeapTester(string output, bool n)
        {
            writer = new StreamWriter(output);
            naive = n;
        }

        #region Methods
        /// <summary>
        /// Main IO method - reads commands from the console and executes them.
        /// </summary>
        public void CreateHeap()
        {
            string s = "";
            while ((s = Console.ReadLine()) != null)
            {
                ExecuteCommand(s);
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
                    if (Heap.NodesCount > 0) Nodes[Heap.Minimum.Identifier] = null;                    
                    var min = Heap.DeleteMinimum();                    
                    if (min != null)
                    {
                        totalMin++;
                        AverageDeleteMinimumSteps += Heap.LastOperationSteps;
                        if (Heap.LastOperationSteps > MaxDeleteMinimumSteps) { MaxDeleteMinimumSteps = Heap.LastOperationSteps; }
                    }
                    break;
                // decrease key
                case "D":
                    var decreased = Heap.DecreaseKey(int.Parse(tokens[2]), Nodes[int.Parse(tokens[1])]);
                    if (decreased)
                    {
                        totalDK++;
                        AverageDecreaseKeySteps += Heap.LastOperationSteps;
                        if (Heap.LastOperationSteps > MaxDecreaseKeySteps) { MaxDecreaseKeySteps = Heap.LastOperationSteps; }
                    }
                    break;
            }
        }

        /// <summary>
        /// Outputs statistics of the heap built to the output file.
        /// </summary>
        public void FinishCurrentHeap()
        {
            if (totalDK == 0) { totalDK = 1; }
            if (totalMin == 0) { totalMin = 1; }
            writer.WriteLine("{0} {1} {2} {3} {4}", totalCurNodes, AverageDecreaseKeySteps*1.0/totalDK, MaxDecreaseKeySteps, AverageDeleteMinimumSteps*1.0/totalMin, MaxDeleteMinimumSteps);
            MaxDeleteMinimumSteps = 0;
            MaxDecreaseKeySteps = 0;
            AverageDeleteMinimumSteps = 0;
            AverageDecreaseKeySteps = 0;
            totalDK = 0;
            totalMin = 0;
        }
        #endregion

        /// <summary>
        /// Auxiliary testing method - randomly executes 1M inserts/deletes/decreases key in a 1K nodes heap.
        /// </summary>
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
            }
        }
    }
}

