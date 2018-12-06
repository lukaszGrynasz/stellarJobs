using Stellar.Jobs.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stellar.Jobs.Repositories
{
    public interface IUserRepository
    {
        User GetUserByDepositCode(string code);
    }
}
