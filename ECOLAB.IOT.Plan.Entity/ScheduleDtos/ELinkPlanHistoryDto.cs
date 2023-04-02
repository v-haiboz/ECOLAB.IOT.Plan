namespace ECOLAB.IOT.Plan.Entity.ScheduleDtos
{
    using ECOLAB.IOT.Plan.Entity.Entities;

    public class ELinkPlanHistoryDto
    {
        public ELinkPlanHistoryCategory Category { get; set; }
        public string? Message { get; set; }

        public string? TargetRowData { get; set; }

        public string? SourceRowData { get; set; }

        public ELinkPlanHistory ToCovertELinkPlanHistory()
        {
            return new ELinkPlanHistory()
            {
                Category= Category.ToString(),
                Message = Message,
                TargetRowData = TargetRowData,
                SourceRowData = SourceRowData
            };
        }

    }

    public enum ELinkPlanHistoryType
    {
        None = 0,
        Info = 1,
        Warning = 2,
        Error = 3
    }

    public enum ELinkPlanHistoryCategory
    {
        BuildClearPlan = 1,
        ExecuteClearPlan = 2,
    }

}
