using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockChain
{
    public class Wallet
    {
		/// <summary>
        /// The Private Key of our wallet.
        /// </summary>
        /// <value>The private key.</value>
        public ECPrivateKeyParameters PrivateKey { get; set; }

        /// <summary>
        /// The Public Key of our wallet - our address.
        /// </summary>
        /// <value>The public key.</value>
        public ECPublicKeyParameters PublicKey { get; set; }

        /// <summary>
        /// The utxos owned by this wallet
        /// </summary>
		public Dictionary<string, TransactionOutput> Utxos { get; set; } = new Dictionary<string, TransactionOutput>();
    

        /// <summary>
        /// Gets the private key bytes.
        /// </summary>
        /// <value>The private key bytes.</value>
		public byte[] PrivateKeyBytes 
		{
			get
			{
				return PrivateKey.D.ToByteArray();
			}
		}

        /// <summary>
        /// Gets the public key bytes.
        /// </summary>
        /// <value>The public key bytes.</value>
		public byte[] PublicKeyBytes
		{
			get
			{
				return PublicKey.Q.GetEncoded();   

			}
		}

        /// <summary>
        /// Gets the private key string.
        /// </summary>
        /// <value>The private key string.</value>
		public string PrivateKeyString
        {
            get
            {
                return Utils.BytesToUTF8(PrivateKeyBytes);
            }
        }

        /// <summary>
        /// Gets the public key string.
        /// </summary>
        /// <value>The public key string.</value>
        public string PublicKeyString
		{
			get
			{
				return Utils.BytesToUTF8(PublicKeyBytes);
			}
		}

        /// <summary>
        /// Constructs a wallet by randomly generating a key-pair
        /// </summary>
        public Wallet()
        {
            GenerateKeyPair();
        }

        /// <summary>
        /// Constructs a wallet from a known private key
        /// </summary>
        /// <param name="privateKey">Private key.</param>
        public Wallet(string privateKey)
		{
			throw new NotImplementedException();
		}

        /// <summary>
        /// Generates a key pair 
        /// </summary>
        void GenerateKeyPair()
        {
            ECKeyPairGenerator keyGen = new ECKeyPairGenerator("ECDSA");
            SecureRandom secureRandom = SecureRandom.GetInstance("SHA1PRNG");
            Org.BouncyCastle.Asn1.X9.X9ECParameters ecp = Org.BouncyCastle.Asn1.Sec.SecNamedCurves.GetByName("secp192k1");//("prime192v1");//("secp224k1");
            ECDomainParameters ecSpec = new ECDomainParameters(ecp.Curve, ecp.G, ecp.N, ecp.H, ecp.GetSeed());
            ECKeyGenerationParameters ecKeyGenParams = new ECKeyGenerationParameters(ecSpec, secureRandom);
            keyGen.Init(ecKeyGenParams);
            AsymmetricCipherKeyPair keyPair = keyGen.GenerateKeyPair();
            PrivateKey = keyPair.Private as ECPrivateKeyParameters;
            PublicKey = keyPair.Public as ECPublicKeyParameters;
        }

	    /// <summary>
		///  Returns balance and stores the UTXO's owned by this wallet
        /// </summary>
        /// <returns>The balance.</returns>
		public decimal GetBalance(Chain chain) 
		{
			decimal total = 0M;
			foreach(TransactionOutput utxo in chain.Utxos.Values.Where(utxo => utxo.IsMine(PublicKey)))
			{
				Utxos[utxo.Id] = utxo;
				total += utxo.Value;
			}
                     
            return total;
        }
        
        //Generates and returns a new transaction from this wallet.
        public Transaction SendFunds(ECPublicKeyParameters recipient, decimal value, Chain chain) 
		{
            if(GetBalance(chain) < value) { //gather balance and check funds.
                Console.WriteLine("Not Enough funds to send transaction. Transaction Discarded.");
                return null;
            }
            
			//create list of inputs - adding transaction inputs until we've got enough to satify the value
            List<TransactionInput> inputs = new List<TransactionInput>();
        
            decimal total = 0;
            foreach(var utxo in this.Utxos.Values)
			{
				total += utxo.Value;
				inputs.Add(new TransactionInput(utxo.Id));
                if(total > value)
				{
					break;
				}
			}
   
            // Create and sign a new transaction
            Transaction transaction = new Transaction(PublicKey, recipient , value, inputs);
            transaction.Sign(PrivateKey);
            
            // And finally clear down any utxos we've used
            foreach(TransactionInput ti in inputs)
			{
                Utxos.Remove(ti.TransactionOutputId);
            }

            return transaction;
        } 
    }
}
