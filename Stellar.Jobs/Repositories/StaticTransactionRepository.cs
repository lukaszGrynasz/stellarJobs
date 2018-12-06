using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stellar.Jobs.Model;

namespace Stellar.Jobs.Repositories
{
    public class StaticTransactionRepository : ITransactionRepository
    {
        List<StellarTransaction> transaction = new List<StellarTransaction>();

        public StaticTransactionRepository()
        {
            AddWithdrawTransaction(Guid.NewGuid(), "GACSZR2FKLCCREMLEC6K75Q25ZIFERHL6Z7HKADUHRURAZW3KLY22UDV", 1);
        }

        public void AddWithdrawTransaction(Guid userId, string destinationAddress, decimal amount)
        {
            transaction.Add(new StellarTransaction()
            {
                Amount = amount,
                UserId = userId,
                Status = Status.Pennding,
                DestinationAddress = destinationAddress
            });
        }

        public List<StellarTransaction> GetPendingTransaction()
        {
            return transaction.Where(x => x.Status == Status.Pennding).ToList();
        }
    }
}
