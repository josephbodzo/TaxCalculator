using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaxCalculator.UI.Extensions;
using TaxCalculator.UI.Models;
using TaxCalculatorApi;
using ProblemDetails = TaxCalculatorApi.ProblemDetails;

namespace TaxCalculator.UI.Pages.Tax
{
    public class CalculateTaxModel : PageModel
    {
        private readonly ITaxCalculatorClient _calculatorClient;
        public CalculateTaxModel(ITaxCalculatorClient calculatorClient)
        {
            _calculatorClient = calculatorClient;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public TaxCalculationModel TaxCalculation { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var taxResult = await _calculatorClient.CalculateAsync(new TaxCalculationRequestModel
                {
                    AnnualIncome = (double) TaxCalculation.AnnualIncome,
                    PostalCode = TaxCalculation.PostalCode
                });

                TaxCalculation.TaxYear = taxResult.TaxYear;
                TaxCalculation.CalculationType = taxResult.CalculationType;
                TaxCalculation.TaxAmount = (decimal) taxResult.TaxAmount;

            }
            catch (ApiException<ProblemDetails> e)
            {
                ModelState.MapErrors(e.Result);
            }
 
            return Page();
        }
    }
}
