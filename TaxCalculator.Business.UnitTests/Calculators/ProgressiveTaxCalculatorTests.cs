using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using TaxCalculator.Business.Calculators.Implementations;
using TaxCalculator.DataLayer.Entities;
using TaxCalculator.DataLayer.Repositories;

namespace TaxCalculator.Business.UnitTests.Calculators
{
    [TestFixture]
    public class ProgressiveTaxCalculatorTests
    {
        [SetUp]
        public void SetUp()
        {
            _repository = new Mock<ITaxRateSettingRepository<ProgressiveTaxRateSetting>>();
            _calculator = new ProgressiveTaxCalculator(_repository.Object);
        }

        [Test]
        public async Task CalculateTax_Should_Return_Error_When_No_Tax_Setting_Found()
        {
            //Arrange
            var taxYear = new TaxYear()
            {
                ToDate = new DateTime(2019, 10, 12),
                FromDate = new DateTime(2018, 10, 23),
                Name = "Tax year 1990"
            };
            const int expectedErrorCount = 1;
            _repository.Setup(r => r.GetByTaxYearAsync(It.IsAny<TaxYear>()))
                .ReturnsAsync(new List<ProgressiveTaxRateSetting>());

            //Act
            var operationResult = await _calculator.CalculateTaxAsync(taxYear, 100M);

            //Assert
            _repository.Verify(r => r.GetByTaxYearAsync(taxYear), Times.Once);

            Assert.IsTrue(operationResult.HasErrors);
            var errors = operationResult.GetErrorMessages();


            Assert.AreEqual(expectedErrorCount, errors.Count);
            Assert.AreEqual(string.Empty, errors.Keys.First());
            Assert.AreEqual("No Tax Rate setting have been found for the tax year: Tax year 1990 (23-Oct-2018 - 12-Oct-2019)", errors.Values.First().First());
        }

        [TestCase(30000, 5400)]
        [TestCase(250000, 48527.74)]
        [TestCase(440000, 103847.43)]
        [TestCase(600000, 161665.68)]
        [TestCase(744801, 218137.68)]
        [TestCase(1577300, 559462.27)]
        [TestCase(2100000, 794676.82)]
        public async Task CalculateTax_Should_Calculate_FlatRateTax_Correctly(decimal annualIncome,  decimal expectedTaxAmount)
        {
            //Arrange
            var taxYear = new TaxYear()
            {
                ToDate = new DateTime(2019, 10, 12),
                FromDate = new DateTime(2018, 10, 23),
                Name = "Tax year 1990"
            };

            _repository.Setup(r => r.GetByTaxYearAsync(It.IsAny<TaxYear>()))
                .ReturnsAsync(new List<ProgressiveTaxRateSetting>
                {
                    new ProgressiveTaxRateSetting{ FromAmount = 0M,     ToAmount = 205900M, TaxRatePerc = 18M},
                    new ProgressiveTaxRateSetting{ FromAmount = 205901M, ToAmount =  321600M, TaxRatePerc = 26M},
                    new ProgressiveTaxRateSetting{ FromAmount = 321601M, ToAmount = 445100M, TaxRatePerc = 31M},
                    new ProgressiveTaxRateSetting{ FromAmount = 445101M, ToAmount =  584200M, TaxRatePerc = 36M},
                    new ProgressiveTaxRateSetting{ FromAmount = 584201M, ToAmount =  744800M, TaxRatePerc = 39M},
                    new ProgressiveTaxRateSetting{ FromAmount = 744801M, ToAmount =  1577300M, TaxRatePerc = 41M},
                    new ProgressiveTaxRateSetting{ FromAmount = 1577301M, ToAmount =  null, TaxRatePerc = 45M},
                });

            //Act
            var operationResult = await _calculator.CalculateTaxAsync(taxYear, annualIncome);

            //Assert
            _repository.Verify(r => r.GetByTaxYearAsync(taxYear), Times.Once);

            Assert.IsFalse(operationResult.HasErrors);
            Assert.AreEqual(expectedTaxAmount, operationResult.Response);
        }

        private Mock<ITaxRateSettingRepository<ProgressiveTaxRateSetting>> _repository;
        private ProgressiveTaxCalculator _calculator;
    }
}
