namespace ECOLAB.IOT.Plan.Resolver.Policies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class JPolicy
    {
        public Batch? Batch { get; set; } = null;

    }

    public class Batch
    {
        public int Frequency { get; set; }

        public int Count { get; set; }
    }
}
