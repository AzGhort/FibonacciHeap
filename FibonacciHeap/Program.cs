using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibonacciHeap
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Two args expected, name of output file, and heap type - STANDARD or NAIVE.");
                return;
            }
            bool naive = false;
            if (args[1] == "STANDARD") naive = false;
            else if (args[1] == "NAIVE") naive = true;
            else
            {
                Console.WriteLine("STANDARD or NAIVE expected as the second argument");
            }
            HeapTester tester =  new HeapTester(args[0], naive);
            tester.CreateHeap();
            tester.FinishCurrentHeap();
            tester.writer.Close();
        }
    }
}
