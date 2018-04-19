using System;
using System.Security.Cryptography;
using System.Text;

namespace BlockChain
{
    /// <summary>
    /// A block has an index, timestamp, string data and a hash.
    /// The hash includes the previous blocks hash,
    /// </summary>
    public class Block
    {
        public long Index { get; private set; }
        public DateTime Timestamp { get; private set; }
        public string Data { get; private set; }
        public string PreviousHash { get; private set; }
        public string Hash { get; private set; }

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
                "{0}_{1}_{2}_{3}",
                this.Index,
                this.Timestamp.ToBinary(),
                this.Data,
                this.PreviousHash
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
        /// Create an inital 'zero' block
        /// </summary>
        /// <returns>The zero block.</returns>
        internal static Block CreateZeroBlock()
        {
            return new Block(
                0,
                "Zero Block",
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
