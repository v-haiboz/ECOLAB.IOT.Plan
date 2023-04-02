namespace ECOLAB.IOT.Plan.Entity.Dtos.Certification
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class TokenInfoDto
    {
        public string? TokenType { get; set; }
        public int Expires_In { get; set; }

        public int Ext_Expires_In { get; set; }

        public string? Access_Token { get; set; }

    }
}
