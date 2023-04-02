namespace ECOLAB.IOT.Plan.Resolver.Sql
{
    using ECOLAB.IOT.Plan.Entity.ScheduleDtos.SqlServer;
    using Microsoft.VisualBasic.FileIO;
    using Newtonsoft.Json.Linq;

    public class CustomMetrics : IMetrics<ClearTable>
    {
        public ClearTable Resolve(string json)
        {
            var obj = JObject.Parse(json);
            return Travel(obj);
        }

        private ClearTable Travel(JObject obj)
        {
            if (obj == null)
            {
                return null;
            }

            if (obj["tableName"] == null || obj["whereStr"] == null)
            {
                return null;
            }

            var clearTable = new ClearTable()
            {
                TableName = (string)obj["tableName"],
                WhereStr = (string)obj["whereStr"]
            };

            if (obj["policy"] != null)
            {
                var policy = (JObject)obj["policy"];
                if (policy["frequency"] != null && policy["count"] != null)
                {
                    var frequency = (int)policy["frequency"];
                    var count = (int)policy["count"];

                    if (frequency > 0 && count > 0)
                    {
                        clearTable.Policy = new Policy()
                        {
                            Frequency = frequency,
                            Count = count
                        };
                    }
                }
            }

            return clearTable;
        }
    }
}
