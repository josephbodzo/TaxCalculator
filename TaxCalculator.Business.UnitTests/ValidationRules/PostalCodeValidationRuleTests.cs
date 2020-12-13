using System.Linq;
using Moq;
using NUnit.Framework;
using TaxCalculator.Business.Models;
using TaxCalculator.Business.ValidationRules.TaxCalculation;
using TaxCalculator.DataLayer.Entities;
using TaxCalculator.DataLayer.Repositories;

namespace TaxCalculator.Business.UnitTests.ValidationRules
{
    [TestFixture]
    public class PostalCodeValidationRuleTests
    {
        [SetUp]
        public void SetUp()
        {
            _mappingRepository = new Mock<IPostalCodeTaxCalculationMappingRepository>();
            _validationRule = new PostalCodeValidationRule(_mappingRepository.Object);
        }

        [Test]
        public void Validate_Should_Return_Error_When_PostalCode_Mapping_Is_Not_Found()
        {
            //Arrange
            const string postalCode = "4577";
            const int expectedErrorCount = 1;

            _mappingRepository.Setup(r => r.GetByPostalCodeAsync(It.IsAny<string>()))
                .ReturnsAsync(() => null);

            //Act
            var operationResult = _validationRule.Validate(new TaxCalculationRequest()
            {
                PostalCode = postalCode
            });

            //Assert
            _mappingRepository.Verify(r => r.GetByPostalCodeAsync(postalCode), Times.Once());

            Assert.IsTrue(operationResult.HasErrors);
            var errors = operationResult.GetErrorMessages();

            Assert.AreEqual(expectedErrorCount, errors.Count);
            Assert.AreEqual("PostalCode", errors.Keys.First());
            Assert.AreEqual($"Postal code: {postalCode} was not found.", errors.Values.First().First());
        }

        [Test]
        public void Validate_Should_Return_No_Errors_When_PostalCode_Mapping_Is_Found()
        {
            //Arrange
            const string postalCode = "4577";
            const int expectedErrorCount = 0;

            _mappingRepository.Setup(r => r.GetByPostalCodeAsync(It.IsAny<string>()))
                .ReturnsAsync(() => new PostalCodeCalculationTypeMapping());

            //Act
            var operationResult = _validationRule.Validate(new TaxCalculationRequest()
            {
                PostalCode = postalCode
            });

            //Assert
            _mappingRepository.Verify(r => r.GetByPostalCodeAsync(postalCode), Times.Once());

            Assert.IsFalse(operationResult.HasErrors);
            var errors = operationResult.GetErrorMessages();

            Assert.AreEqual(expectedErrorCount, errors.Count);
        }

        private Mock<IPostalCodeTaxCalculationMappingRepository> _mappingRepository;
        private PostalCodeValidationRule _validationRule;
    }
}
