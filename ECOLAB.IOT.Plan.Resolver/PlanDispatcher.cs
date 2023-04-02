namespace ECOLAB.IOT.Plan.Resolver
{
    using ECOLAB.IOT.Plan.Entity.ScheduleDtos.SqlServer;
    using System;
    using System.Diagnostics;

    public interface IPlanDispatcher
    {
        public void Execute(List<ClearPlan> clearPlans);
    }

    public class PlanDispatcher : IPlanDispatcher
    {
        public void Execute(List<ClearPlan> clearPlans)
        {
            var stopWatch = Stopwatch.StartNew();
            var options = new ParallelOptions() { MaxDegreeOfParallelism = 8 };
            Parallel.ForEach(clearPlans, options, clearPlan =>
            {
                var plans = clearPlan.GetPlans();
                var connectionStr = plans.Key;
                foreach (var plan in plans.Value)
                {
                    //CallerContext.SqlServerRepository.Delete(connectionStr, plan);
                }
            });

            Console.WriteLine("Parallel.ForEach() execution time = {0} seconds", stopWatch.Elapsed.TotalSeconds);
        }
    }
}
