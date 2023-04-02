using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(ECOLAB.IOT.Plan.Startup))]
namespace ECOLAB.IOT.Plan
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var build = builder.RegisterRepositories()
            .RegisterRepositories()
            .RegisterProviders()
            .RegisterServices()
            .RegisterAutoMapper()
            .Configure();
        }


        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            var config = builder.ConfigurationBuilder.Build();
            var connectionString = config.GetConnectionString("SqlConnectionString");
        }
    }
}