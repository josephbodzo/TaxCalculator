using System;
using System.Threading.Tasks;
using TaxCalculator.API.Extensions;
using TaxCalculator.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaxCalculator.Business.Managers;
using TaxCalculator.Business.Models;
using TaxCalculator.Common.Extension;

namespace TaxCalculator.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TaxCalculatorController : ControllerBase
    {
        private readonly ITaxCalculatorManager _calculatorManager;

        public TaxCalculatorController(ITaxCalculatorManager calculatorManager)
        {
            _calculatorManager = calculatorManager;
        }

        [HttpPost]
        [Route("Calculate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TaxCalculationResponseModel>> Calculate(TaxCalculationRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _calculatorManager.CalculateTax(new TaxCalculationRequest
            {
                AnnualIncome = model.AnnualIncome,
                PostalCode = model.PostalCode,
                RequestedBy = Guid.NewGuid()// to be set when authentication is available
            }).ConfigureAwait(false);

            if (result.HasErrors)
            {
                ModelState.AddErrors(result.GetErrorMessages());
                return BadRequest(ModelState);
            }

            var responseModel = new TaxCalculationResponseModel
            {
                CalculationType = result.Response.CalculationType.GetDescription(),
                TaxAmount = result.Response.TaxAmount,
                TaxYear = result.Response.TaxYear
            };

            return Ok(responseModel);
        }
    }
}
