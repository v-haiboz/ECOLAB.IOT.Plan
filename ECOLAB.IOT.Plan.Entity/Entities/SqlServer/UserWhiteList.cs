namespace ECOLAB.IOT.Plan.Entity.Entities.SqlServer
{
    using System;

    public class UserWhiteList
    {
        [NonSerialized]
        public int Id;

        public string? Email { get; set; }

        public string? SecretKey { get; set; }

        public DateTime ExpiredAt { get; set; } = DateTime.UtcNow.AddYears(1);

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
