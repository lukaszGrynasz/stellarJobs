using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stellar.Jobs.Model;

namespace Stellar.Jobs.Repositories
{
    public class StaticUserRepository : IUserRepository
    {
        private List<User> users = new List<User>()
        {
            new User(Guid.NewGuid(), "3141592653"),
            new User(Guid.NewGuid(), "1123581321"),
            new User(Guid.NewGuid(), "1617647058"),
        };

        public User GetUserByDepositCode(string code)
        {
            return users.Where(x => x.DepositCode == code).SingleOrDefault();
        }
    }
}
