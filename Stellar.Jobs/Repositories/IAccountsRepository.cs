using Stellar.Jobs.Model;
using System.Collections.Generic;

namespace Stellar.Jobs.Repositories
{
    public interface IAccountsRepository
    {
        List<StellarAccount> GetAccounts();
       
    }
}