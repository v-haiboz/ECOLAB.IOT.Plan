namespace ECOLAB.IOT.Plan.Entity.ScheduleDtos.SqlServer
{
    public class TableClearScheduleRecordDto
    {
        public ClearTable? ClearTable { get; set; }
        public bool Enable { get; set; }

        public TableClearScheduleRecordDisplay ToCovertTableClearScheduleRecordDisplay()
        {
            return new TableClearScheduleRecordDisplay()
            {
                ServerName = ClearTable.ELinkSqlServer.ServerName,
                DBName = ClearTable.ELinkSqlServer.DBName,
                TableName = ClearTable.TableName,
                TableNamePattern = ClearTable.TableNamePattern,
                SourcePolicyMetrics = ClearTable.SourcePolicyMetrics,
                Enable = this.Enable ? "√" : "×",
            };
        }

    }

    public class TableClearScheduleRecordDisplay
    {
        public string? ServerName { get; set; }

        public string? DBName { get; set; }

        public string? TableName{ get; set; }

        public string? TableNamePattern { get; set; }

        public string? Enable { get; set; }

        public string? SourcePolicyMetrics { get; set; }

    }
}
