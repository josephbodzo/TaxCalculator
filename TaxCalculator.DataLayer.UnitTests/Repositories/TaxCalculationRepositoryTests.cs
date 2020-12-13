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
    public class TaxCalculationRepositoryTests
    {
 
        [Test]
        public async Task Add_Should_Save_TaxCalculation()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: GetType().Name + Guid.NewGuid())
                .Options;

            await using var context = new ApplicationContext(options);
            
            var taxCalculation = new TaxCalculation
            {
                CalculationType = TaxCalculationType.FLAT_RATE,
                TaxYear = new TaxYear(),
                AnnualIncome = 300000,
                CreatedBy = Guid.Parse("b11ac84a-028e-4288-ae85-49f647cee187"),
                CreationDate = new DateTime(2020, 10, 10),
                PostalCode = "1212",
                TaxAmount = 200.45M
            };

            _repository = new TaxCalculationRepository(context);

            //Act
             await _repository.AddAsync(taxCalculation);

            //Assert
            var entity = await context.TaxCalculations.FirstOrDefaultAsync();
            Assert.IsNotNull(entity);
            Assert.AreEqual(taxCalculation.TaxYear, entity.TaxYear);
            Assert.AreEqual(taxCalculation.CalculationType, entity.CalculationType);
            Assert.AreEqual(taxCalculation.AnnualIncome, entity.AnnualIncome);
            Assert.AreEqual(taxCalculation.CreatedBy, entity.CreatedBy);
            Assert.AreEqual(taxCalculation.CreationDate, entity.CreationDate);
            Assert.AreEqual(taxCalculation.PostalCode, entity.PostalCode);
            Assert.AreEqual(taxCalculation.TaxAmount, entity.TaxAmount);
        }

       private TaxCalculationRepository _repository;

    }
}
