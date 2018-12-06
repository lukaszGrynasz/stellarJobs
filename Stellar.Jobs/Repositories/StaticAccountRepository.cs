using Stellar.Jobs.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stellar.Jobs.Repositories
{
    public class StaticAccountRepository : IAccountsRepository
    {
        private List<StellarAccount> accounts = new List<StellarAccount>()
        {
            new StellarAccount(1, "GACSZR2FKLCCREMLEC6K75Q25ZIFERHL6Z7HKADUHRURAZW3KLY22UDV"),
            //new StellarAccount(2, "GASQSGPRQRUHCFTDCNOSYR2JO3YPVGKVBJYW5PDAC4R4QRCFIIHTBBHA"),
        };

        public List<StellarAccount> GetAccounts()
        {
            return accounts;
        }
      
    }
}
