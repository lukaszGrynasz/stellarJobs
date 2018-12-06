using Stellar.Jobs.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stellar.Jobs.Repositories
{
    public interface ITransactionRepository
    {
        List<StellarTransaction> GetPendingTransaction();
        void AddWithdrawTransaction(Guid UserId, string DestinationAddress, decimal Amount);
    }
}
