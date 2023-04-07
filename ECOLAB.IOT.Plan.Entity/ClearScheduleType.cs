namespace ECOLAB.IOT.Plan.Entity
{
    using System;

    public enum ClearScheduleType
    {
        CustomDateTimeMetrics = 1,
        CustomIntMetrics = 2,
        PartialMatchDateTimeMetrics= 3,
        PartialMatchIntMetrics = 4,
        DynamicDateTimeMetrics = 5,
        DynamicIntMetrics = 6,
    }
}
