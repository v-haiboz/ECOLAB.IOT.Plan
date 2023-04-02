namespace ECOLAB.IOT.Plan.Service.Sql
{
    using ECOLAB.IOT.Plan.Entity;
    using ECOLAB.IOT.Plan.Entity.ScheduleDtos.SqlServer;
    using ECOLAB.IOT.Plan.Repository.Repositories.SqlServer;
    using System;

    public interface IPlanService
    {
        public bool IsExistTable(string pTableName, string pDB_NAME, string connectionStr);

        public bool IsExistColumn(string pTableName, string columnName, string connectionStr, string columnTypeName = "datetime2");

        public TData ValidateSQL(SqlPlan sqlPlan, string connectionStr);

        public TData ExecutePlan(SqlPlan sqlPlan, string connectionStr, DateTime expiryTime = default(DateTime));
    }

    public class PlanService : IPlanService
    {
        private readonly IPlanRepository _planRepository;
        public PlanService(IPlanRepository planRepository)
        {
            _planRepository = planRepository;
        }

        public bool IsExistTable(string pTableName, string pDB_NAME, string connectionStr)
        {
            return _planRepository.IsExistTable(pTableName, pDB_NAME, connectionStr);
        }

        public bool IsExistColumn(string pTableName, string columnName, string connectionStr, string columnTypeName = "datetime2")
        {
            return _planRepository.IsExistColumn(pTableName, columnName, connectionStr, columnTypeName);
        }

        public TData ExecutePlan(SqlPlan sqlPlan, string connectionStr, DateTime expiryTime = default(DateTime))
        {
            var result = new TData();
            var totalRows = 0;
            try
            {
                var rows = 0;
                if (sqlPlan.ClearTableType == ClearTableType.NOFREQUENCY)
                {
                    var batchRows = ClearTable.noFrequencyBatchCount;
                    do
                    {
                        rows = 0;
                        rows = ExecutePlan(sqlPlan.Sql, connectionStr, expiryTime);
                        totalRows += rows;
                        Thread.Sleep(1000);

                    } while (rows ==batchRows);
                }
                else if(sqlPlan.ClearTableType == ClearTableType.FREQUENCY)
                {
                    rows = ExecutePlan(sqlPlan.Sql, connectionStr, expiryTime);
                    totalRows += rows;
                }

                result.Tag = rows != -1 ? 1 : 0;
                result.Message = $"ExecutePlan successful";
                result.Description = $"Total Rows:{totalRows} Sql:{sqlPlan.Sql}";
                return result;
            }
            catch (Exception ex)
            {
                result.Tag = 0;
                result.Message = "ExecutePlan failed";
                result.Description = $"Total Rows:{totalRows} Sql:{sqlPlan.Sql} {ex.Message}";
                return result;
            }
        }

        private int ExecutePlan(string sql, string connectionStr, DateTime expiryTime = default(DateTime))
        {
            return _planRepository.ExecutePlan(sql, connectionStr, expiryTime);
        }

        public TData ValidateSQL(SqlPlan sqlPlan, string connectionStr)
        {
            var result = new TData();
            try
            {
                var rows = _planRepository.ExecutePlan(sqlPlan.Sql, connectionStr);
                result.Tag = rows!=-1 ? 1 : 0;
                result.Message = $"ValidateSQL successful";
                result.Description = $"Sql:{sqlPlan.Sql}";
                return result;
            }
            catch (Exception ex)
            {
                result.Tag = 0;
                result.Message = "ValidateSQL failed";
                result.Description = $"Sql:{sqlPlan.Sql}";
                return result;
            }
        }

    }
}
