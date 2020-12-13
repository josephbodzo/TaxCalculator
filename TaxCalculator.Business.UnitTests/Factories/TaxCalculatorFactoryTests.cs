using System;
using Moq;
using NUnit.Framework;
using TaxCalculator.Business.Calculators;
using TaxCalculator.Business.Calculators.Implementations;
using TaxCalculator.Business.Factories;
using TaxCalculator.Common.Enums;
using TaxCalculator.DataLayer.Entities;
using TaxCalculator.DataLayer.Repositories;

namespace TaxCalculator.Business.UnitTests.Factories
{
    [TestFixture]
    public class TaxCalculatorFactoryTests
    {
        [SetUp]
        public void SetUp()
        {
            _flatRateSettingRepository = new Mock<ITaxRateSettingRepository<FlatRateSetting>>();
            _flatValueSettingRepository = new Mock<ITaxRateSettingRepository<FlatValueSetting>>();
            _progressiveTaxRateSettingRepository = new Mock<ITaxRateSettingRepository<ProgressiveTaxRateSetting>>();
            _taxCalculator = new TaxCalculatorFactory(
                _flatRateSettingRepository.Object,
                _flatValueSettingRepository.Object,
                _progressiveTaxRateSettingRepository.Object);
        }

        [TestCase(TaxCalculationType.FLAT_RATE, ExpectedResult = typeof(FlatRateCalculator))]
        [TestCase(TaxCalculationType.FLAT_VALUE, ExpectedResult = typeof(FlatValueCalculator))]
        [TestCase(TaxCalculationType.PROGRESSIVE_TAX, ExpectedResult = typeof(ProgressiveTaxCalculator))]
        public Type GetCalculator_Returns_Correct_Type(TaxCalculationType calculationType)
        {
            //Act
            return _taxCalculator.GetCalculator(calculationType).GetType();
        }

        [Test]
        public void GetCalculator_Should_Process_All_Enum_Values([Values] TaxCalculationType calculationType)
        {
            //Act
            var result = _taxCalculator.GetCalculator(calculationType);

            //Assert
            Assert.IsInstanceOf<ITaxCalculator>(result);
        }

        private  Mock<ITaxRateSettingRepository<FlatRateSetting>> _flatRateSettingRepository;
        private  Mock<ITaxRateSettingRepository<FlatValueSetting>> _flatValueSettingRepository;
        private  Mock<ITaxRateSettingRepository<ProgressiveTaxRateSetting>> _progressiveTaxRateSettingRepository;
        private ITaxCalculatorFactory _taxCalculator;
    }
}
