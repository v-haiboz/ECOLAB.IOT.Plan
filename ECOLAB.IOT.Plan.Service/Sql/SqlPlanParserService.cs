namespace ECOLAB.IOT.Plan.Service.Sql
{
    using ECOLAB.IOT.Plan.Entity;
    using ECOLAB.IOT.Plan.Entity.Dtos.Sql;
    using ECOLAB.IOT.Plan.Entity.ScheduleDtos;
    using ECOLAB.IOT.Plan.Entity.ScheduleDtos.SqlServer;
    using ECOLAB.IOT.Plan.Entity.SqlServer;
    using ECOLAB.IOT.Plan.Provider;
    using ECOLAB.IOT.Plan.Repository.Repositories.SqlServer;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using XAct;

    public interface ISqlPlanParserService
    {
        public Task<List<ClearPlan>> Execute();
        public Task<ClearPlan> BuildClearPlan(ELinkSqlServer eLINKSql);
        public Task<List<ClearPlan>> BuildClearPlans(List<ELinkSqlServer> eLinkSqlServers);

    }

    public class SqlPlanParserService : ISqlPlanParserService
    {
        private readonly Func<string, IPolicyProvider<ClearTable, PolicyDto<DateTimeMetricsDto>>> _policyDateTimeMetricsProviders;
        private readonly Func<string, IPolicyProvider<ClearTable, PolicyDto<IntMetricsDto>>> _policyIntMetricsProviders;
        private readonly IELinkSqlServerRepository _eLINKSqlServerRepository;
        private readonly IELinkPlanHistoryService _eLinkPlanHistoryService;

        public SqlPlanParserService(Func<string, IPolicyProvider<ClearTable, PolicyDto<DateTimeMetricsDto>>> policyDateTimeMetricsProviders,
            Func<string, IPolicyProvider<ClearTable, PolicyDto<IntMetricsDto>>> policyIntMetricsProviders,
            IELinkSqlServerRepository eLINKSqlServerRepository,
            IELinkPlanHistoryService eLinkPlanHistoryService)
        {
            _policyDateTimeMetricsProviders = policyDateTimeMetricsProviders;
            _policyIntMetricsProviders = policyIntMetricsProviders;
            _eLINKSqlServerRepository = eLINKSqlServerRepository;
            _eLinkPlanHistoryService = eLinkPlanHistoryService;
        }

        public async Task<List<ClearPlan>> Execute()
        {
            var list = _eLINKSqlServerRepository.GetDetailList();
            var clearPlans = new List<ClearPlan>();
            foreach (var item in list)
            {
                try
                {
                    if (item != null)
                    {
                        var clearPlan = await BuildClearPlan(item);
                        clearPlans.Add(clearPlan);
                        var traceLog = new ELinkPlanHistoryDto()
                        {
                            Category= ELinkPlanHistoryCategory.BuildClearPlan,
                            Message = "Build ClearPlan Successful",
                            TargetRowData = JsonConvert.SerializeObject(clearPlan),
                            SourceRowData = JsonConvert.SerializeObject(item),
                        };
                        _eLinkPlanHistoryService.WriteInfoMessage(traceLog);
                    }
                }
                catch (Exception ex)
                {
                    var traceLog = new ELinkPlanHistoryDto()
                    {
                        Category = ELinkPlanHistoryCategory.BuildClearPlan,
                        Message = ex.Message,
                        SourceRowData = JsonConvert.SerializeObject(item),
                    };

                    _eLinkPlanHistoryService.WriteErrorMessage(traceLog);
                }
            }

            return clearPlans;
        }

        public async Task<List<ClearPlan>> BuildClearPlans(List<ELinkSqlServer> eLinkSqlServers)
        {
            var list = new List<ClearPlan>();
            if (eLinkSqlServers == null)
            {
                return list;
            }

            foreach (var server in eLinkSqlServers)
            {
                var clearPlan = await BuildClearPlan(server);
                list.Add(clearPlan);
            }

            return list;
        }

        public async Task<ClearPlan> BuildClearPlan(ELinkSqlServer eLINKSql)
        {
            var clearPlan = new ClearPlan();
            clearPlan.ClearServer = new ClearServer()
            {
                DBName = eLINKSql.DBName,
                Password = eLINKSql.Password,
                ServerName = eLINKSql.ServerName,
                UserId = eLINKSql.UserId
            };

            clearPlan.ClearTables = new HashSet<ClearTable>(new ClearTableComparer());
            foreach (var item in eLINKSql.SqlTableClearSchedules)
            {
                switch (item.Type)
                {
                    case ClearScheduleType.CustomDateTimeMetrics:
                        {
                            var instance = JsonConvert.DeserializeObject<PolicyDto<DateTimeMetricsDto>>(item.JObject);
                            var clearTables = await _policyDateTimeMetricsProviders("Custom").Resolve(instance, eLINKSql);
                            RecordClearPlan(clearPlan, clearTables);
                            clearPlan.ClearTables.UnionWith(clearTables);
                        }
                        break;
                    case ClearScheduleType.CustomIntMetrics:
                        {
                            var instance = JsonConvert.DeserializeObject<PolicyDto<IntMetricsDto>>(item.JObject);
                            var clearTables = await _policyIntMetricsProviders("Custom").Resolve(instance, eLINKSql);
                            RecordClearPlan(clearPlan, clearTables);
                            clearPlan.ClearTables.UnionWith(clearTables);
                        }

                        break;
                    case ClearScheduleType.PartialMatchDateTimMetrics:
                        {
                            var instance = JsonConvert.DeserializeObject<PolicyDto<DateTimeMetricsDto>>(item.JObject);
                            var clearTables = await _policyDateTimeMetricsProviders("PartialMatch").Resolve(instance, eLINKSql, clearPlan.ClearServer);
                            RecordClearPlan(clearPlan, clearTables);
                            clearPlan.ClearTables.UnionWith(clearTables);
                        }
                        break;
                    case ClearScheduleType.PartialMatchIntMetrics:
                        {
                            var instance = JsonConvert.DeserializeObject<PolicyDto<IntMetricsDto>>(item.JObject);
                            var clearTables = await _policyIntMetricsProviders("PartialMatch").Resolve(instance, eLINKSql, clearPlan.ClearServer);
                            RecordClearPlan(clearPlan, clearTables);
                            clearPlan.ClearTables.UnionWith(clearTables);
                        }
                        break;
                    case ClearScheduleType.DynamicDateTimeMetrics:
                        {
                            var instance = JsonConvert.DeserializeObject<PolicyDto<DateTimeMetricsDto>>(item.JObject);
                            var clearTables = await _policyDateTimeMetricsProviders("Dynamic").Resolve(instance, eLINKSql, clearPlan.ClearServer);
                            RecordClearPlan(clearPlan, clearTables);
                            clearPlan.ClearTables.UnionWith(clearTables);
                        }
                        break;
                    case ClearScheduleType.DynamicIntMetrics:
                        {
                            var instance = JsonConvert.DeserializeObject<PolicyDto<IntMetricsDto>>(item.JObject);
                            var clearTables = await _policyIntMetricsProviders("Dynamic").Resolve(instance, eLINKSql, clearPlan.ClearServer);
                            RecordClearPlan(clearPlan, clearTables);
                            clearPlan.ClearTables.UnionWith(clearTables);
                        }
                        break;
                }
            }

            return await Task.FromResult(clearPlan);
        }

        private void RecordClearPlan(ClearPlan clearPlan, HashSet<ClearTable> clearTables)
        {
            if (clearPlan.TableClearScheduleRecordDtos == null || clearTables==null)
            {
                return;
            }

            foreach (var clearTable in clearTables)
            {
                if (clearPlan.ClearTables.Contains(clearTable))
                {
                    clearPlan.TableClearScheduleRecordDtos.Add(new TableClearScheduleRecordDto() { ClearTable = clearTable, Enable = false });
                }
                else
                {
                    clearPlan.TableClearScheduleRecordDtos.Add(new TableClearScheduleRecordDto() { ClearTable = clearTable, Enable = true });
                }
            }
        }
    }
}
