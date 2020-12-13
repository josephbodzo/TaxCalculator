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
    public class FlatRateCalculatorTests
    {
        [SetUp]
        public void SetUp()
        {
            _repository = new Mock<ITaxRateSettingRepository<FlatRateSetting>>();
            _calculator = new FlatRateCalculator(_repository.Object);
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
            const int expectedErrorCount =1;
            _repository.Setup(r => r.GetByTaxYearAsync(It.IsAny<TaxYear>()))
                .ReturnsAsync(new List<FlatRateSetting>());

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

        [Test]
        public async Task CalculateTax_Should_Return_Error_When_More_Than_One_TaxSetting_Exists()
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
                .ReturnsAsync(new List<FlatRateSetting>()
                {
                    new FlatRateSetting(),
                    new FlatRateSetting()
                });

            //Act
            var operationResult = await _calculator.CalculateTaxAsync(taxYear, 100M);

            //Assert
            _repository.Verify(r => r.GetByTaxYearAsync(taxYear), Times.Once);

            Assert.IsTrue(operationResult.HasErrors);
            var errors = operationResult.GetErrorMessages();
            
            Assert.AreEqual(expectedErrorCount, errors.Count);
            Assert.AreEqual(string.Empty, errors.Keys.First());
            Assert.AreEqual("More than 1 Flat Rate Tax settings have been found for the year: Tax year 1990 (23-Oct-2018 - 12-Oct-2019)", errors.Values.First().First());
        }
        
        [TestCase(10000 , 20.5, 2050.00)]
        [TestCase(0 , 20.5, 0)]
        [TestCase(1000 , 0, 0)]
        [TestCase(99.85 , 16.45, 16.43, Description = "Tax amount should be rounded to 2 decimal places(rounding up) i.e 16.425325M to 16.43")]
        [TestCase(99.40 , 16.45, 16.35, Description = "Tax amount should be rounded to 2 decimal places(rounding down) i.e 16.35130M to 16.35")]
        public async Task CalculateTax_Should_Calculate_FlatRateTax_Correctly(decimal annualIncome, decimal flatRatePerc, decimal expectedTaxAmount)
        {
            //Arrange
            var taxYear = new TaxYear()
            {
                ToDate = new DateTime(2019, 10, 12),
                FromDate = new DateTime(2018, 10, 23),
                Name = "Tax year 1990"
            };

            _repository.Setup(r => r.GetByTaxYearAsync(It.IsAny<TaxYear>()))
                .ReturnsAsync(new List<FlatRateSetting>()
                {
                    new FlatRateSetting{ FlatRatePerc = flatRatePerc}
                });

            //Act
            var operationResult = await _calculator.CalculateTaxAsync(taxYear, annualIncome);

            //Assert
            _repository.Verify(r => r.GetByTaxYearAsync(taxYear), Times.Once);

            Assert.IsFalse(operationResult.HasErrors);
            Assert.AreEqual(expectedTaxAmount, operationResult.Response);
        }

        private Mock<ITaxRateSettingRepository<FlatRateSetting>> _repository;
        private FlatRateCalculator _calculator;
    }
}
