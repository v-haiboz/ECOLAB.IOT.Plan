namespace ECOLAB.IOT.Plan.Entity.ScheduleDtos.SqlServer
{
    using System;

    public class ClearPlan
    {
        public string TraceId { get; set; } = Guid.NewGuid().ToString();

        public ClearServer ClearServer { get; set; }

        public HashSet<ClearTable>? ClearTables { get; set; }

        public List<TableClearScheduleRecordDto> TableClearScheduleRecordDtos { get; set; } = new List<TableClearScheduleRecordDto>();

        public KeyValuePair<string, List<SqlPlan>> GetPlans(bool isParseOnly = false)
        {
            if (ClearServer == null || ClearTables == null)
            {
                return new KeyValuePair<string, List<SqlPlan>>();
            }

            var items =new List<SqlPlan>();

            foreach (var clearTable in ClearTables)
            {
                var commandSqls = clearTable.ToSql(isParseOnly);
                if(commandSqls!=null)
                items.AddRange(commandSqls);
            }

            return new KeyValuePair<string, List<SqlPlan>>(ClearServer.ConnectionStr, items);
        }

        public List<TableClearScheduleRecordDisplay> GetTableClearScheduleRecordDisplays()
        {
            var list=new List<TableClearScheduleRecordDisplay>();
            foreach (var item in TableClearScheduleRecordDtos)
            {
                list.Add(item.ToCovertTableClearScheduleRecordDisplay());
            }

            return list;
        }
    }

    public class ClearServer
    {
        private readonly string template = @"Server={ServerName},1433;Initial Catalog={DBName};Persist Security Info=False;User ID={UserId};Password={Password};Pooling=true;Min Pool Size=1;Max Pool Size=100;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=18000;";
       
        public string? ServerName { get; set; }

        public string? DBName { get; set; }

        public string? UserId { get; set; }

        public string? Password { get; set; }

        public string ConnectionStr
        {
            get {
                 return template.Replace("{ServerName}", ServerName)
                                  .Replace("{DBName}", DBName)
                                  .Replace("{UserId}", UserId)
                                  .Replace("{Password}", Password);
            }
        }
    }
}
