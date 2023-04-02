namespace ECOLAB.IOT.Plan.Entity.Dtos.Sql
{
    using System;

    public class IntMetricsDto
    {

        //"columnName": "Id",
        //"operator": "equalto or lessthan",
        //"value": 36

        public string? ColumnName { get; set; }

        public string? Operator { get; set; }

        public int? Value { get; set; }
    }
}
