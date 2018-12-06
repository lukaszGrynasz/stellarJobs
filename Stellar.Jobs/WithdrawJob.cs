using Quartz;
using Stellar.Jobs.Core;
using Stellar.Jobs.Model;
using Stellar.Jobs.Repositories;
using stellar_dotnet_sdk;
using System;
using System.Threading.Tasks;

namespace Stellar.Jobs
{
    public class WithdrawJob : IJob
    {
        private ITransactionRepository transactionRepository;
        private IUserRepository userRepository;
        private HorizonClient horizonClient;

        public WithdrawJob() { }

        //Ctor for tests 
        public WithdrawJob(
            ITransactionRepository transactionRepository,
            HorizonClient horizonClient,
            IUserRepository userRepository)
        {
            this.transactionRepository = transactionRepository;
            this.userRepository = userRepository;
            this.horizonClient = horizonClient;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                GetRepositories(context);

                // get all pending withdraw transactions
                var transactions = transactionRepository.GetPendingTransaction();

                foreach (var tx in transactions)
                {
                    try
                    {
                        tx.Status = Status.Pennding;
                        
                        var mainAccountKey = KeyPair.FromSecretSeed(Configurations.MAIN_ACCOUNT_SEED);

                        //withdraw lumens from main account to destination account 
                        await horizonClient.Transfer(mainAccountKey, tx.DestinationAddress, tx.Amount);

                        tx.Status = Status.Complited;

                        Console.WriteLine($"Withdraw of {tx.Amount} lumens to {tx.DestinationAddress} was completed.");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        tx.Status = Status.Error;
                    }
                }

            }
            catch (Exception ex)
            {
                //handle exception
                Console.WriteLine(ex);
            }
        }

        private void GetRepositories(IJobExecutionContext context)
        {
            //Get account repository instance
            if (transactionRepository == null)
            {
                transactionRepository =
                    context.Scheduler.Context[Constans.DATA_MAP_TRANS_REPO] as ITransactionRepository;
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
