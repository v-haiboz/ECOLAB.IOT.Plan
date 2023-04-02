namespace ECOLAB.IOT.Plan.Provider.Sql
{
    using ECOLAB.IOT.Plan.Entity;
    using ECOLAB.IOT.Plan.Entity.Dtos.Sql;
    using ECOLAB.IOT.Plan.Entity.ScheduleDtos.SqlServer;
    using ECOLAB.IOT.Plan.Entity.SqlServer;
    using ECOLAB.IOT.Plan.Provider.HttpClient;
    using ECOLAB.IOT.Plan.Repository.Repositories.SqlServer;
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;

    public class DynamicPolicyProvider<Metrics> : IPolicyProvider<ClearTable, PolicyDto<Metrics>>
    {
        private readonly IMetricsProvider<WhereMetrics, DateTimeMetricsDto> _dateTimeMetricsProvider;
        private readonly IMetricsProvider<WhereMetrics, IntMetricsDto> _intMetricsProvider;
        private readonly IPlanRepository _planRepository;

        public DynamicPolicyProvider(IMetricsProvider<WhereMetrics, DateTimeMetricsDto> dateTimeMetricsProvider,
            IMetricsProvider<WhereMetrics, IntMetricsDto> intMetricsProvider,
            IDMPFluentHttpProvider dmpFluentHttpProvider,
            IPlanRepository planRepository)
        {
            _dateTimeMetricsProvider = dateTimeMetricsProvider;
            _intMetricsProvider = intMetricsProvider;
            _planRepository = planRepository;
        }

        public async Task<HashSet<ClearTable>> Resolve(PolicyDto<Metrics> policyDto, ELinkSqlServer? eLinkSqlServer = null, ClearServer clearServer = null)
        {
            if (clearServer == null)
            {
                throw new Exception("clearServer can't be empty.");
            }

            if (policyDto == null)
            {
                throw new Exception("policy can't be empty.");
            }

            var typeName = typeof(Metrics).Name;
            if (string.Equals(typeName, "DateTimeMetricsDto", StringComparison.Ordinal))
            {
                return await DateTimeMetricsResolve(policyDto as PolicyDto<DateTimeMetricsDto>, eLinkSqlServer, clearServer);
            }
            else if (string.Compare(typeName, "IntMetricsDto") == 0)
            {
                return await IntMetricsResolve(policyDto as PolicyDto<IntMetricsDto>, eLinkSqlServer, clearServer);
            }

            return null;
        }
        private async Task<HashSet<ClearTable>> DateTimeMetricsResolve(PolicyDto<DateTimeMetricsDto>? policyDto, ELinkSqlServer? eLinkSqlServer, ClearServer clearServer = null)
        {
            if (clearServer == null)
            {
                throw new Exception("clearServer can't be empty.");
            }

            if (policyDto == null)
            {
                throw new Exception("DateTimeMetricsDto can't be empty.");
            }

            policyDto.Validate();

            var tableNames = _planRepository.GetUserTableNameInDB(clearServer.ConnectionStr);
            var clearTables = new HashSet<ClearTable>(new ClearTableComparer());
            foreach (var tableName in tableNames)
            {
                var clearTable = new ClearTable()
                {
                    TableName = tableName,
                    
                    Policy = new Policy()
                    {
                        Frequency = policyDto.Frequency,
                        Count = policyDto.Count
                    },
                    ELinkSqlServer = eLinkSqlServer,
                    TableNamePattern = policyDto.TableName,
                    SourcePolicyMetrics = JsonConvert.SerializeObject(policyDto),
                    ClearScheduleType = ClearScheduleType.DynamicDateTimeMetrics,
                    WhereMetrics = _dateTimeMetricsProvider.Resolve(policyDto.Where)
                };

                clearTables.Add(clearTable);
            }

            return await Task.FromResult(clearTables);
        }
        private async Task<HashSet<ClearTable>> IntMetricsResolve(PolicyDto<IntMetricsDto>?  policyDto, ELinkSqlServer? eLinkSqlServer, ClearServer clearServer = null)
        {
            if (clearServer == null)
            {
                throw new Exception("clearServer can't be empty.");
            }

            if (policyDto == null)
            {
                throw new Exception("DateTimeMetricsDto can't be empty.");
            }

            policyDto.Validate();

            var tableNames = _planRepository.GetUserTableNameInDB(clearServer.ConnectionStr);
            var clearTables = new HashSet<ClearTable>(new ClearTableComparer());
            foreach (var tableName in tableNames)
            {
                var clearTable = new ClearTable()
                {
                    TableName = tableName,

                    Policy = new Policy()
                    {
                        Frequency = policyDto.Frequency,
                        Count = policyDto.Count
                    },
                    ELinkSqlServer = eLinkSqlServer,
                    TableNamePattern = policyDto.TableName,
                    ClearScheduleType = ClearScheduleType.DynamicDateTimeMetrics,
                    SourcePolicyMetrics = JsonConvert.SerializeObject(policyDto),
                    WhereMetrics = _intMetricsProvider.Resolve(policyDto.Where)
                };

                clearTables.Add(clearTable);
            }

            return await Task.FromResult(clearTables);
        }

       
    }
}
