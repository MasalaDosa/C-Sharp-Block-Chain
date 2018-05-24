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

		public String MerkleRoot { get; set; }
		public List<Transaction> Transactions { get; set; } = new List<Transaction>();
          
        public string PreviousHash { get; set; }
        public string Hash { get; set; }
        // For our proof of work.
        long _nonce;

        Block(long index, string previousHash)
        {
            Index = index;
            Timestamp = DateTime.UtcNow;
            PreviousHash = previousHash;
            Hash = CalculateHash();
        }

        /// <summary>
        /// Adds a transaction to this block
        /// </summary>
        /// <returns><c>true</c>, if transaction was added, <c>false</c> otherwise.</returns>
        /// <param name="transaction">Transaction.</param>
        public bool AddTransaction(Transaction transaction, Chain chain)
        {
            //process transaction and check if valid, unless block is genesis block then ignore.
            if (transaction == null) return false;

            if ((PreviousHash != string.Empty)) // Test for not genesis block
            {
                if ((transaction.ProcessTransaction(chain) != true))
                {
                    Console.WriteLine("Transaction failed to process. Discarded.");
                    return false;
                }
            }
            Transactions.Add(transaction);
            Console.WriteLine("Transaction Successfully added to Block");
            return true;
        }

        internal string CalculateHash()
        {
            string toBeHashed = string.Format(
                "{0}_{1}_{2}_{3}_{4}",
                Index,
                Timestamp.ToBinary(),
                MerkleRoot,
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
        public void MineBlock(int difficulty)
        {
			MerkleRoot = Utils.MerkleRoot(Transactions);
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
                string.Empty
            );
        }

        /// <summary>
        /// Create a new block
        /// </summary>
        /// <returns>The next block.</returns>
        /// <param name="previousBlock">Previous block.</param>
        internal static Block CreateNextBlock(Block previousBlock)
        {
            return new Block(
                previousBlock.Index + 1,
                previousBlock.Hash
            );
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine(string.Format("Block {0} @ {1}: {2}\n Hash: {3}", Index, Timestamp, MerkleRoot, Hash));
            foreach (var t in Transactions)
            {
                result.AppendLine(string.Format("\t{0}", t.ToString()));
            }
            return result.ToString();
        }
    }
}
