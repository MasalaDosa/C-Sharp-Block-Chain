using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockChain
{

    /// <summary>
    /// A transaction has a Sender, Recipient and a Value.
	/// It is signed by the Sender which serves two purposes.
	/// a) only the owner is allowed to spend their coins, 
	/// b) prevent others from tampering with their submitted transaction before a new block is mined 
    /// </summary>
    public class Transaction
    {
        static int _sequence = 0;
        public string TransactionId { get;  set; }
        
        public ECPublicKeyParameters Sender { get; set; }

        public ECPublicKeyParameters Recipient { get; set; }

        public decimal Value { get; set; }

        public byte[] Signature { get; set; }

        public List<TransactionInput> Inputs = new List<TransactionInput>();

        public List<TransactionOutput> Outputs = new List<TransactionOutput>();

        public Transaction(ECPublicKeyParameters sender, ECPublicKeyParameters recipient, decimal value, List<TransactionInput> inputs)
        {
            Sender = sender;
            Recipient = recipient;
            Value = value;
            Inputs = inputs;
        }

		string CalculateHash()
        {
            _sequence++; // Avoid otherwise identical transactions having the same hash.
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
        public void Sign(ECPrivateKeyParameters privateKey)
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
       
        /// <summary>
        /// Processes the transaction.  Returns true if new transaction could be created.
        /// </summary>
        /// <returns><c>true</c>, if transaction was processed, <c>false</c> otherwise.</returns>
        public bool ProcessTransaction(Chain chain) 
    	{
            if(!VerifySignature()) 
			{
                Console.WriteLine("Transaction Signature failed to verify");
                return false;
            }
                    
            // Gather transaction inputs (Make sure they are unspent):
            foreach (var ti in Inputs) 
			{
                ti.Utxo = chain.Utxos[ti.TransactionOutputId];
            }
            
            // Check if transaction is valid:
            if(SumInputValues() < Chain.MINIMUM_TRANSACTION) 
			{
				Console.WriteLine("Transaction Inputs to small: {0}",SumInputValues());
                return false;
            }
            
            // Generate transaction outputs:
            decimal leftOver = SumInputValues() - Value; //get value of inputs then the left over change:
            TransactionId = CalculateHash();
            Outputs.Add(new TransactionOutput( this.Recipient, Value, TransactionId)); //send value to recipient
            Outputs.Add(new TransactionOutput( this.Sender, leftOver, TransactionId)); //send the left over 'change' back to sender      
                    
            // Add outputs to Unspent list
            foreach (TransactionOutput to in Outputs) 
			{
                chain.Utxos[to.Id] = to;
            }
            
            // Remove transaction inputs from UTXO lists as spent:
            foreach (TransactionInput ti in Inputs) 
			{
				if (ti.Utxo == null)
				{
					continue; //if Transaction couldn't be found skip it 
				}
                chain.Utxos.Remove(ti.Utxo.Id);
            }
            
            return true;
        }
    
		public decimal SumInputValues() 
		{
			return Inputs == null ? 
				0M : 
				Inputs.Where(ti => ti.Utxo != null).Select(ti => ti.Utxo.Value).Sum();
        }
        
        public decimal SumOutputValues()
		{
			return Outputs == null ?
				0M :
				Outputs.Select(to => to.Value).Sum();
        }

        public override string ToString()
        {
            return string.Format("{0} from: {1} To: {2}",
                                 Value,
                                 string.Concat(Utils.BytesToUTF8(Sender.Q.GetEncoded()).Substring(0,5), "..."),
                                 string.Concat(Utils.BytesToUTF8(Recipient.Q.GetEncoded()).Substring(0,5), "..."));
        }
    }

    /// <summary>
	/// Transaction inputs are references to previous transactions
	/// They prove the sender has funds to send.
    /// </summary>
    public class TransactionInput
    {
        // Reference to TransactionOutputs TransactionId
        public string TransactionOutputId { get; set; } 

        // Unspent transaction output
        public TransactionOutput Utxo { get; set; } 

        public TransactionInput(string transactionOutputId)
        {
            TransactionOutputId = transactionOutputId;
        }
    }

    public class TransactionOutput
    {
        public string Id { get; set; }
        // The new owner of these coins
        public ECPublicKeyParameters Recipient { get; set; } 
        // The amount of coins they own.
        public decimal Value { get; set; } 
        // The ID of the transaction this output was created in
        public string ParentTransactionId { get; set; } 

        public TransactionOutput(ECPublicKeyParameters recipient, decimal value, string parentTransactionId)
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