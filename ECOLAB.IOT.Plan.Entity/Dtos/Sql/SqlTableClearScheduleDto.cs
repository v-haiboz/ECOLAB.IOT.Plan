namespace ECOLAB.IOT.Plan.Entity.Dtos.Sql
{
    using ECOLAB.IOT.Plan.Common.Utilities;
    using ECOLAB.IOT.Plan.Entity;
    using ECOLAB.IOT.Plan.Entity.Entities.SqlServer;
    using Newtonsoft.Json;

    public class SqlTableClearScheduleDto<Metrics>
    {
        public string? ServerName { get; set; }

        public string? DBName { get; set; }

        public PolicyDto<Metrics>? Policy { get; set; }

        public bool Enable { get; set; }

        public SqlTableClearSchedule ToCovertSqlTableClearSchedule()
        {
            var instance = new SqlTableClearSchedule()
            {
                JObject = JsonConvert.SerializeObject(this.Policy, Utility.jsonSettingIgnoreNullValue),
                Enable=true,
            };

            var partitionKey = string.Empty;

            var metricsTypeName = typeof(Metrics).Name;

            if (this.Policy != null)
            {
                if (metricsTypeName == "DateTimeMetricsDto")
                {
                    var obj = this.Policy as PolicyDto<DateTimeMetricsDto>;
                    if (obj != null)
                    {
                        partitionKey = obj.TableName;
                        instance.Type = GetClearScheduleTypeByTableName(obj.TableName, true);
                    }
                }
                else if (metricsTypeName == "IntMetricsDto")
                {
                    var obj = this.Policy as PolicyDto<IntMetricsDto>;
                    if (obj != null)
                    {
                        partitionKey = obj.TableName;
                        instance.Type = GetClearScheduleTypeByTableName(obj.TableName);
                    }
                }
                else
                {
                    throw new Exception($"There are no metrics of this type.");
                }
            }

            instance.PartitionKey = partitionKey;

            return instance;
        }

        private ClearScheduleType GetClearScheduleTypeByTableName(string tableName, bool isDateTimeMetrics=false)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new Exception($"The table name can't be empty.");
            }


            if (tableName == "*")
            {
                return isDateTimeMetrics?ClearScheduleType.DynamicDateTimeMetrics: ClearScheduleType.DynamicIntMetrics;
            }
            else if(tableName.Length>1 && tableName.IndexOf("*") == -1 )
            {
                return isDateTimeMetrics?ClearScheduleType.CustomDateTimeMetrics: ClearScheduleType.CustomIntMetrics;
            }
            else if (tableName.Length > 1 && tableName.IndexOf("*") != -1 && tableName.EndsWith("*") && tableName.Split(new string[] { "*" }, StringSplitOptions.RemoveEmptyEntries).Length == 1)
            {
                return isDateTimeMetrics?ClearScheduleType.PartialMatchDateTimMetrics: ClearScheduleType.PartialMatchIntMetrics;
            }
            else
            {
                throw new Exception($"The table name {tableName} format is incorrect.");
            }
        }
    }
}
