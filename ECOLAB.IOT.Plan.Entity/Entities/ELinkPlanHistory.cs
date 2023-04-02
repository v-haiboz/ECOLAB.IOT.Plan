using ECOLAB.IOT.Plan.Entity.ScheduleDtos;

namespace ECOLAB.IOT.Plan.Entity.Entities
{
    public class ELinkPlanHistory
    {

        public int Id { get; set; }

        public string? Category{ get; set; }
        public string? Type { get; set; }

        public string? Message { get; set; }

        public string? TargetRowData { get; set; }

        public string? SourceRowData { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
