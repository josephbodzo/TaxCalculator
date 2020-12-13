using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using TaxCalculator.API.Models;
using TaxCalculator.Business.Factories;
using TaxCalculator.Business.Managers;
using TaxCalculator.Business.Models;
using TaxCalculator.Business.ValidationRules.TaxCalculation;
using TaxCalculator.Common.Services;
using TaxCalculator.Common.Services.Implementations;
using TaxCalculator.Common.ValidationRuleEngines;
using TaxCalculator.Common.ValidationRuleEngines.Implementations;
using TaxCalculator.DataLayer.DatabaseContexts;
using TaxCalculator.DataLayer.Entities;
using TaxCalculator.DataLayer.Repositories;
using TaxCalculator.DataLayer.Repositories.Implementations;
using TaxCalculator.DataLayer.Repositories.Implementations.CachedImplementations;

namespace TaxCalculator.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<TaxCalculationRequestModelValidator>());

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TaxCalculator", Version = "v1" });
            });
            services.AddSingleton<IClock, Clock>();

            services.AddDbContext<ApplicationContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("ApplicationContext")));

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddMemoryCache();

            //Business Managers
            services.AddScoped<ITaxCalculatorManager, TaxCalculatorManager>();
            services.AddScoped<ITaxCalculatorFactory, TaxCalculatorFactory>();

            //Business Rules
            services.AddScoped<IValidationRuleEngine<TaxCalculationRequest, TaxCalculationResponse>, ValidationRuleEngine<TaxCalculationRequest, TaxCalculationResponse>>();
            services.AddScoped<IValidationRule<TaxCalculationRequest, TaxCalculationResponse>, PostalCodeValidationRule>();
            services.AddScoped<IValidationRule<TaxCalculationRequest, TaxCalculationResponse>, TaxYearValidationRule>();

            //Repositories
            services.AddScoped<IPostalCodeTaxCalculationMappingRepository, CachedPostalCodeTaxCalculationMappingRepository>();
            services.AddScoped<PostalCodeTaxCalculationMappingRepository>();
            services.AddScoped<ITaxCalculationRepository, TaxCalculationRepository>();
            services.AddScoped<ITaxYearRepository, CachedTaxYearRepository>();
            services.AddScoped<TaxYearRepository>();
            services.AddScoped<ITaxRateSettingRepository<FlatRateSetting>, CachedTaxRateSettingRepository<FlatRateSetting>>();
            services.AddScoped<TaxRateSettingRepository<FlatRateSetting>>();
            services.AddScoped<ITaxRateSettingRepository<FlatValueSetting>, CachedTaxRateSettingRepository<FlatValueSetting>>();
            services.AddScoped<TaxRateSettingRepository<FlatValueSetting>>();
            services.AddScoped<ITaxRateSettingRepository<ProgressiveTaxRateSetting>, CachedTaxRateSettingRepository<ProgressiveTaxRateSetting>>();
            services.AddScoped<TaxRateSettingRepository<ProgressiveTaxRateSetting>>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaxCalculator v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
