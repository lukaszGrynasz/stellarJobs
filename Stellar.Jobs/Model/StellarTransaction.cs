using System;
using System.Collections.Generic;
using System.Text;

namespace Stellar.Jobs.Model
{
    public class StellarTransaction
    {
        public Guid UserId { get; set; }
        public string DestinationAddress { get; set; }
        public Status Status { get; set; }
        public decimal Amount { get; set; }
    }

    public enum Status
    {
        Pennding,
        Running, 
        Complited,
        Error
    }
}
