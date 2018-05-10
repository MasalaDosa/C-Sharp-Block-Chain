using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockChain
{
    /// <summary>
    /// A block has an index, timestamp, string data and a hash.
    /// The hash includes the previous blocks hash.
    /// </summary>
    public class Block
    {
        public long Index { get; set; }
        public DateTime Timestamp { get; set; }
        public string Data { get; set; }
        public string PreviousHash { get; set; }
        public string Hash { get; set; }
        // For our proof of work.
        long _nonce;

        Block(long index, string data, string previousHash)
        {
            Index = index;
            Timestamp = DateTime.UtcNow;
            Data = data;
            PreviousHash = previousHash;
            Hash = CalculateHash();
			MineBlock(Chain.DIFFICULTY);
        }

        internal string CalculateHash()
        {
            string toBeHashed = string.Format(
                "{0}_{1}_{2}_{3}_{4}",
                Index,
                Timestamp.ToBinary(),
                Data,
                PreviousHash,
                _nonce
            );
            return Utils.BytesToUTF8(Utils.Hash(toBeHashed.ToString(), string.Empty));
        }

        /// <summary>
        /// In reality each miner would start iterating from a random point. 
        /// Some miners may even try random numbers for nonce, or messing with the timestamp etc.
        /// </summary>
        /// <param name="difficulty">Difficulty.</param>
        void MineBlock(int difficulty)
        {
            // Create a string with difficulty * "0"
            // We want to modify our data until the hash starts with this many 0s.
            String target = new String(new char[difficulty]).Replace('\0', '0');
            while (!Hash.Substring(0, difficulty).Equals(target))
            {
                _nonce++;
                Hash = CalculateHash();
            }
            Console.WriteLine("Block Mined: " + this.Hash);
        }

        /// <summary>
        /// Create an initial 'Genesis' block
        /// </summary>
        /// <returns>The zero block.</returns>
        internal static Block CreateGenesisBlock()
        {
            return new Block(
                0,
                "Genesis Block",
                string.Empty
            );
        }

        /// <summary>
        /// Create a new block
        /// </summary>
        /// <returns>The next block.</returns>
        /// <param name="previousBlock">Previous block.</param>
        /// <param name="data">Data.</param>
        internal static Block CreateNextBlock(Block previousBlock, string data)
        {
            return new Block(
                previousBlock.Index + 1,
                data,
                previousBlock.Hash
            );
        }

        public override string ToString()
        {
            return string.Format("Block {0} @ {1}: {2}\n Hash: {3}", Index, Timestamp, Data, Hash);
        }
    }
}
