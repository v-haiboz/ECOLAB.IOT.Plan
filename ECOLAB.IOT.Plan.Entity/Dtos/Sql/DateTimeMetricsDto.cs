namespace ECOLAB.IOT.Plan.Entity.Dtos.Sql
{
    using System;


    public class DateTimeMetricsDto
    {
                //"columnName": "CreateTime",
				//"operator": "equalto or lessthan",
				//"holdTime": 36
        public string? ColumnName { get; set; }

        public string? Operator { get; set; }

        public int? HoldDays { get;set; }
           
    }
}
