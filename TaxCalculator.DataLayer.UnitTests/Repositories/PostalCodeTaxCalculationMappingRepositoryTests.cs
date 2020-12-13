using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TaxCalculator.Common.Enums;
using TaxCalculator.DataLayer.DatabaseContexts;
using TaxCalculator.DataLayer.Entities;
using TaxCalculator.DataLayer.Repositories.Implementations;

namespace TaxCalculator.DataLayer.UnitTests.Repositories
{
    [TestFixture]
    public class PostalCodeTaxCalculationMappingRepositoryTests
    {
        [Test]
        public async Task GetByPostalCodeAsync_Should_Return_Mapping()
        {
            //Arrange
            const string postalCode = "1232";
            const TaxCalculationType expectedCalculationType = TaxCalculationType.PROGRESSIVE_TAX;

            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: nameof(PostalCodeTaxCalculationMappingRepositoryTests) + Guid.NewGuid())
                .Options;

            await using var context = new ApplicationContext(options);

            var mapping = new PostalCodeCalculationTypeMapping()
            {
                CalculationType = expectedCalculationType,
                PostalCode = postalCode
            };
            context.PostalCodeCalculationTypeMappings.Add(mapping);
            await context.SaveChangesAsync();

            _repository = new PostalCodeTaxCalculationMappingRepository(context);

            //Act
            PostalCodeCalculationTypeMapping entity = await _repository.GetByPostalCodeAsync(postalCode);

            //Assert
           Assert.AreEqual(postalCode, entity.PostalCode);
           Assert.AreEqual(expectedCalculationType, entity.CalculationType);
        }

        [Test]
        public async Task GetByPostalCodeAsync_Should_Return_Null_If_Not_Exists()
        {
            //Arrange
            const string postalCode = "0000";

            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: nameof(PostalCodeTaxCalculationMappingRepositoryTests) + Guid.NewGuid())
                .Options;

            await using var context = new ApplicationContext(options);

            _repository = new PostalCodeTaxCalculationMappingRepository(context);

            //Act
            PostalCodeCalculationTypeMapping entity = await _repository.GetByPostalCodeAsync(postalCode);

            //Assert
            Assert.IsNull(entity);
        }

        [Test]
        public async Task GetByPostalCodeAsync_Should_Throw_Exception_For_Duplicated_Mapping()
        {
            //Arrange
            const string postalCode = "1000";

            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: nameof(PostalCodeTaxCalculationMappingRepositoryTests) + Guid.NewGuid())
                .Options;

            await using var context = new ApplicationContext(options);

            var mapping = new PostalCodeCalculationTypeMapping()
            {
                CalculationType = TaxCalculationType.FLAT_RATE,
                PostalCode = postalCode
            };
            context.PostalCodeCalculationTypeMappings.Add(mapping);

            var mapping2 = new PostalCodeCalculationTypeMapping()
            {
                CalculationType = TaxCalculationType.PROGRESSIVE_TAX,
                PostalCode = postalCode
            };
            context.PostalCodeCalculationTypeMappings.Add(mapping2);

            await context.SaveChangesAsync();

            _repository = new PostalCodeTaxCalculationMappingRepository(context);

            //Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _repository.GetByPostalCodeAsync(postalCode));
        }

        private PostalCodeTaxCalculationMappingRepository _repository;

    }
}
