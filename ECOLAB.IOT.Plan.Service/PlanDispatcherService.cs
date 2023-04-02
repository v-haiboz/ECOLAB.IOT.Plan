namespace ECOLAB.IOT.Plan.Service
{
    using ECOLAB.IOT.Plan.Entity.ScheduleDtos;
    using ECOLAB.IOT.Plan.Entity.ScheduleDtos.SqlServer;
    using ECOLAB.IOT.Plan.Repository.Repositories.SqlServer;
    using ECOLAB.IOT.Plan.Service.Sql;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;

    public interface ISqlPlanDispatcherService
    {
        public void Execute(List<ClearPlan> clearPlans,DateTime expiryTime);
    }

    public class SqlPlanDispatcherService : ISqlPlanDispatcherService
    {
        public IPlanService _planService;
        private readonly IELinkPlanHistoryService _eLinkPlanHistoryService;
        private readonly IELinkServerDBMappingTableRepository _eLinkServerDBMappingTableRepository;
        public SqlPlanDispatcherService(IPlanService planService, IELinkPlanHistoryService eLinkPlanHistoryService, IELinkServerDBMappingTableRepository eLinkServerDBMappingTableRepository)
        {
            _planService = planService;
            _eLinkPlanHistoryService = eLinkPlanHistoryService;
            _eLinkServerDBMappingTableRepository = eLinkServerDBMappingTableRepository;
        }

        public void Execute(List<ClearPlan> clearPlans, DateTime expiryTime)
        {
            var startTime = DateTime.UtcNow;
            var traceGuid = Guid.NewGuid().ToString();
            try
            {
                var traceLog = new ELinkPlanHistoryDto()
                {
                    Category = ELinkPlanHistoryCategory.ExecuteClearPlan,
                    Message = $"Execute Plan Delete Action Start. Start Time:{startTime} trace Guid:{traceGuid} ",
                };

                _eLinkPlanHistoryService.WriteInfoMessage(traceLog);
                var options = new ParallelOptions() { MaxDegreeOfParallelism = 8 };
                Parallel.ForEach(clearPlans, options, (clearPlan, parallelLoopState) =>
                {
                    var stopWatch = new Stopwatch();
                    try
                    {
                        if (DateTime.UtcNow >= expiryTime)
                        {
                            parallelLoopState.Break();
                            return;
                        }

                        var plans = clearPlan.GetPlans();
                        var connectionStr = plans.Key;
                        foreach (var sqlPlan in plans.Value)
                        {
                            if (!_eLinkServerDBMappingTableRepository.IsFinish(clearPlan.ClearServer.ServerName, clearPlan.ClearServer.DBName, sqlPlan.TableName))
                            {
                                _eLinkPlanHistoryService.WriteInfoMessage(new ELinkPlanHistoryDto()
                                {
                                    Category = ELinkPlanHistoryCategory.ExecuteClearPlan,
                                    Message = $"Did {clearPlan.ClearServer.ServerName}:{clearPlan.ClearServer.DBName}:{sqlPlan.TableName} complete the first backup?",
                                    TargetRowData = JsonConvert.SerializeObject(sqlPlan),
                                    SourceRowData = JsonConvert.SerializeObject(clearPlan),
                                });

                                continue;
                            }
                            stopWatch.Restart();
                            var result=_planService.ExecutePlan(sqlPlan, connectionStr, expiryTime);
                            stopWatch.Stop();
                            var traceLog = new ELinkPlanHistoryDto()
                            {
                                Category = ELinkPlanHistoryCategory.ExecuteClearPlan,
                                Message =  result.Tag==1? $"Execute {sqlPlan.TableName} successful trace Guid:{traceGuid}. {result.Description} Execution time:{stopWatch.Elapsed} Execution Sql:{ sqlPlan.Sql}": $"Execute {sqlPlan.TableName} failed trace Guid:{traceGuid}.{result.Description} Execution time:{stopWatch.Elapsed} Execution Sql:{sqlPlan.Sql}",
                                TargetRowData = JsonConvert.SerializeObject(sqlPlan),
                                SourceRowData = JsonConvert.SerializeObject(clearPlan),
                            };

                            _eLinkPlanHistoryService.WriteInfoMessage(traceLog);
                        }
                    }
                    catch (Exception ex)
                    {
                        stopWatch.Stop();
                        var traceLog = new ELinkPlanHistoryDto()
                        {
                            Category = ELinkPlanHistoryCategory.ExecuteClearPlan,
                            Message = $"trace Guid:{traceGuid}.{ex.Message}",
                            SourceRowData = JsonConvert.SerializeObject(clearPlan),
                        };

                        _eLinkPlanHistoryService.WriteErrorMessage(traceLog);
                    }
                    finally
                    {
                        stopWatch.Stop();
                    }
                });

                var endTime = DateTime.UtcNow;
                var totalTime = endTime - startTime;
                traceLog = new ELinkPlanHistoryDto()
                {
                    Category = ELinkPlanHistoryCategory.ExecuteClearPlan,
                    Message = $"Execute Plan DeleteAction successful End Time:{endTime} trace Guid:{traceGuid}",
                    TargetRowData = $"Total Time:{totalTime}"
                };

                _eLinkPlanHistoryService.WriteInfoMessage(traceLog);
            }
            catch (Exception ex)
            {
                var endTime = DateTime.UtcNow;
                var totalTime = endTime - startTime;
                var traceLog = new ELinkPlanHistoryDto()
                {
                    Message = $"Execute Plan DeleteAction failed End Time:{endTime} trace Guid:{traceGuid}",
                    TargetRowData = $"Total Time:{totalTime}"
                };

                _eLinkPlanHistoryService.WriteInfoMessage(traceLog);
            }
        }
    }
}
