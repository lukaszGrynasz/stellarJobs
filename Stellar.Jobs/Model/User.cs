using System;

namespace Stellar.Jobs.Model
{
    public class User
    {
        public User(){}
        //For tests
        public User(Guid id, string depositCode)
        {
            Id = id;
            DepositCode = depositCode;
        }

        public Guid Id { get; set; }
        //To simplify deposit code is in user data
        public string DepositCode { get; set; }

        public decimal Deposit { get; set; }
    }
}
