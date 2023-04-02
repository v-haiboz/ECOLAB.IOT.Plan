namespace ECOLAB.IOT.Plan.Entity.Dtos.Sql
{
    using ECOLAB.IOT.Plan.Entity.Entities.SqlServer;

    public class SqlTableClearScheduleDeleteDto
    {
        public string? ServerName { get; set; }

        public string? DBName { get; set; }

        public string? TableName { get; set; }

        public SqlTableClearSchedule ToCovertSqlTableClearSchedule()
        {
            var instance = new SqlTableClearSchedule()
            {
               PartitionKey = TableName
            };

            return instance;
        }
    }
}
