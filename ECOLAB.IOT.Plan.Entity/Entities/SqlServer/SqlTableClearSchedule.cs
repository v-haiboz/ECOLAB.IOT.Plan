namespace ECOLAB.IOT.Plan.Entity.Entities.SqlServer
{
    [PrimaryKey("Id")]
    public class SqlTableClearSchedule
    {
        [NonSerialized]
        public int Id;

        public int SqlServerId { get; set; }

        public ClearScheduleType? Type { get; set; }

        public string? PartitionKey { get; set; }

        public string? JObject { get; set; }

        public bool Enable { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}
