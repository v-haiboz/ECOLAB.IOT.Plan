using ECOLAB.IOT.Plan.Entity.SqlServer;

namespace ECOLAB.IOT.Plan.Provider.Sql
{
    using ECOLAB.IOT.Plan.Entity;
    using ECOLAB.IOT.Plan.Entity.Dtos.Sql;
    using ECOLAB.IOT.Plan.Entity.ScheduleDtos.SqlServer;
    using ECOLAB.IOT.Plan.Entity.SqlServer;
    using Newtonsoft.Json;

    public class CustomPolicyProvider<Metrics> : IPolicyProvider<ClearTable, PolicyDto<Metrics>>
    {
        private readonly IMetricsProvider<WhereMetrics, DateTimeMetricsDto> _dateTimeMetricsProvider;
        private readonly IMetricsProvider<WhereMetrics, IntMetricsDto> _intMetricsProvider;

        public CustomPolicyProvider(IMetricsProvider<WhereMetrics, DateTimeMetricsDto> dateTimeMetricsProvider, IMetricsProvider<WhereMetrics, IntMetricsDto> intMetricsProvider)
        {
            _dateTimeMetricsProvider = dateTimeMetricsProvider;
            _intMetricsProvider = intMetricsProvider;
        }

        public async Task<HashSet<ClearTable>> Resolve(PolicyDto<Metrics> customPolicyDto, ELinkSqlServer? eLinkSqlServer, ClearServer clearServer = null)
        {
            if (customPolicyDto == null)
            {
                throw new Exception("policy can't be empty.");
            }

            var typeName = typeof(Metrics).Name;
            if (string.Equals(typeName, "DateTimeMetricsDto", StringComparison.Ordinal))
            {
                return await DateTimeMetricsResolve(customPolicyDto as PolicyDto<DateTimeMetricsDto>, eLinkSqlServer);

            }
            else if (string.Compare(typeName, "IntMetricsDto") == 0)
            {
                return await IntMetricsResolve(customPolicyDto as PolicyDto<IntMetricsDto>, eLinkSqlServer);
            }

            return await Task.FromResult(new HashSet<ClearTable>());
        }

        private async Task<HashSet<ClearTable>> DateTimeMetricsResolve(PolicyDto<DateTimeMetricsDto> policyDto, ELinkSqlServer? eLinkSqlServer = null)
        {
            if (policyDto == null)
            {
                throw new Exception("DateTimeMetricsDto can't be empty.");
            }

            policyDto.Validate();

            var clearTable = new ClearTable()
            {
                TableName = policyDto.TableName,
                Policy = new Policy()
                {
                    Frequency = policyDto.Frequency,
                    Count = policyDto.Count
                },
                ClearScheduleType = ClearScheduleType.CustomDateTimeMetrics,
                ELinkSqlServer = eLinkSqlServer,
                TableNamePattern = policyDto.TableName,
                SourcePolicyMetrics = JsonConvert.SerializeObject(policyDto),
                WhereMetrics = _dateTimeMetricsProvider.Resolve(policyDto.Where)
            };

            return await Task.FromResult(new HashSet<ClearTable> { clearTable });
        }

        private async Task<HashSet<ClearTable>> IntMetricsResolve(PolicyDto<IntMetricsDto> policyDto, ELinkSqlServer? eLinkSqlServer = null)
        {
            if (policyDto == null)
            {
                throw new Exception("IntMetricsDto can't be empty.");
            }

            policyDto.Validate();

            var clearTable = new ClearTable()
            {
                TableName = policyDto.TableName,
                Policy = new Policy()
                {
                    Frequency = policyDto.Frequency,
                    Count = policyDto.Count
                },
                ClearScheduleType = ClearScheduleType.CustomIntMetrics,
                ELinkSqlServer = eLinkSqlServer,
                TableNamePattern = policyDto.TableName,
                SourcePolicyMetrics = JsonConvert.SerializeObject(policyDto),
                WhereMetrics = _intMetricsProvider.Resolve(policyDto.Where)
            };

            return await Task.FromResult(new HashSet<ClearTable> { clearTable });
        }
    }
}
