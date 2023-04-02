namespace ECOLAB.IOT.Plan.Entity.ScheduleDtos.SqlServer
{
    using ECOLAB.IOT.Plan.Entity.SqlServer;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    public class ClearTable
    {
        private readonly static string template = "Delete {count} from {TableName} where {WhereStr}";

        public readonly static int noFrequencyBatchCount = 15000;

        public string TraceId { get; set; } = Guid.NewGuid().ToString();

        public string? TableName { get; set; }

        public Policy? Policy { get; set; }

        public WhereMetrics WhereMetrics { get; set; }

        public ClearTableType ClearTableType
        {
            get
            {
                if (Policy == null || Policy.Frequency == null || Policy.Count == null || Policy.Frequency <= 0 || Policy.Count <= 0)
                {
                    return ClearTableType.NOFREQUENCY;
                }
                else
                {
                    return ClearTableType.FREQUENCY;
                }
            }
        }

        public ClearScheduleType ClearScheduleType { get; set; }

        #region trace information
        [JsonIgnore]
        public string? TableNamePattern { get; set; }

        [JsonIgnore]
        public string? SourcePolicyMetrics { get; set; }

        [JsonIgnore]
        public ELinkSqlServer? ELinkSqlServer { get; set; }
        #endregion


        public List<SqlPlan>? ToSql(bool isParseOnly=false)
        {
            if (string.IsNullOrEmpty(TableName) || WhereMetrics==null || string.IsNullOrEmpty(WhereMetrics.Sql))
            {
                return null;
            }

            if (ClearTableType == ClearTableType.NOFREQUENCY)
            {
                var sql = template.Replace("{count}", $"top({noFrequencyBatchCount})")
                                          .Replace("{TableName}", TableName)
                                          .Replace("{WhereStr}", WhereMetrics.Sql);
                return new List<SqlPlan> { new SqlPlan() { ClearTableType = ClearTableType.NOFREQUENCY, Sql = sql, TableName= TableName } };
            }


            if (ClearTableType == ClearTableType.FREQUENCY)
            {

                var batchSql = new List<SqlPlan>();

                for (var i = 0; i < Policy.Frequency; i++)
                {
                    var sql = template.Replace("{count}", $"top({Policy.Count})")
                                      .Replace("{TableName}", TableName)
                                      .Replace("{WhereStr}", WhereMetrics.Sql);

                    batchSql.Add(new SqlPlan()
                    {
                        ClearTableType = ClearTableType.FREQUENCY,
                        Sql = sql,
                        TableName = TableName
                    });

                    if (isParseOnly)
                    {
                        break;
                    }
                }
                
                return batchSql;
            }
           
            return new List<SqlPlan>();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class Policy
    {
        public int? Frequency { get; set; } = 0;

        public int? Count { get; set; } = 0;
    }

    public class WhereMetrics
    {
        public string? ColumnName { get; set; }

        public string? Operator { get; set; }

        public object? Value { get; set; }

        public string? Sql {get;set;}

    }

    public class SqlPlan
    {
        public ClearTableType ClearTableType { get; set; }

        public string? Sql { get; set; }

        public string? TableName { get; set; }
    }

    public enum ClearTableType
    {
        PARSEONLY = 0,
        FREQUENCY = 1,
        NOFREQUENCY = 2,
    }

    public class ClearTableComparer : IEqualityComparer<ClearTable>
    {
        public bool Equals([AllowNull] ClearTable x, [AllowNull] ClearTable y)
        {
            return x.TableName == y.TableName;
        }

        public int GetHashCode([DisallowNull] ClearTable obj)
        {
            return obj.TableName.GetHashCode();
        }
    }
}
