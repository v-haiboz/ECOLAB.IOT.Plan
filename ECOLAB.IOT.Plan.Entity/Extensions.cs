namespace ECOLAB.IOT.Plan.Entity
{
    using ECOLAB.IOT.Plan.Entity.ScheduleDtos.SqlServer;
    using System.Collections.Generic;

    public static class Extensions
    {
        public static List<TableClearScheduleRecordDisplay> ToConvertTableClearScheduleRecordDisplays(this List<ClearPlan> clearPlans)
        { 
            var list=new List<TableClearScheduleRecordDisplay>();
            if (clearPlans == null)
                return list;

            foreach (var item in clearPlans)
            {
                list.AddRange(item.GetTableClearScheduleRecordDisplays());
            }

            return list;
        }
    }
}
