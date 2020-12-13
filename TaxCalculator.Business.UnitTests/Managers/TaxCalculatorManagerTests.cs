using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using TaxCalculator.Business.Calculators;
using TaxCalculator.Business.Factories;
using TaxCalculator.Business.Managers;
using TaxCalculator.Business.Models;
using TaxCalculator.Common.Enums;
using TaxCalculator.Common.Responses;
using TaxCalculator.Common.Services;
using TaxCalculator.Common.ValidationRuleEngines;
using TaxCalculator.DataLayer.Entities;
using TaxCalculator.DataLayer.Repositories;
using DateTime = System.DateTime;

namespace TaxCalculator.Business.UnitTests.Managers
{
    [TestFixture]
    public class TaxCalculatorManagerTests
    {
        [SetUp]
        public void SetUp()
        {
            _calculationRepository = new Mock<ITaxCalculationRepository>();
            _taxYearRepository = new Mock<ITaxYearRepository>();
            _calculationMappingRepository =
                new Mock<IPostalCodeTaxCalculationMappingRepository>();
            _validationRuleEngine =
                new Mock<IValidationRuleEngine<TaxCalculationRequest, TaxCalculationResponse>>();
            _taxCalculatorFactory = new Mock<ITaxCalculatorFactory>();
            _clock = new Mock<IClock>();
            _taxCalculator = new Mock<ITaxCalculator>();

            _calculatorManager = new TaxCalculatorManager(_calculationRepository.Object,
                _taxYearRepository.Object,
                _calculationMappingRepository.Object,
                _validationRuleEngine.Object,
                _taxCalculatorFactory.Object,
                _clock.Object
            );
        }

        [Test]
        public async Task CalculateTax_Should_Return_ValidationRuleEngine_Errors()
        {
            //Arrange
            var calculationRequest = new TaxCalculationRequest();
            var validationRuleEngineResult = new OperationResult<TaxCalculationResponse>();

            const string expectedErrorKey1 = "1232";
            const string expectedErrorMessage1 = "This is an error";
            validationRuleEngineResult.AddErrorMessage(expectedErrorKey1, expectedErrorMessage1);

            const string expectedErrorKey2 = "error2";
            const string expectedErrorMessage2 = "This is an error number2";
            validationRuleEngineResult.AddErrorMessage(expectedErrorKey2, expectedErrorMessage2);

            const int expectedErrorCount = 2;

            _validationRuleEngine.Setup(s => s.Validate(calculationRequest))
                .Returns(validationRuleEngineResult);

            //Act
            var operationResult = await _calculatorManager.CalculateTax(calculationRequest);

            //Assert
            Assert.IsTrue(operationResult.HasErrors);

            var actualErrors = operationResult.GetErrorMessages();
            Assert.AreEqual(expectedErrorCount, actualErrors.Count);

            Assert.IsTrue(actualErrors.Any(e => e.Key == expectedErrorKey1 && e.Value.Contains(expectedErrorMessage1)));
            Assert.IsTrue(actualErrors.Any(e => e.Key == expectedErrorKey2 && e.Value.Contains(expectedErrorMessage2)));
        }

        [Test]
        public async Task CalculateTax_Should_Return_Calculation_Errors()
        {
            //Arrange
            var calculationRequest = new TaxCalculationRequest();
            var taxCalculationResult = new OperationResult<decimal>();

            const string expectedErrorKey1 = "calcError1";
            const string expectedErrorMessage1 = "This is a calc error";
            taxCalculationResult.AddErrorMessage(expectedErrorKey1, expectedErrorMessage1);

            const string expectedErrorKey2 = "calcError2";
            const string expectedErrorMessage2 = "This is calc error number2";
            taxCalculationResult.AddErrorMessage(expectedErrorKey2, expectedErrorMessage2);

            const int expectedErrorCount = 2;

            _validationRuleEngine.Setup(s => s.Validate(calculationRequest))
                .Returns(new OperationResult<TaxCalculationResponse>());

            _calculationMappingRepository.Setup(f => f.GetByPostalCodeAsync(calculationRequest.PostalCode))
                .ReturnsAsync(new PostalCodeCalculationTypeMapping());

            _taxCalculatorFactory.Setup(f => f.GetCalculator(It.IsAny<TaxCalculationType>()))
                .Returns(_taxCalculator.Object);

            _taxYearRepository.Setup(t => t.GetTaxYearAsync(It.IsAny<DateTime>()))
                .ReturnsAsync(new TaxYear());

            _taxCalculator.Setup(t => t.CalculateTaxAsync(It.IsAny<TaxYear>(), calculationRequest.AnnualIncome))
                .ReturnsAsync(taxCalculationResult);

            //Act
            var operationResult = await _calculatorManager.CalculateTax(calculationRequest);

            //Assert
            Assert.IsTrue(operationResult.HasErrors);

            var actualErrors = operationResult.GetErrorMessages();
            Assert.AreEqual(expectedErrorCount, actualErrors.Count);

            Assert.IsTrue(actualErrors.Any(e => e.Key == expectedErrorKey1 && e.Value.Contains(expectedErrorMessage1)));
            Assert.IsTrue(actualErrors.Any(e => e.Key == expectedErrorKey2 && e.Value.Contains(expectedErrorMessage2)));
        }

        [Test]
        public async Task CalculateTax_Should_Successfully_Calculate_Tax()
        {
            //Arrange
            const string postalCode = "12234";
            const decimal incomeAmount = 23000M;
            var requestDate = new DateTime(2020, 12, 15);

            var calculationRequest = new TaxCalculationRequest()
            {
                AnnualIncome = incomeAmount,
                PostalCode = postalCode
            };

            const int expectedTaxAmount = 100;
            const TaxCalculationType expectedCalculationType = TaxCalculationType.PROGRESSIVE_TAX;
            const string expectedTaxYear = "Tax Year (01-Jan-2020 - 02-Jan-2021)";

            var taxYear = new TaxYear
            {
                FromDate = new DateTime(2020, 01, 01),
                ToDate = new DateTime(2021, 01, 02),
                Name = "Tax Year"
            };

            var taxCalculationResult = new OperationResult<decimal>
            {
                Response = expectedTaxAmount
            };

            _clock.Setup(c => c.GetCurrentDateTime()).Returns(requestDate);

            _validationRuleEngine.Setup(s => s.Validate(It.IsAny<TaxCalculationRequest>()))
                .Returns(new OperationResult<TaxCalculationResponse>());

            _calculationMappingRepository.Setup(f => f.GetByPostalCodeAsync(It.IsAny<string>()))
                .ReturnsAsync(new PostalCodeCalculationTypeMapping { CalculationType = expectedCalculationType});
            
            _taxCalculatorFactory.Setup(f => f.GetCalculator(It.IsAny<TaxCalculationType>()))
                .Returns(_taxCalculator.Object);

            _taxYearRepository.Setup(t => t.GetTaxYearAsync(It.IsAny<DateTime>()))
                .ReturnsAsync(taxYear);

            _taxCalculator.Setup(t => t.CalculateTaxAsync(It.IsAny<TaxYear>(), It.IsAny<decimal>()))
                .ReturnsAsync(taxCalculationResult);

            //Act
            var operationResult = await _calculatorManager.CalculateTax(calculationRequest);

            //Assert
            Assert.IsNotNull(operationResult?.Response);
            Assert.IsFalse(operationResult.HasErrors);

            _validationRuleEngine.Verify(s => s.Validate(calculationRequest), Times.Once);
            _calculationMappingRepository.Verify(f => f.GetByPostalCodeAsync(postalCode),
                Times.Once);
            _taxCalculatorFactory.Verify(f => f.GetCalculator(expectedCalculationType), Times.Once);

            _taxYearRepository.Verify(t => t.GetTaxYearAsync(requestDate), Times.Once);

            _taxCalculator.Verify(t => t.CalculateTaxAsync(taxYear, incomeAmount), Times.Once());

            Assert.AreEqual(expectedTaxAmount, operationResult.Response.TaxAmount);
            Assert.AreEqual(expectedCalculationType, operationResult.Response.CalculationType);
            Assert.AreEqual(expectedTaxYear, operationResult.Response.TaxYear);
        }

        [Test]
        public async Task CalculateTax_Should_Save_Successfully_Calculated_Tax()
        {
            //Arrange
            const string expectedPostalCode = "12234";
            const decimal expectedAnnualIncome = 23000M;
            const int expectedTaxAmount = 100;
            const TaxCalculationType expectedCalculationType = TaxCalculationType.PROGRESSIVE_TAX;
            DateTime expectedCreationDate = new DateTime(2020, 12, 16);
            Guid expectedRequestedBy = Guid.Parse("e64a32e6-f2a9-43d5-b87b-79c845955f93");

            var calculationRequest = new TaxCalculationRequest()
            {
                AnnualIncome = expectedAnnualIncome,
                PostalCode = expectedPostalCode,
                RequestedBy = expectedRequestedBy
            };

            TaxCalculation savedRecord = null;

            var taxYear = new TaxYear
            {
                FromDate = new DateTime(2020, 01, 01),
                ToDate = new DateTime(2021, 01, 02),
                Name = "Tax Year"
            };

            var taxCalculationResult = new OperationResult<decimal>
            {
                Response = expectedTaxAmount
            };

            _clock.Setup(c => c.GetCurrentDateTime()).Returns(expectedCreationDate);

            _validationRuleEngine.Setup(s => s.Validate(It.IsAny<TaxCalculationRequest>()))
                .Returns(new OperationResult<TaxCalculationResponse>());

            _calculationMappingRepository.Setup(f => f.GetByPostalCodeAsync(It.IsAny<string>()))
                .ReturnsAsync(new PostalCodeCalculationTypeMapping { CalculationType = expectedCalculationType });

            _taxCalculatorFactory.Setup(f => f.GetCalculator(It.IsAny<TaxCalculationType>()))
                .Returns(_taxCalculator.Object);

            _taxYearRepository.Setup(t => t.GetTaxYearAsync(It.IsAny<DateTime>()))
                .ReturnsAsync(taxYear);

            _taxCalculator.Setup(t => t.CalculateTaxAsync(It.IsAny<TaxYear>(), It.IsAny<decimal>()))
                .ReturnsAsync(taxCalculationResult);

            _calculationRepository.Setup(r => r.AddAsync(It.IsAny<TaxCalculation>()))
                .Callback((TaxCalculation c) => savedRecord = c);

            //Act
            var operationResult = await _calculatorManager.CalculateTax(calculationRequest);

            //Assert
            Assert.IsNotNull(operationResult?.Response);
            Assert.IsFalse(operationResult.HasErrors);

            _calculationRepository.Verify(r=> r.AddAsync(It.IsAny<TaxCalculation>()), Times.Once);

            Assert.IsNotNull(savedRecord);
            Assert.AreEqual(expectedAnnualIncome, savedRecord.AnnualIncome);
            Assert.AreEqual(expectedTaxAmount, savedRecord.TaxAmount);
            Assert.AreEqual(expectedCalculationType, savedRecord.CalculationType);
            Assert.AreEqual(expectedRequestedBy, savedRecord.CreatedBy);
            Assert.AreEqual(expectedCreationDate, savedRecord.CreationDate);
            Assert.AreEqual(expectedPostalCode, savedRecord.PostalCode);
            Assert.AreEqual(taxYear, savedRecord.TaxYear);
        }

        private Mock<ITaxCalculationRepository> _calculationRepository;
        private Mock<ITaxYearRepository> _taxYearRepository;
        private Mock<IPostalCodeTaxCalculationMappingRepository> _calculationMappingRepository;
        private Mock<IValidationRuleEngine<TaxCalculationRequest, TaxCalculationResponse>> _validationRuleEngine;
        private Mock<ITaxCalculatorFactory> _taxCalculatorFactory;
        private Mock<ITaxCalculator> _taxCalculator;
        private Mock<IClock> _clock;
        private ITaxCalculatorManager _calculatorManager;
    }
}
