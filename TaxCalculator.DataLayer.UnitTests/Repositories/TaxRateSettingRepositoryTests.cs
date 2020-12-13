using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TaxCalculator.DataLayer.DatabaseContexts;
using TaxCalculator.DataLayer.Entities;
using TaxCalculator.DataLayer.Repositories.Implementations;

namespace TaxCalculator.DataLayer.UnitTests.Repositories
{
    [TestFixture]
    public class TaxRateSettingRepositoryTests
    {

        public static IEnumerable<Action> TestCases()
        {
            yield return async () => await GetTaxYearAsync_Should_Search_By_Date<ProgressiveTaxRateSetting>();
            yield return async () => await GetTaxYearAsync_Should_Search_By_Date<FlatRateSetting>();
            yield return async () => await GetTaxYearAsync_Should_Search_By_Date<FlatValueSetting>();
        }

        [Test]
        [TestCaseSource(nameof(TestCases))]
        public void GetTaxYearAsync_Should_Search_By_Date(Action runTest)
        {
            runTest();
        }

        public static async Task GetTaxYearAsync_Should_Search_By_Date<TTaxSetting>() where  TTaxSetting : BaseTaxRateSetting, new()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: "TaxRateSettingRepositoryTests" + Guid.NewGuid())
                .Options;

            await using var context = new ApplicationContext(options);

            var taxYear = new TaxYear()
            {
                FromDate = new DateTime(2020, 01, 01),
                ToDate = new DateTime(2020, 12, 01),
                Name = "2020 Tax Year"
            };

            context.TaxYears.Add(taxYear);

            var taxRateSetting = new TTaxSetting
            {
                TaxYear = taxYear
            };

            context.Set<TTaxSetting>().Add(taxRateSetting);

            await context.SaveChangesAsync();

            var repository = new TaxRateSettingRepository<TTaxSetting>(context);

            //Act
            var entity = await repository.GetByTaxYearAsync(taxYear);

            //Assert
            CollectionAssert.AreEqual(new List<TTaxSetting> { taxRateSetting }, entity);
        }

    }
}
