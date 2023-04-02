namespace ECOLAB.IOT.Plan.Entity.Dtos.Sql
{
    using System;

    public class PolicyDto<Metrics>
    { 

        public string? TableName { get; set; }

        public int? Frequency { get; set; }

        public int? Count { get; set; }

        /// <summary>
        /// Metrics
        /// </summary>
        public Metrics? Where { get; set; }


        public bool Validate()
        {
            if (Frequency.HasValue && Count.HasValue)
            {
                if (Frequency.Value < 1 || Frequency.Value > 50)
                {
                    throw new Exception("Frequency must be greater than or equal to 1 and less than or equal to 30.");
                }

                if (Count.Value < 1000 || Count.Value > 20000)
                {
                    throw new Exception("Count must be greater than or equal to 1000 and less than or equal to 20000.");
                }
            }

            return true;
        }

    }
}
