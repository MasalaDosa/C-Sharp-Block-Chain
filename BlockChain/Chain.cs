using System;
using System.Collections.Generic;
using System.Linq;
namespace BlockChain
{
    /// <summary>
    /// A chain is simple a list of blocks
    /// Each block is 'chained' to the previous block via its hash
    /// </summary>
    public class Chain
    {
        List<Block> _backingList;

        /// <summary>
        /// Creates a new chain and initialises it with the 'zero block'
        /// </summary>
        public Chain()
        {
            // Create backing list and zero block
            _backingList = new List<Block>() 
            { 
                Block.CreateZeroBlock() 
            };
        }

        /// <summary>
        /// Add the specified data to the chain by creating a new block
        /// </summary>
        /// <returns>The add.</returns>
        /// <param name="data">Data.</param>
        public void Add(string data)
        {
            _backingList.Add(
                Block.CreateNextBlock(_backingList.Last(), data)
            );
        }

        /// <summary>
        /// Get the underlying list
        /// (Actually a copy of it to prevent re-ordering / removal / etc)
        /// </summary>
        /// <value>The blocks.</value>
        public IEnumerable<Block> Blocks => _backingList.ToList();

        /// <summary>
        /// Ensure the chain is still consistent
        /// </summary>
        /// <returns><c>true</c>, if check was consistencyed, <c>false</c> otherwise.</returns>
        public bool ConsistencyCheck()
        {
            foreach (var b in Blocks)
            {
                if(b.HashBlock() != b.Hash)
                {
                    Console.WriteLine("Inconsistent data found: {0}", b);
                    return false;
                }
            }
            return true;
        }
    }
}
