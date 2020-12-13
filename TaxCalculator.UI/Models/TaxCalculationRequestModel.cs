using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace TaxCalculator.UI.Models
{
    public class TaxCalculationModel
    {
        [Display(Name = "Postal Code")]
        public string  PostalCode { get; set; }

        [Display(Name = "Annual Income")]
        public decimal  AnnualIncome { get; set; }

        [Display(Name = "Tax Year")]
        public string TaxYear { get; set; }

        [Display(Name = "Calculation Type")]
        public string CalculationType { get; set; }

        [Display(Name = "Tax Amount")]
        [DisplayFormat(DataFormatString = "{0:N}")]
        public decimal TaxAmount { get; set; }
    }

    public class TaxCalculationModelValidator : AbstractValidator<TaxCalculationModel>
    {
        public TaxCalculationModelValidator()
        {
            RuleFor(x => x.PostalCode)
                .NotNull()
                .MinimumLength(4)
                .MaximumLength(4);
            RuleFor(x => x.AnnualIncome)
                .GreaterThan(0);
        }
    }
}
