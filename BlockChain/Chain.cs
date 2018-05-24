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
		List<Block> _backingList;

		/// <summary>
        /// The difficulty of mining
        /// </summary>
        public const int DIFFICULTY = 2;

        /// <summary>
        /// and the minimum allowed transaction size
        /// </summary>
		public const decimal MINIMUM_TRANSACTION = 0.1M;

		/// <summary>
		///  Shortcut dictionary to keep track of all unspent transactions.
		/// </summary>
		public Dictionary<string, TransactionOutput> Utxos = new Dictionary<string, TransactionOutput>();      

        /// <summary>
        /// Creates a new chain and initialises it with the 'genesis block'
        /// </summary>
        public Chain()
        {
            _backingList = new List<Block>();
        }

        /// <summary>
        /// Adds a block to the chain and mines it.
        /// </summary>
        /// <param name="b">The blue component.</param>
        public void AddAndMineBlock(Block b)
        {
            _backingList.Add(b);
            b.MineBlock(DIFFICULTY);
        }

        public Block CreateGenesisBlock()
        {
            return Block.CreateGenesisBlock();
        }

        public Block CreateNextBlock()
        {
            if(_backingList == null || _backingList.Count == 0)
            {
                throw new InvalidOperationException("Attempt to create next block on empty chain.  Create Genesis block first.");
            }
            return Block.CreateNextBlock(_backingList.Last());
        }

        /// <summary>
        /// Ensure the chain is still consistent
        /// </summary>
        /// <returns><c>true</c>, if chain was consistent, <c>false</c> otherwise.</returns>
        public bool ConsistencyCheck()
        {
            String hashTarget = new String(new char[DIFFICULTY]).Replace('\0', '0');
			Dictionary<String, TransactionOutput> tempUtxos = new Dictionary<string, TransactionOutput>(); //a temporary working list of unspent transactions at a given block state.
            tempUtxos[_backingList[0].Transactions[0].Outputs[0].Id] = _backingList[0].Transactions[0].Outputs[0];

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

                // Loop through transactions and checl they are all valid.
                // Note that we skip the genesis block here since we've used that to create currency.
                if (i > 0)
                {
                    TransactionOutput tempOutput;
                    for (int t = 0; t < _backingList[i].Transactions.Count(); t++)
                    {
                        Transaction currentTransaction = _backingList[i].Transactions[t];

                        if (!currentTransaction.VerifySignature())
                        {
                            Console.WriteLine("Signature on Transaction(" + t + ") is Invalid");
                            return false;
                        }

                        if (currentTransaction.SumInputValues() != currentTransaction.SumOutputValues())
                        {
                            Console.WriteLine("Inputs are not equal to outputs on Transaction(" + t + ")");
                            return false;
                        }

                        foreach (TransactionInput input in currentTransaction.Inputs)
                        {
                            tempOutput = tempUtxos[input.TransactionOutputId];

                            if (tempOutput == null)
                            {
                                Console.WriteLine("Referenced input on Transaction(" + t + ") is Missing");
                                return false;
                            }

                            if (input.Utxo.Value != tempOutput.Value)
                            {
                                Console.WriteLine("Referenced input Transaction(" + t + ") value is Invalid");
                                return false;
                            }

                            tempUtxos.Remove(input.TransactionOutputId);
                        }

                        foreach (TransactionOutput output in currentTransaction.Outputs)
                        {
                            tempUtxos[output.Id] = output;
                        }

                        if (currentTransaction.Outputs[0].Recipient != currentTransaction.Recipient)
                        {
                            Console.WriteLine("Transaction(" + t + ") output recipient is not who it should be");
                            return false;
                        }
                        if (currentTransaction.Outputs[1].Recipient != currentTransaction.Sender) // TODO
                        {
                            Console.WriteLine("Transaction(" + t + ") output 'change' is not sender.");
                            return false;
                        }
                    }
                }
            }
		    Console.WriteLine("Blockchain is valid");
            return true;
        }	

        /// <summary>
        /// Dump this instance to console
        /// </summary>
        public void Dump()
        {
            // Dump the blocks
            foreach (var b in _backingList)
            {
                Console.WriteLine(b);
            }

            // Check consistency
            Console.WriteLine("Consistent: {0}", ConsistencyCheck());
        }
    }
}
