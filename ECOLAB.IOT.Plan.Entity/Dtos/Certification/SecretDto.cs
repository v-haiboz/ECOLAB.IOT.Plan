namespace ECOLAB.IOT.Plan.Entity.Dtos.Certification
{
    using System;

    public class SecretDto
    {
        public string? SecretKey { get; set; }

        public DateTime AbsoluteExpirationTime { get; set; }
    }
}
