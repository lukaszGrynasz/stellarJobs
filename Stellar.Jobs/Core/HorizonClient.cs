using Stellar.Jobs.Model;
using stellar_dotnet_sdk;
using stellar_dotnet_sdk.responses.operations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Stellar.Jobs.Core
{
    public class HorizonClient : IDisposable
    {
        private Server server = null;

        public HorizonClient()
        {
            server = new Server(Configurations.TESTNET_URL);
        }

        public async Task<List<PaymentTransaction>> GetLastDepositTransactions(StellarAccount account)
        {
            try
            {
                var builder = server.Payments;
                if (!string.IsNullOrWhiteSpace(account.LastCursor))
                {
                    builder.Cursor(account.LastCursor);
                }

                // get all payments operation form given account
                var result = await builder.ForAccount(KeyPair.FromAccountId(account.Address)).Execute();
                var payments = new List<PaymentTransaction>();

                var txHashList = new List<string>();

                foreach (var payment in result.Records)
                {
                    var pay = payment as PaymentOperationResponse;

                    //extracts only a native payments 
                    if (pay != null && pay.AssetType == "native" && pay.To.AccountId == account.Address)
                    {
                        payments.Add(new PaymentTransaction()
                        {
                            Amount = Decimal.Parse(pay.Amount, CultureInfo.InvariantCulture),
                            From = pay.From,
                            To = pay.To,
                            Hash = pay.TransactionHash,
                            PagingToken = pay.PagingToken
                        });

                    }
                }

                foreach (var payment in payments)
                {
                    //get payment transaction to read memo field
                    var transaction = await server.Transactions.Transaction(payment.Hash);
                    payment.Memo = transaction.MemoStr;
                }

                return payments;

            }
            catch (Exception ex)
            {
                //handle exception
                Console.WriteLine(ex);
                return null;
            }
        }

        public async Task Transfer(KeyPair source, List<KeyPair> signers,  string destinationAddress, decimal amount)
        {
            //get source account - check existance
            var mainAccount = await server.Accounts.Account(source);

            //create stellar transaction
            var tx = new Transaction.Builder(mainAccount).AddOperation(
               new PaymentOperation.Builder(KeyPair.FromAccountId(destinationAddress), new AssetTypeNative(), amount.ToString()).Build())
               .Build();

            foreach(var signer in signers)
                tx.Sign(signer);

            var response = await server.SubmitTransaction(tx);

            if (response.SubmitTransactionResponseExtras != null &&
                response.SubmitTransactionResponseExtras.ExtrasResultCodes.TransactionResultCode == "tx_failed")
            {
                throw new Exception("Transaction failed : " + response.SubmitTransactionResponseExtras.ExtrasResultCodes?.TransactionResultCode);
            }
                
        }

        public void Dispose()
        {
            server?.Dispose();
        }

    }
}
