namespace ECOLAB.IOT.Plan.Resolver
{
    using ECOLAB.IOT.Plan.Entity;
    using ECOLAB.IOT.Plan.Entity.ScheduleDtos.SqlServer;
    using ECOLAB.IOT.Plan.Entity.SqlServer;
    using System.Collections.Generic;

    public interface IPlanParser
    {
        public List<ClearPlan> Execute();
    }
    public class PlanParser : IPlanParser
    {
        public List<ClearPlan> Execute()
        {
            var list = new List<ELINKSqlServer>(); //CallerContext.ELINKSqlServerRepository.GetPlanList();
            var clearPlans = new List<ClearPlan>();
            foreach (var item in list)
            {
                if (item != null)
                {
                    var clearPlan = BuildClearPlan(item);
                    clearPlans.Add(clearPlan);
                }
            }

            return clearPlans;
        }

        private ClearPlan BuildClearPlan(ELINKSqlServer eLINKSql)
        {
            var clearPlan = new ClearPlan();
            clearPlan.ClearServer = new ClearServer()
            {
                DBName = eLINKSql.DBName,
                Password = eLINKSql.Password,
                ServerName = eLINKSql.ServerName,
                UserId = eLINKSql.UserId
            };
            clearPlan.ClearTable = new List<ClearTable>();

            foreach (var item in eLINKSql.SqlTableClearSchedules)
            {
                if (item.Type == ClearScheduleType.Dynamic)
                {
                    //var clearTable = CallerContext.DynamicMetrics.Resolve(item.JObject);
                    //clearPlan.ClearTable.Add(clearTable);
                }
                else if (item.Type == ClearScheduleType.Custom)
                {
                    //var clearTable = CallerContext.DynamicMetrics.Resolve(item.JObject);
                    //clearPlan.ClearTable.Add(clearTable);
                }
            }

            return clearPlan;
        }
    }
}
