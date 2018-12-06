using Quartz;
using Stellar.Jobs.Core;
using Stellar.Jobs.Model;
using Stellar.Jobs.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Stellar.Jobs
{
    public class DepositTrackerJob : IJob
    {
        private IAccountsRepository accountRepository;
        private IUserRepository userRepository;
        private HorizonClient horizonClient;

        public DepositTrackerJob() { }

        //Ctor for tests 
        public DepositTrackerJob(
            IAccountsRepository accountRepository,
            HorizonClient horizonClient,
            IUserRepository userRepository)
        {
            this.accountRepository = accountRepository;
            this.userRepository = userRepository;
            this.horizonClient = horizonClient;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                GetRepositories(context);

                var accounts = accountRepository.GetAccounts();

                foreach (var account in accounts)
                {
                    await CheckDeposits(account);
                }
            }
            catch (Exception ex)
            {
                //handle exception
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// Check last deposits for all account and match deposit with internal user
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        private async Task CheckDeposits(StellarAccount account)
        {
            //get last deposits from given account
            var lastDeposits = await horizonClient.GetLastDepositTransactions(account);

            foreach (var deposits in lastDeposits)
            {
                //match deposits with user
                var user = userRepository.GetUserByDepositCode(deposits.Memo);
                if (user != null)
                {
                    //increment user deposit
                    user.Deposit += deposits.Amount;
                    Console.WriteLine($"User ID {user.Id} deposited {deposits.Amount} in transaction {deposits.Hash}");
                }
            }

            if (lastDeposits.Count > 0)
            {
                account.LastCursor = lastDeposits.Last().PagingToken;
            }
        }

        /// <summary>
        /// If services was not injected by constructor GetRepositories method will extracts them form job context 
        /// </summary>
        /// <param name="context"></param>
        private void GetRepositories(IJobExecutionContext context)
        {
            //Get account repository instance
            if (accountRepository == null)
            {
                accountRepository =
                    context.Scheduler.Context[Constans.DATA_MAP_ACCOUNT_REPO] as IAccountsRepository;
            }

            if (userRepository == null)
            {
                userRepository =
                    context.Scheduler.Context[Constans.DATA_MAP_USER_REPO] as IUserRepository;
            }

            if (horizonClient == null)
            {
                horizonClient =
                    context.Scheduler.Context[Constans.DATA_MAP_HORIZON_CLIENT] as HorizonClient;
            }
        }
    }
}
