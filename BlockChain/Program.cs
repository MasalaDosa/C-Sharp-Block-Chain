using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BlockChain
{
    class Program
    {
        static void Main(string[] args)
		{
			//ChainPlaying();
			WalletAndTransactions();

		}

		private static void WalletAndTransactions()
		{
            Wallet coinBaseWallet = new Wallet();
            Wallet myWallet = new Wallet();
            Wallet yourWallet = new Wallet();


			var chain = new Chain();

			// Manually create the first transaction.
            // We create 100 coins and send them to 'me'
			var genesisTransaction = new Transaction(coinBaseWallet.PublicKey, myWallet.PublicKey, 100M, null);
			genesisTransaction.Sign(coinBaseWallet.PrivateKey);
			genesisTransaction.TransactionId = "0";
			genesisTransaction.Outputs.Add(new TransactionOutput(genesisTransaction.Recipient, genesisTransaction.Value, genesisTransaction.TransactionId));
			
            // Its important to store our first transaction in the UTXOs list.
			chain.Utxos[genesisTransaction.Outputs[0].Id] = genesisTransaction.Outputs[0];

            // Create genesis block
            var genesisBlock = chain.CreateGenesisBlock();
			genesisBlock.AddTransaction(genesisTransaction, chain);
            chain.AddAndMineBlock(genesisBlock);
			chain.Dump();
			Console.WriteLine("Me: {0} You: {1}.", myWallet.GetBalance(chain), yourWallet.GetBalance(chain));

            // Now our chain is ready - lets try some transactions
            Console.WriteLine("Attempting to send 50 coins from  me to you.");
            Block block1 = chain.CreateNextBlock();
            var trans = myWallet.SendFunds(yourWallet.PublicKey, 50, chain);
            block1.AddTransaction(trans, chain);
            chain.AddAndMineBlock(block1);
            chain.Dump();
			Console.WriteLine("Me: {0} You: {1}.", myWallet.GetBalance(chain), yourWallet.GetBalance(chain));

            Block block2 = chain.CreateNextBlock();
            Console.WriteLine("Attempting to send more funds (1000) than I have.");
            block2.AddTransaction(myWallet.SendFunds(yourWallet.PublicKey, 1000, chain), chain);
            chain.AddAndMineBlock(block2);
            chain.Dump();
            Console.WriteLine("Me: {0} You: {1}.", myWallet.GetBalance(chain), yourWallet.GetBalance(chain));

            Block block3 = chain.CreateNextBlock();
            Console.WriteLine("Attempting to send 20 coins from you to me.");
            block3.AddTransaction(yourWallet.SendFunds(myWallet.PublicKey, 20, chain), chain);
            chain.AddAndMineBlock(block3);
            chain.Dump();
            Console.WriteLine("Me: {0} You: {1}.", myWallet.GetBalance(chain), yourWallet.GetBalance(chain));
		}

		//private static void ChainPlaying()
		//{
		//	// Create an (in memory) block chain and the zero block
		//	var blockChain = new Chain();

		//	// Now lets add some data
		//	string data;
		//	while (true)
		//	{
		//		Console.WriteLine("Block data (blank to finish): ");
		//		data = Console.ReadLine();
		//		if (string.IsNullOrWhiteSpace(data))
		//		{
		//			break;
		//		}
		//		else
		//		{
		//			blockChain.Add(data);
		//		}
		//	}

		//	// Dump the blocks
		//	foreach (var b in blockChain.Blocks)
		//	{
		//		Console.WriteLine(b.ToString());
		//	}

		//	// Try using the debugger to tamper with some data before doing the check below
		//	// blockChain.Blocks[2].Data = "Changed";
		//	// blockChain.Blocks.RemoveAt(2);

		//	// Check consistency
		//	Console.WriteLine("Consistent: {0}", blockChain.ConsistencyCheck());
		//}
	}





}
