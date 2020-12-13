using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TaxCalculator.API.Controllers;
using TaxCalculator.API.Models;
using TaxCalculator.Business.Managers;
using TaxCalculator.Business.Models;
using TaxCalculator.Common.Enums;
using TaxCalculator.Common.Extension;
using TaxCalculator.Common.Responses;

namespace TaxCalculator.API.UnitTests.Controllers
{
    [TestFixture]
    public class TaxCalculatorControllerTests
    {
        [SetUp]
        public void SetUp()
        {
             _manager = new Mock<ITaxCalculatorManager>();
            _controller = new TaxCalculatorController(_manager.Object);
        }

        [Test]
        public async Task Calculate_ReturnsBadRequestResult_WhenModelStateIsInvalid()
        {
            //Arrange
            var expectedErrorMessage = "my errors";
            var errorKey = "errorKey";
            _controller.ModelState.AddModelError(errorKey, expectedErrorMessage);
            TaxCalculationRequestModel model = new TaxCalculationRequestModel();
            
            //Act
            var result = await _controller.Calculate(model);
           
            //Assert
            Assert.IsInstanceOf<BadRequestResult>(result.Result);
        }

        [Test]
        public async Task Calculate_Should_Calculate()
        {
            //Arrange
            const decimal annualIncome = 250000;
            const string postalCode = "1234";
            const string expectedTaxYear = "Tax Year";
            const decimal expectedTaxAmount = 234.56M;
            const TaxCalculationType expectedCalculationType = TaxCalculationType.PROGRESSIVE_TAX;
            string expectedCalculationTypeDescription = expectedCalculationType.GetDescription();

            TaxCalculationRequestModel model = new TaxCalculationRequestModel
            {
                AnnualIncome = annualIncome,
                PostalCode = postalCode
            };

            TaxCalculationRequest actualManagerRequest = null;

            TaxCalculationResponse managerResponse = new TaxCalculationResponse
            {
                CalculationType = expectedCalculationType,
                TaxAmount = expectedTaxAmount,
                TaxYear = expectedTaxYear
            };

            _manager.Setup(r => r.CalculateTax(It.IsAny<TaxCalculationRequest>()))
                .Callback((TaxCalculationRequest r) => actualManagerRequest = r)
                .ReturnsAsync(() => new OperationResult<TaxCalculationResponse>(managerResponse));

            //Act
            var result = await _controller.Calculate(model);

            //Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            TaxCalculationResponseModel
                responseMode = (TaxCalculationResponseModel)((OkObjectResult)result.Result).Value;
            Assert.AreEqual(expectedCalculationTypeDescription, responseMode.CalculationType);
            Assert.AreEqual(expectedTaxYear, responseMode.TaxYear);
            Assert.AreEqual(expectedTaxAmount, responseMode.TaxAmount);

            _manager.Verify(f => f.CalculateTax(It.IsAny<TaxCalculationRequest>()), Times.Once);
            Assert.AreEqual(annualIncome, actualManagerRequest.AnnualIncome);
            Assert.AreEqual(postalCode, actualManagerRequest.PostalCode);
        }

        [Test]
        public async Task Calculate_Should_Return_BadRequest_When_Manager_Returns_Errors()
        {
            //Arrange
            const decimal annualIncome = 250000;
            const string postalCode = "1234";
            var expectedErrorMessage = "my errors";
            var errorKey = "errorKey";

            TaxCalculationRequestModel model = new TaxCalculationRequestModel
            {
                AnnualIncome = annualIncome,
                PostalCode = postalCode
            };

            TaxCalculationRequest actualManagerRequest = null;

            var managerResult = new OperationResult<TaxCalculationResponse>();
            managerResult.AddErrorMessage(errorKey, expectedErrorMessage);

            _manager.Setup(r => r.CalculateTax(It.IsAny<TaxCalculationRequest>()))
                .Callback((TaxCalculationRequest r) => actualManagerRequest = r)
                .ReturnsAsync(() => managerResult);

            //Act
            var result = await _controller.Calculate(model);

            //Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
            var
                responseModel = (SerializableError)((BadRequestObjectResult)result.Result).Value;
            Assert.AreEqual(expectedErrorMessage, ((string[])responseModel[errorKey])[0]);

            _manager.Verify(f => f.CalculateTax(It.IsAny<TaxCalculationRequest>()), Times.Once);
            Assert.AreEqual(annualIncome, actualManagerRequest.AnnualIncome);
            Assert.AreEqual(postalCode, actualManagerRequest.PostalCode);
        }


        private TaxCalculatorController _controller;
        private Mock<ITaxCalculatorManager> _manager;
    }
}
