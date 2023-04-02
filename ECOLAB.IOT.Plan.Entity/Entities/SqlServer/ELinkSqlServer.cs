using ECOLAB.IOT.Plan.Entity.Entities.SqlServer;

namespace ECOLAB.IOT.Plan.Entity.SqlServer
{
    [PrimaryKey("Id")]
    public class ELinkSqlServer
    {
        [NonSerialized]
        public int Id;

        public string? ServerName { get; set; }

        public string? DBName { get; set; }

        public string? UserId { get; set; }

        public string? Password { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime CreatedAt { get; set; }=DateTime.UtcNow;

        public virtual List<SqlTableClearSchedule> SqlTableClearSchedules { get; set; } = new List<SqlTableClearSchedule>();
    }
}
