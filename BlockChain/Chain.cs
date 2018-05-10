using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockChain
{

    /// <summary>
    /// A chain is simple a list of blocks
    /// Each block is 'chained' to the previous block via its hash
    /// </summary>
    public class Chain
    {
		/// <summary>
        /// The difficulty of mining
        /// </summary>
        public const int DIFFICULTY = 2;

        List<Block> _backingList;

        /// <summary>
        /// Creates a new chain and initialises it with the 'genesis block'
        /// </summary>
        public Chain()
        {
            var genesisBlock = Block.CreateGenesisBlock();
            _backingList = new List<Block>()
            {
                genesisBlock
            };
        }

        public Chain(List<Block> blocks)
        {
            if (blocks == null)
                throw new ArgumentNullException("blocks");
            _backingList = blocks;
            if (!ConsistencyCheck()) throw new ArgumentException("Inconsistent chain", "blocks");
        }

        /// <summary>
        /// Add the specified data to the chain by creating and mining a new block
        /// </summary>
        /// <returns>The add.</returns>
        /// <param name="data">Data.</param>
        public void Add(string data)
        {
            var newBlock = Block.CreateNextBlock(_backingList.Last(), data);
            _backingList.Add(newBlock);
        }

        /// <summary>
        /// Get the underlying list
        /// </summary>
        /// <value>The blocks.</value>
        public List<Block> Blocks => _backingList;

        /// <summary>
        /// Ensure the chain is still consistent
        /// </summary>
        /// <returns><c>true</c>, if chain was consistent, <c>false</c> otherwise.</returns>
        public bool ConsistencyCheck()
        {
            String hashTarget = new String(new char[DIFFICULTY]).Replace('\0', '0');

			for (int i = 0; i < _backingList.Count(); i++)
			{
				// Does our hash match?
				if (_backingList[i].CalculateHash() != _backingList[i].Hash)
				{
					Console.WriteLine("Inconsistent data found - Current hash not equal: {0}", _backingList[i]);
					return false;
				}
				// Does the previous hash match?
				if (i > 1 && _backingList[i].PreviousHash != _backingList[i - 1].Hash)
				{
					Console.WriteLine("Inconsistent data found - Previous hash not equal: {0}", _backingList[i]);
					return false;
				}

				// And check if hash is solved
				if (!_backingList[i].Hash.Substring(0, DIFFICULTY).Equals(hashTarget))
				{
					Console.WriteLine("This block hasn't been mined: {0}", _backingList[i]);
					return false;
				}
			}
            return true;
        }
    }
}
