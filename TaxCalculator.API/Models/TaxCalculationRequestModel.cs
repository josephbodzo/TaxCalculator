using FluentValidation;

namespace TaxCalculator.API.Models
{
    public class TaxCalculationRequestModel
    {
        public string  PostalCode { get; set; }  
        public decimal  AnnualIncome { get; set; }
    }

    public class TaxCalculationRequestModelValidator : AbstractValidator<TaxCalculationRequestModel>
    {
        public TaxCalculationRequestModelValidator()
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
