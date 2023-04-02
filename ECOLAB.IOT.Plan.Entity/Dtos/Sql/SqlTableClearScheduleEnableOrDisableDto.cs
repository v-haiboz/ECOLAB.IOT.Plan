namespace ECOLAB.IOT.Plan.Entity.Dtos.Sql
{
    using ECOLAB.IOT.Plan.Entity.Entities.SqlServer;

    public class SqlTableClearScheduleEnableOrDisableDto
    {
        public string? ServerName { get; set; }

        public string? DBName { get; set; }

        public string? TableName { get; set; }

        public bool Enable { get; set; }

        public SqlTableClearSchedule ToCovertSqlTableClearSchedule()
        {
            var instance = new SqlTableClearSchedule()
            {
                 PartitionKey= TableName,
                 Enable=Enable
            };

            return instance;
        }
    }
}
