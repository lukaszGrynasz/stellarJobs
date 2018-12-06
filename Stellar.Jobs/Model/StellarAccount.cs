using System;
using System.Collections.Generic;
using System.Text;

namespace Stellar.Jobs.Model
{
    public class StellarAccount
    {
        public StellarAccount(int id, string address, string lastCursor = null)
        {
            Id = id;
            Address = address;
            LastCursor = lastCursor;
        }

        public int Id { get; set; }
        public string Address { get; set; }
        public string LastCursor { get; set; }
    }
}
