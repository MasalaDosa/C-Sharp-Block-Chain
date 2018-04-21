using System;
using System.Collections.Generic;

namespace BlockChain
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create an (in memory) block chain and the zero block
            var blockChain = new Chain();

            // Now lets add some data
            string data;
            while (true)
            {
                Console.WriteLine("Block data (blank to finish): ");
                data = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(data))
                {
                    break;
                }
                else
                {
                    blockChain.Add(data);
                }
            }

            // Dump the blocks
            foreach (var b in blockChain.Blocks)
            {
                Console.WriteLine(b.ToString());
            }

            // Try using the debugger to tamper with some data before doing the check below
            // blockChain.Blocks[2].Data = "Changed";
            // blockChain.Blocks.RemoveAt(2);

            // Check consistency
            Console.WriteLine("Consistent: {0}", blockChain.ConsistencyCheck());

        }
    }
}
