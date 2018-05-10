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
			ChainPlaying();
			WalletAndTransactions();

		}

		private static void WalletAndTransactions()
		{
			Wallet me = new Wallet();
			Wallet you = new Wallet();

			Console.WriteLine("Creating a transaction from Me ({0}) to You ({1}).", me.PublicKeyString, you.PublicKeyString);
			Transaction t = new Transaction(me.PublicKey, you.PublicKey, 999.50M, null);
			Console.WriteLine("Signing transaction by me ({0})", me.PrivateKeyString);
			t.CaclulateSignature(me.PrivateKey);
			Console.WriteLine("Is signature valid? {0}",t.VerifySignature());
			Console.WriteLine("Changing transaction");
			t.Value = 99999M;
			Console.WriteLine("Is signature valid? {0}", t.VerifySignature());

   
		}
        
		private static void ChainPlaying()
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
