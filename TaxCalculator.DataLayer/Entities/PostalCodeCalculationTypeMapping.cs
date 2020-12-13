using TaxCalculator.Common.Enums;

namespace TaxCalculator.DataLayer.Entities
{
    public class PostalCodeCalculationTypeMapping: BaseEntity<int>
    {
        public string PostalCode { get; set; }
        public TaxCalculationType CalculationType { get; set; }
    }
}
