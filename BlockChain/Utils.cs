using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockChain
{
    public static class Utils
    {
		/// <summary>
        /// Converts an array of bytes to a string
        /// </summary>
        /// <returns>The to UTF.</returns>
        /// <param name="bytes">Bytes.</param>
        public static string BytesToUTF8(Byte[] bytes)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }

        /// <summary>
        /// Hashes a string
        /// </summary>
        /// <returns>The hash.</returns>
        /// <param name="toBeHashed">To be hashed.</param>
        /// <param name="key">Key.</param>
        public static byte[] Hash(string toBeHashed, string key)
        {
            var hmac = new HMac(new Sha256Digest());
            hmac.Init(new KeyParameter(Encoding.UTF8.GetBytes(key)));
            byte[] result = new byte[hmac.GetMacSize()];
            byte[] bytes = Encoding.UTF8.GetBytes(toBeHashed);

            hmac.BlockUpdate(bytes, 0, bytes.Length);
            hmac.DoFinal(result, 0);

            return result;
        }
        
        /// <summary>
        /// Applies ECDSA Signature and returns the result.
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="data"></param>
        /// <returns></returns>
		public static byte[] SignData(ECPrivateKeyParameters privateKey, String data)
        {
            ISigner signer = SignerUtilities.GetSigner("ECDSA");
            signer.Init(true, privateKey);
            var bytes = Encoding.UTF8.GetBytes(data);
            signer.BlockUpdate(bytes, 0, bytes.Length);
            return signer.GenerateSignature();
        }

        /// <summary>
        /// Verifies an ECDSA Signature.
        /// </summary>
        /// <param name="publicKey"></param>
        /// <param name="data"></param>
        /// <param name="signature"></param>
        /// <returns></returns>
        public static bool VerifySignature(ECPublicKeyParameters publicKey, String data, byte[] signature)
        {
            ISigner signer = SignerUtilities.GetSigner("ECDSA");
            signer.Init(false, publicKey);
            var bytes = Encoding.UTF8.GetBytes(data);
            signer.BlockUpdate(bytes, 0, bytes.Length);
			return signer.VerifySignature(signature);
        }
    }
}
