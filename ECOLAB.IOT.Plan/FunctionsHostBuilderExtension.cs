namespace ECOLAB.IOT.Plan
{
    using AutoMapper;
    using ECOLAB.IOT.Plan.Common;
    using ECOLAB.IOT.Plan.Entity.Dtos.Sql;
    using ECOLAB.IOT.Plan.Entity.Entities.SqlServer;
    using ECOLAB.IOT.Plan.Entity.ScheduleDtos.SqlServer;
    using ECOLAB.IOT.Plan.Entity.SqlServer;
    using ECOLAB.IOT.Plan.Provider;
    using ECOLAB.IOT.Plan.Provider.Certification;
    using ECOLAB.IOT.Plan.Provider.HttpClient;
    using ECOLAB.IOT.Plan.Provider.Sql;
    using ECOLAB.IOT.Plan.Repository.Repositories.SqlServer;
    using ECOLAB.IOT.Plan.Service;
    using ECOLAB.IOT.Plan.Service.Certifiction;
    using ECOLAB.IOT.Plan.Service.Sql;
    using Microsoft.Azure.Functions.Extensions.DependencyInjection;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    internal static class FunctionsHostBuilderExtension
    {
        public static IFunctionsHostBuilder RegisterRepositories(this IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IELinkSqlServerRepository, ELinkSqlServerRepository>();
            builder.Services.AddSingleton<IPlanSqlServerRepository, PlanSqlServerRepository>();
            builder.Services.AddSingleton<ISqlTableClearScheduleRepository, SqlTableClearScheduleRepository>();
            builder.Services.AddSingleton<IUserWhiteListRepository, UserWhiteListRepository>();
            builder.Services.AddSingleton<IPlanRepository, PlanRepository>();
            builder.Services.AddSingleton<IELinkPlanHistoryRepository, ELinkPlanHistoryRepository>();
            builder.Services.AddSingleton<IELinkServerDBMappingTableRepository, ELinkServerDBMappingTableRepository>();
            return builder;
        }

        public static IFunctionsHostBuilder RegisterServices(this IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IELinkSqlServerService, ELinkSqlServerService>();
            builder.Services.AddSingleton<ISqlTableClearScheduleService, SqlTableClearScheduleService>();
            builder.Services.AddSingleton<ISqlPlanParserService, SqlPlanParserService>();
            builder.Services.AddSingleton<IUserWhiteListService, UserWhiteListService>();
            builder.Services.AddSingleton<ICertificationService, CertificationService>();
            builder.Services.AddSingleton<IPlanService, PlanService>();
            builder.Services.AddSingleton<ISqlPlanDispatcherService, SqlPlanDispatcherService>();
            builder.Services.AddSingleton<IELinkPlanHistoryService, ELinkPlanHistoryService>();
            builder.Services.AddSingleton<IELinkServerDBMappingTableService, ELinkServerDBMappingTableService>();
            return builder;
        }

        public static IFunctionsHostBuilder RegisterProviders(this IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<DateTimeMetricsProvider>();
            builder.Services.AddSingleton<IntMetricsProvider>();
            builder.Services.AddSingleton<ICertificationProvider, CertificationProvider>();
            builder.Services.AddSingleton<IECOLABIOTSecurityProvider, ECOLABIOTSecurityProvider>();
            builder.Services.AddSingleton<IMetricsProvider<WhereMetrics, DateTimeMetricsDto>, DateTimeMetricsProvider>();
            builder.Services.AddSingleton<IMetricsProvider<WhereMetrics, IntMetricsDto>, IntMetricsProvider>();
            builder.Services.AddSingleton<IDMPFluentHttpProvider, DMPFluentHttpProvider>();

            builder.Services.AddSingleton<CustomPolicyProvider<DateTimeMetricsDto>>();
            builder.Services.AddSingleton<PartialMatchPolicyProvider<DateTimeMetricsDto>>();
            builder.Services.AddSingleton<DynamicPolicyProvider<DateTimeMetricsDto>>();
            builder.Services.AddSingleton(implementationFactory =>
            {
                Func<string, IPolicyProvider<ClearTable, PolicyDto<DateTimeMetricsDto>>> accesor = key =>
                {
                    if (key.Equals("Custom"))
                    {
                        return implementationFactory.GetService<CustomPolicyProvider<DateTimeMetricsDto>>();
                    }
                    else if (key.Equals("PartialMatch"))
                    {
                        return implementationFactory.GetService<PartialMatchPolicyProvider<DateTimeMetricsDto>>();
                    }
                    else if (key.Equals("Dynamic"))
                    {
                        return implementationFactory.GetService<DynamicPolicyProvider<DateTimeMetricsDto>>();
                    }
                    else
                    {
                        throw new ArgumentException($"Not Support key : {key}");
                    }
                };
                return accesor;
            });


            builder.Services.AddSingleton<CustomPolicyProvider<IntMetricsDto>>();
            builder.Services.AddSingleton<PartialMatchPolicyProvider<IntMetricsDto>>();
            builder.Services.AddSingleton<DynamicPolicyProvider<IntMetricsDto>>();
            builder.Services.AddSingleton(implementationFactory =>
            {
                Func<string, IPolicyProvider<ClearTable, PolicyDto<IntMetricsDto>>> accesor = key =>
                {

                    if (key.Equals("Custom"))
                    {
                        return implementationFactory.GetService<CustomPolicyProvider<IntMetricsDto>>();
                    }
                    else if (key.Equals("PartialMatch"))
                    {
                        return implementationFactory.GetService<PartialMatchPolicyProvider<IntMetricsDto>>();
                    }
                    else if (key.Equals("Dynamic"))
                    {
                        return implementationFactory.GetService<DynamicPolicyProvider<IntMetricsDto>>();
                    }
                    else
                    {
                        throw new ArgumentException($"Not Support key : {key}");
                    }
                };
                return accesor;
            });


            return builder;
        }

        public static IFunctionsHostBuilder RegisterAutoMapper(this IFunctionsHostBuilder builder)
        {
            var mappperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ELINKSqlServerDto, ELinkSqlServer>();
                cfg.CreateMap<UserWhiteListDto, UserWhiteList>();
                //cfg.CreateMap<SqlTableClearScheduleDto, SqlTableClearSchedule>();
                //cfg.CreateMap<List<SqlTableClearScheduleDto>, List<SqlTableClearSchedule>>();
            });

            builder.Services.AddSingleton(mappperConfig.CreateMapper());

            return builder;
        }

        public static IFunctionsHostBuilder Configure(this IFunctionsHostBuilder builder)
        {

            //var configuration = BuildConfiguration(builder.GetContext().ApplicationRootPath);
            //builder.Services.Configure<MyServerOptions>(configuration.GetSection(nameof(MyServerOptions)));
            //return builder;

            string basePath = IsDevelopmentEnvironment() ?
                 Environment.GetEnvironmentVariable("AzureWebJobsScriptRoot") :
                 $"{Environment.GetEnvironmentVariable("HOME")}\\site\\wwwroot";
          
            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)  // common settings go here.
                .AddJsonFile("appsettings.local.json", optional: false, reloadOnChange: true)  // secrets go here. This file is excluded from source control.
                //.AddJsonFile($"appsettings.dev.json", optional: true, reloadOnChange: true)  // environment specific settings go here
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT")}.json", optional: true, reloadOnChange: true)  // environment specific settings go here
                .Build();

            builder.Services.AddSingleton<IConfiguration>(config);

            return builder;
        }

        public static bool IsDevelopmentEnvironment()
        {
            return "Development".Equals(Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT"), StringComparison.OrdinalIgnoreCase);
        }

        public static void Build(this IFunctionsHostBuilder builder)
        {
            CallerContextBase.Container = builder.Services.BuildServiceProvider();
        }
    }
}
