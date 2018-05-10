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
        public ECPrivateKeyParameters PrivateKey { get; private set; }

        /// <summary>
        /// The Public Key of our wallet - our address.
        /// </summary>
        /// <value>The public key.</value>
        public ECPublicKeyParameters PublicKey { get; private set; }

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
    }
}
