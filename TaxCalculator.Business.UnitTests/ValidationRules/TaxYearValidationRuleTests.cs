using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using TaxCalculator.Business.Models;
using TaxCalculator.Business.ValidationRules.TaxCalculation;
using TaxCalculator.Common.Services;
using TaxCalculator.DataLayer.Entities;
using TaxCalculator.DataLayer.Repositories;

namespace TaxCalculator.Business.UnitTests.ValidationRules
{
    [TestFixture]
    public class TaxYearValidationRuleTests
    {
        [SetUp]
        public void SetUp()
        {
            _repository = new Mock<ITaxYearRepository>();
            _clock = new Mock<IClock>();
            _validationRule = new TaxYearValidationRule(_repository.Object, _clock.Object);
        }

        [Test]
        public void Validate_Should_Return_Error_When_TaxYear_Is_Not_Found()
        {
            //Arrange
            DateTime requestDate = new DateTime(2020, 11, 12) ;
            const int expectedErrorCount = 1;

            _repository.Setup(r => r.GetTaxYearAsync(It.IsAny<DateTime>()))
                .ReturnsAsync(() => null);

            _clock.Setup(c => c.GetCurrentDateTime()).Returns(requestDate);

            //Act
            var operationResult = _validationRule.Validate(new TaxCalculationRequest());

            //Assert
            _repository.Verify(r => r.GetTaxYearAsync(requestDate), Times.Once);

            Assert.IsTrue(operationResult.HasErrors);
            var errors = operationResult.GetErrorMessages();

            Assert.AreEqual(expectedErrorCount, errors.Count);
            Assert.AreEqual(string.Empty, errors.Keys.First());
            Assert.AreEqual("Tax year was not found for today's date.", errors.Values.First().First());
        }

        [Test]
        public void Validate_Should_Return_No_Errors_When_TaxYear_Is_Found()
        {
            //Arrange
            DateTime requestDate = new DateTime(2020, 11, 12);
            const int expectedErrorCount = 0;

            _repository.Setup(r => r.GetTaxYearAsync(It.IsAny<DateTime>()))
                .ReturnsAsync(() => new TaxYear());
            _clock.Setup(c => c.GetCurrentDateTime()).Returns(requestDate);

            //Act
            var operationResult = _validationRule.Validate(new TaxCalculationRequest());

            //Assert
            _repository.Verify(r => r.GetTaxYearAsync(requestDate), Times.Once);

            Assert.IsFalse(operationResult.HasErrors);
            var errors = operationResult.GetErrorMessages();

            Assert.AreEqual(expectedErrorCount, errors.Count);
        }

        private Mock<ITaxYearRepository> _repository;
        private Mock<IClock> _clock;
        private TaxYearValidationRule _validationRule;
    }
}
