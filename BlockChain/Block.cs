using System;
using System.Security.Cryptography;
using System.Text;

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

        internal Block(long index, string data, string previousHash)
        {
            this.Index = index;
            this.Timestamp = DateTime.UtcNow;
            this.Data = data;
            this.PreviousHash = previousHash;
            this.Hash = this.HashBlock();
        }

        internal string HashBlock()
        {
            StringBuilder hash = new StringBuilder();
            string toBeHashed = string.Format(
                "{0}_{1}_{2}_{3}_{4}",
                this.Index,
                this.Timestamp.ToBinary(),
                this.Data,
                this.PreviousHash,
                this._nonce
            );
            using (var sha = SHA256.Create())
            {
                Byte[] result = sha.ComputeHash(Encoding.UTF8.GetBytes(toBeHashed));
                foreach (var b in result)
                {
                    hash.Append(b.ToString("x2"));
                }
            }
            return hash.ToString();
        }

        /// <summary>
        /// In reality each miner would start iterating from a random point. 
        /// Some miners may even try random numbers for nonce, or messing with the timestamp etc.
        /// </summary>
        /// <param name="difficulty">Difficulty.</param>
        public void MineBlock(int difficulty)
        {
            // Create a string with difficulty * "0"
            // We want to modify our data until the hash starts with this many 0s.
            String target = new String(new char[difficulty]).Replace('\0', '0');  
            while (!Hash.Substring(0, difficulty).Equals(target))
            {
                _nonce++;
                this.Hash = HashBlock();
            }
            Console.WriteLine("Block Mined!!! : " + this.Hash);
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
