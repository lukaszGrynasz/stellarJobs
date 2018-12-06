using Quartz;
using Quartz.Impl;
using Stellar.Jobs.Core;
using Stellar.Jobs.Repositories;
using stellar_dotnet_sdk;
using System;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;

namespace Stellar.Jobs
{
    class Program
    {
        static void Main(string[] args)
        {
            Network.UseTestNetwork();
            RunProgram().GetAwaiter().GetResult();
        }

        private static async Task RunProgram()
        {
            try
            {
                // Grab the Scheduler instance from the Factory
                NameValueCollection props = new NameValueCollection
                {
                    { "quartz.serializer.type", "binary" }
                };
                StdSchedulerFactory factory = new StdSchedulerFactory(props);
                IScheduler scheduler = await factory.GetScheduler();

                // and start it off
                await scheduler.Start();

                //injects services
                scheduler.Context.Put(Constans.DATA_MAP_ACCOUNT_REPO, new StaticAccountRepository());
                scheduler.Context.Put(Constans.DATA_MAP_USER_REPO, new StaticUserRepository());
                scheduler.Context.Put(Constans.DATA_MAP_TRANS_REPO, new StaticTransactionRepository());
                scheduler.Context.Put(Constans.DATA_MAP_HORIZON_CLIENT, new HorizonClient());


                //create jobs
                await CreateJob<DepositTrackerJob>(scheduler, "Job1", 40, 0);
                await CreateJob<WithdrawJob>(scheduler, "Job2", 60, 0);

                Console.WriteLine("Press any key to close the application");
                Console.ReadKey();

                // and last shut down the scheduler when you are ready to close your program
                await scheduler.Shutdown();
            }
            catch (SchedulerException se)
            {
                await Console.Error.WriteLineAsync(se.ToString());
            }
        }

        private async static Task CreateJob<T>(IScheduler scheduler, string jobName, int seconds, int delay) where T:IJob
        {
            IJobDetail job = JobBuilder.Create<T>()
                .WithIdentity(jobName, "group1")
                .Build();

            // Trigger the job to run now, and then repeat every 10 seconds
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(jobName+"_trigger", "group1")
                .StartAt(DateTime.Now.AddSeconds(delay))
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(seconds)
                    .RepeatForever())
                .Build();

            // Tell quartz to schedule the job using our trigger
            await scheduler.ScheduleJob(job, trigger);
        }
    }
}
