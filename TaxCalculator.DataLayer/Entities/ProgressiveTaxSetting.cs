namespace TaxCalculator.DataLayer.Entities
{
    public class ProgressiveTaxRateSetting : BaseTaxRateSetting
    {
        public decimal FromAmount { get; set; }
        public decimal? ToAmount { get; set; }
        public decimal TaxRatePerc { get; set; }
    }
}
