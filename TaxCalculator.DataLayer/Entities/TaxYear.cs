using System;

namespace TaxCalculator.DataLayer.Entities
{
    public class TaxYear : BaseEntity<long>
    {
        public string Name { get; set; }
        public DateTime FromDate { get; set; }  
        public DateTime ToDate { get; set; }

        public override string ToString()
        {
            return $"{Name} ({FromDate.ToLocalTime():dd-MMM-yyyy} - {ToDate.ToLocalTime():dd-MMM-yyyy})";
        }
    }
}
