namespace TaxCalculator.DataLayer.Entities
{
    public abstract class BaseTaxRateSetting: BaseEntity<int>
    {
        public TaxYear TaxYear { get; set; }
    }
}
