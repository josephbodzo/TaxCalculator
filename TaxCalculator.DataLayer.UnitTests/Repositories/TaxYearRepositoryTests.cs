using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TaxCalculator.DataLayer.DatabaseContexts;
using TaxCalculator.DataLayer.Entities;
using TaxCalculator.DataLayer.Repositories.Implementations;

namespace TaxCalculator.DataLayer.UnitTests.Repositories
{
    [TestFixture]
    public class TaxYearRepositoryTests
    {
        [Test]
        public async Task GetTaxYearAsync_Should_Search_By_Date()
        {
            //Arrange
            DateTime requestDate = new DateTime(2020, 02, 20);
            DateTime expectedFromDate = new DateTime(2020, 01, 01);
            DateTime expectedToDate = new DateTime(2020, 12, 01);
            const string expectedTaxYearName = "2020 Tax Year";

            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: GetType().Name + Guid.NewGuid())
                .Options;

            await using var context = new ApplicationContext(options);

            var taxYear = new TaxYear()
            {
                FromDate = expectedFromDate,
                ToDate = expectedToDate,
                Name = expectedTaxYearName
            };
            context.TaxYears.Add(taxYear);
            await context.SaveChangesAsync();

            _repository = new TaxYearRepository(context);

            //Act
            var entity = await _repository.GetTaxYearAsync(requestDate);

            //Assert
            Assert.AreEqual(expectedFromDate, entity.FromDate);
            Assert.AreEqual(expectedToDate, entity.ToDate);
            Assert.AreEqual(expectedTaxYearName, entity.Name);
        }

        [Test]
        public async Task GetTaxYearAsync_Should_Return_Null_If_Not_Exists()
        {
            //Arrange
            DateTime requestDate = new DateTime(2020, 02, 20);

            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: GetType().Name + Guid.NewGuid())
                .Options;

            await using var context = new ApplicationContext(options);

            _repository = new TaxYearRepository(context);

            //Act
            var entity = await _repository.GetTaxYearAsync(requestDate);

            //Assert
            Assert.IsNull(entity);
        }

        [Test]
        public async Task GetTaxYearAsync_Should_Throw_Exception_For_Duplicated_Years()
        {
            //Arrange
            DateTime requestDate = new DateTime(2020, 02, 20);
            DateTime expectedFromDate = new DateTime(2020, 01, 01);
            DateTime expectedToDate = new DateTime(2020, 12, 01);
            const string expectedTaxYearName = "2020 Tax Year";

            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: GetType().Name + Guid.NewGuid())
                .Options;

            await using var context = new ApplicationContext(options);

            var taxYear = new TaxYear()
            {
                FromDate = expectedFromDate,
                ToDate = expectedToDate,
                Name = expectedTaxYearName
            };
            context.TaxYears.Add(taxYear);

            var taxYear2 = new TaxYear()
            {
                FromDate = expectedFromDate,
                ToDate = expectedToDate,
                Name = expectedTaxYearName
            };
            context.TaxYears.Add(taxYear2);

            await context.SaveChangesAsync();

            _repository = new TaxYearRepository(context);

            //Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _repository.GetTaxYearAsync(requestDate));
        }

        private TaxYearRepository _repository;

    }
}
