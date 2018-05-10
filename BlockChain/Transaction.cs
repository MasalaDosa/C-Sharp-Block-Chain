using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockChain
{

    public class Transaction
    {
        static int _sequence = 0;
        public string Transactionid { get; private set; }

        public ECPublicKeyParameters Sender { get; private set; }

        public ECPublicKeyParameters Recipient { get; private set; }

        public Decimal Value { get; set; }

        public byte[] Signature { get; set; }

        public List<TransactionInput> Inputs = new List<TransactionInput>();

        public List<TransactionOutput> Outputs = new List<TransactionOutput>();

        public Transaction(ECPublicKeyParameters sender, ECPublicKeyParameters recipient, Decimal value, List<TransactionInput> inputs)
        {
            Sender = sender;
            Recipient = recipient;
            Value = value;
            Inputs = inputs;
        }

		string CalculateHash()
        {
            _sequence++; // Avoid to otherwise identical transactions having the same hash.
            StringBuilder toBeHashed = new StringBuilder();
            toBeHashed.Append(Utils.BytesToUTF8(Sender.Q.GetEncoded()));
            toBeHashed.Append(Utils.BytesToUTF8(Recipient.Q.GetEncoded()));
            toBeHashed.Append(Value.ToString());
            toBeHashed.Append(_sequence);

            return Utils.BytesToUTF8(Utils.Hash(toBeHashed.ToString(), string.Empty));
        }

        /// <summary>
        /// Signs the data we don't want to be tampered with.
        /// </summary>
        /// <param name="privateKey"></param>
        public void CaclulateSignature(ECPrivateKeyParameters privateKey)
		{
			Signature = Utils.SignData(privateKey, DataToSign());
		}

        /// <summary>
        /// Verifies the signature or dagta has not been tampered with
        /// </summary>
        /// <returns><c>true</c>, if signature was verifyed, <c>false</c> otherwise.</returns>
		public bool VerifySignature()
        {    
            return Utils.VerifySignature(this.Sender, DataToSign(), Signature);
        }

        /// <summary>
        /// Returns a concatination of the data we want to be protected from tampering
        /// </summary>
        /// <returns>The to sign.</returns>
	    string DataToSign()
		{
			// In reality may want to sign more infor - like inputs/outputs/timestamp - for now just the bare minimum
			StringBuilder toBeSigned = new StringBuilder();
			toBeSigned.Append(Utils.BytesToUTF8(Sender.Q.GetEncoded()));
			toBeSigned.Append(Utils.BytesToUTF8(Recipient.Q.GetEncoded()));
			toBeSigned.Append(Value.ToString());
			return toBeSigned.ToString();;
		}
    }

    public class TransactionInput
    {
        public string TransactionOutputId { get; set; } // Reference to TransactionOutputs => TransactionId
        public TransactionOutput Utxo { get; set; } // Unspent transaction output

        public TransactionInput(string transactionOutputId)
        {
            TransactionOutputId = transactionOutputId;
        }
    }

    public class TransactionOutput
    {
        public string Id { get; set; }
        public ECPublicKeyParameters Recipient { get; set; } // The new owner of these coins
        public Decimal Value { get; set; } // The amount of coins they own.
        public string ParentTransactionId { get; set; } // The ID of the transaction this output was created in

        public TransactionOutput(ECPublicKeyParameters recipient, Decimal value, string parentTransactionId)
        {
            Recipient = recipient;
            Value = value;
            ParentTransactionId = parentTransactionId;

            StringBuilder toBeHashed = new StringBuilder();
            toBeHashed.Append(Utils.BytesToUTF8(Recipient.Q.GetEncoded()));
            toBeHashed.Append(Value.ToString());
            toBeHashed.Append(ParentTransactionId);
            Id = Utils.BytesToUTF8(Utils.Hash(toBeHashed.ToString(), string.Empty));
        }

        public bool IsMine(ECPublicKeyParameters publicKey)
        {
            return publicKey == Recipient;
        }
    }

}