namespace ECOLAB.IOT.Plan.Provider.Sql
{
    using ECOLAB.IOT.Plan.Entity.Dtos.Sql;
    using ECOLAB.IOT.Plan.Entity.ScheduleDtos.SqlServer;
    using System;
    using System.Globalization;
    using System.Linq;

    public class DateTimeMetricsProvider : IMetricsProvider<WhereMetrics, DateTimeMetricsDto>
    {
        private string schema = @"
            ""datetime"": {
				""columnName"": ""CreateTime"",
				""operator"": ""=<"",
				""holdDays"": 60
			}";

        private readonly string[] operators = new string[] { "=", "<=" };

        private string metricsName = "datetime";

        public WhereMetrics Resolve(DateTimeMetricsDto? dateTimeMetricsDto)
        {
            return Travel(dateTimeMetricsDto);
        }

        private WhereMetrics Travel(DateTimeMetricsDto? dateTimeMetricsDto)
        {
            if (dateTimeMetricsDto == null)
            {
                throw new Exception("DateTime Metrics is null.");
            }

            if (string.IsNullOrEmpty(dateTimeMetricsDto.ColumnName)
                || string.IsNullOrEmpty(dateTimeMetricsDto.Operator)
                || dateTimeMetricsDto.HoldDays == null)
            {
                throw new Exception("Keys of dateTime Metrics are wrong.");
            }

            if (!operators.Contains(dateTimeMetricsDto.Operator))
            {
                throw new Exception(@$"{dateTimeMetricsDto.Operator} operator is not supported. Only '{operators[0]}' and '{operators[1]}' are supported");
            }

            if (dateTimeMetricsDto.HoldDays.Value <90)
            {
                throw new Exception("holdDays must be greater than 90 days");
            }

            var holdTimeDays = -1 * dateTimeMetricsDto.HoldDays.Value;
            var actualTime = DateTime.UtcNow.AddDays(holdTimeDays);

            var whereStr = $"CONVERT(varchar,{dateTimeMetricsDto.ColumnName},23)<=CONVERT(varchar,'{actualTime}',23)";
            if (operators[0] == dateTimeMetricsDto.Operator)
            {
                whereStr = $"DATENAME(year,{dateTimeMetricsDto.ColumnName})<DATENAME(year,'{actualTime}') and DATENAME(MONTH,{dateTimeMetricsDto.ColumnName})=DATENAME(MONTH,'{actualTime}') and DATENAME(DAY,{dateTimeMetricsDto.ColumnName})=DATENAME(DAY,'{actualTime}')";
            }
            else
            {
                whereStr = $"CONVERT(varchar,{dateTimeMetricsDto.ColumnName},23)<=CONVERT(varchar,'{actualTime.ToString("yyyy-MM-dd HH:mm:ss")}',23)";
            }

            return new WhereMetrics()
            {
                ColumnName = dateTimeMetricsDto.ColumnName,
                Operator = dateTimeMetricsDto.Operator,
                Value = dateTimeMetricsDto.HoldDays,
                Sql = whereStr,
            };
        }
    }
}
