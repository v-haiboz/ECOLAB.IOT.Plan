namespace ECOLAB.IOT.Plan.Entity.Dtos.Certification
{
    using ECOLAB.IOT.Plan.Entity.Dtos.Sql;
    using System;
    using System.Net.NetworkInformation;

    public class CertificationInfoDto
    {
        public RequestTokenDto? UserWhiteList { get; set; }

        public DateTime AbsoluteExpirationtime { get; set; }

        public string? FirstMACAddress { get; set; }

    }
}
