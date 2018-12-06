using stellar_dotnet_sdk;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Stellar.Jobs.Model
{
    public class PaymentTransaction
    {
        public string Hash { get; set; }
        public string Memo { get; set; }
        public decimal Amount { get; set; }
        public KeyPair From { get; set; }
        public KeyPair To { get; set; }
        public string PagingToken { get; set; }
    }
}
