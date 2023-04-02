namespace ECOLAB.IOT.Plan.Provider.Sql
{
    using ECOLAB.IOT.Plan.Entity.Dtos.Sql;
    using ECOLAB.IOT.Plan.Entity.ScheduleDtos.SqlServer;
    using Newtonsoft.Json.Linq;
    using System;

    public class IntMetricsProvider : IMetricsProvider<WhereMetrics, IntMetricsDto>
    {
        private string schema = @"
            ""int"": {
				""ahead"": ""Id"",
				""condition"": ""<="",
				""behind"": 36
			}";

        private readonly string[] operators = new string[] { "<=" };

        public WhereMetrics Resolve(IntMetricsDto? intMetricsDto)
        {
            return Travel(intMetricsDto);
        }
        private WhereMetrics Travel(IntMetricsDto? intMetricsDto)
        {
            if (intMetricsDto == null)
            {
                throw new Exception("Int Metrics is null.");
            }

            if (string.IsNullOrEmpty(intMetricsDto.ColumnName)
                 || string.IsNullOrEmpty(intMetricsDto.Operator)
                 || intMetricsDto.Value == null)
            {
                throw new Exception("Keys of int Metrics are wrong");
            }

            if (!operators.Contains(intMetricsDto.Operator))
            {
                throw new Exception($"{intMetricsDto.Operator} operator is not supported.only {operators[0]} is supported");
            }

            return new WhereMetrics()
            {
                ColumnName = intMetricsDto.ColumnName,
                Operator = intMetricsDto.Operator,
                Value = intMetricsDto.Value,
                Sql = $"{intMetricsDto.ColumnName} {intMetricsDto.Operator} {intMetricsDto.Value}"
            };
        }
    }
}
