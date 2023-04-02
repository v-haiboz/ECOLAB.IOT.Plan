namespace ECOLAB.IOT.Plan.Entity.Dtos.Sql
{
    using ECOLAB.IOT.Plan.Entity.Entities.SqlServer;

    public class DynamicSqlTableClearScheduleDeleteDto
    {
        public string? ServerName { get; set; }

        public string? DBName { get; set; }

        public ClearScheduleType? ClearScheduleType { get; set; }

        public SqlTableClearSchedule ToCovertSqlTableClearSchedule()
        {
            var instance = new SqlTableClearSchedule()
            {
                Type = ClearScheduleType,
            };

            return instance;
        }
    }
}
