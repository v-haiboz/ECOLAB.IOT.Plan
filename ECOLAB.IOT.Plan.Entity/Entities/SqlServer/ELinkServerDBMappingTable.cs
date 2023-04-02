namespace ECOLAB.IOT.Plan.Entity.Entities.SqlServer
{
    using System;

    public class ELinkServerDBMappingTable
    {
        [NonSerialized]
        public int Id;

        public string? ServerName { get; set; }

        public string? DBName { get; set; }

        public string? TableName { get; set; }

        public string? ColumnName { get; set; }

        public int Status { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
