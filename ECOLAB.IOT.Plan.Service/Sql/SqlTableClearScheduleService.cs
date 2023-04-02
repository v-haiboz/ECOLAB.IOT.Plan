namespace ECOLAB.IOT.Plan.Service.Sql
{
    using AutoMapper;
    using ECOLAB.IOT.Plan.Common.Utilities;
    using ECOLAB.IOT.Plan.Entity;
    using ECOLAB.IOT.Plan.Entity.Dtos.Sql;
    using ECOLAB.IOT.Plan.Entity.Entities.SqlServer;
    using ECOLAB.IOT.Plan.Entity.ScheduleDtos.SqlServer;
    using ECOLAB.IOT.Plan.Repository.Repositories.SqlServer;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using XAct;
    using static System.Collections.Specialized.BitVector32;

    public interface ISqlTableClearScheduleService
    {
        public Task<TData> Insert(string json);

        public Task<TData> Update(string json);

        public Task<TData> EnableOrDisable(SqlTableClearScheduleEnableOrDisableDto sqlTableClearScheduleEnableOrDisableDto);

        public TData Delete(SqlTableClearScheduleDeleteDto sqlTableClearScheduleDto);
    }

    public class SqlTableClearScheduleService : ISqlTableClearScheduleService
    {
        private readonly IELinkSqlServerRepository _eLINKSqlServerRepository;
        private readonly ISqlTableClearScheduleRepository _sqlTableClearScheduleRepository;
        private readonly IPlanRepository _planRepository;
        private readonly ISqlPlanParserService _sqlPlanParserService;
        private readonly IELinkServerDBMappingTableService _eLinkServerDBMappingTableService;
        private readonly IMapper _mapper;

        public SqlTableClearScheduleService(IELinkSqlServerRepository eLINKSqlServerRepository,
            ISqlTableClearScheduleRepository sqlTableClearScheduleRepository,
            ISqlPlanParserService sqlPlanParserService,
            IELinkServerDBMappingTableService eLinkServerDBMappingTableService,
            IMapper mapper,
            IPlanRepository planRepository)
        {
            _eLINKSqlServerRepository = eLINKSqlServerRepository;
            _sqlTableClearScheduleRepository = sqlTableClearScheduleRepository;
            _planRepository = planRepository;
            _sqlPlanParserService = sqlPlanParserService;
            _eLinkServerDBMappingTableService = eLinkServerDBMappingTableService;
            _mapper = mapper;
        }

        public async Task<TData> Insert(string json)
        {
            var result = new TData();
            try
            {
                if (string.IsNullOrEmpty(json))
                {
                    ;
                    result.Message = $"request body can't be empty.";
                }

                var item = JsonConvert.DeserializeObject<SqlTableClearScheduleDto<dynamic>>(json, Utility.setting);
                if (item == null || item.Policy == null || item.Policy.Where == null)
                {
                    return await Task.FromResult(new TData() { Message = $"sqlTableClearScheduleDto's parameters are incomplete, pls double check." });
                }

                var metricsStr = JsonConvert.SerializeObject(item.Policy.Where);

                if (CheckMetricsType(metricsStr, "holdDays"))
                {
                    var datetimeDto = JsonConvert.DeserializeObject<SqlTableClearScheduleDto<DateTimeMetricsDto>>(json, Utility.setting);
                    return await Insert(datetimeDto);
                }
                else if (CheckMetricsType(metricsStr, "Value"))
                {
                    var intDto = JsonConvert.DeserializeObject<SqlTableClearScheduleDto<IntMetricsDto>>(json, Utility.setting);
                    return await Insert(intDto);
                }
                else
                {
                    result.Message = $"There is no metrics of this type";
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                return result;
            }
        }

        private bool CheckMetricsType(string json, string propertyKey)
        {
            JObject jObject = JObject.Parse(json);
            bool hasCode = jObject.Properties().Any(p => p.Name.ToLowerInvariant() == propertyKey.ToLowerInvariant());
            return hasCode;
        }

        private string GetSqlColumnTypeNameByMetricsType<M>()
        {
            var metricsType = typeof(M).Name;
            if (metricsType == "DateTimeMetricsDto")
            {
                return $"datetime2,datetime";
            }

            if (metricsType == "IntMetricsDto")
            {
                return "int";
            }

            return string.Empty;
        }

        public async Task<TData> Update(string json)
        {
            var result = new TData();
            try
            {
                if (string.IsNullOrEmpty(json))
                {
                    ;
                    result.Message = $"request body can't be empty.";
                }

                var item = JsonConvert.DeserializeObject<SqlTableClearScheduleDto<dynamic>>(json, Utility.setting);
                if (item == null || item.Policy == null || item.Policy.Where == null)
                {
                    return await Task.FromResult(new TData() { Message = $"sqlTableClearScheduleDto's parameters are incomplete" });
                }

                var metricsStr = JsonConvert.SerializeObject(item.Policy.Where);

                if (CheckMetricsType(metricsStr, "holdDays"))
                {
                    var datetimeDto = JsonConvert.DeserializeObject<SqlTableClearScheduleDto<DateTimeMetricsDto>>(json, Utility.setting);
                    return await Update(datetimeDto);
                }
                else if (CheckMetricsType(metricsStr, "Value"))
                {
                    var intDto = JsonConvert.DeserializeObject<SqlTableClearScheduleDto<IntMetricsDto>>(json, Utility.setting);
                    return await Update(intDto);
                }
                else
                {
                    result.Message = $"There is no metrics of this type";
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                return result;
            }
        }

        public TData Delete(SqlTableClearScheduleDeleteDto sqlTableClearScheduleDto)
        {
            var result = new TData();
            try
            {
                if (sqlTableClearScheduleDto == null || string.IsNullOrEmpty(sqlTableClearScheduleDto.ServerName) || string.IsNullOrEmpty(sqlTableClearScheduleDto.DBName))
                {
                    result.Message = "ServerName And DBName cannot be empty.";
                    return result;
                }

                var eLINKSqlServer = _eLINKSqlServerRepository.GetELINKSqlServerByServerNameAndDBName(sqlTableClearScheduleDto.ServerName, sqlTableClearScheduleDto.DBName);
                if (eLINKSqlServer == null)
                {
                    result.Message = $"{sqlTableClearScheduleDto.ServerName} And {sqlTableClearScheduleDto.DBName} doesn't exist.";
                    return result;
                }

                var sqlTableClearSchedule = sqlTableClearScheduleDto.ToCovertSqlTableClearSchedule(); ;
                sqlTableClearSchedule.SqlServerId = eLINKSqlServer.Id;

                var schedule = _sqlTableClearScheduleRepository.GetSqlTableClearScheduleByPartitionKeyAndServerId(sqlTableClearSchedule.PartitionKey, sqlTableClearSchedule.SqlServerId);
                if (schedule == null)
                {
                    result.Message = $"This PolicySchedule doesn't exist.{sqlTableClearSchedule?.JObject}";
                    return result;
                }

                sqlTableClearSchedule.Id = schedule.Id;


                var bl = _sqlTableClearScheduleRepository.Delete(sqlTableClearSchedule);
                result.Tag = bl ? 1 : 0;
                if (bl)
                    result.Message = "SqlTableClearSchedule delete successful.";
                else
                    result.Message = "SqlTableClearSchedule delete failed.";

                return result;
            }
            catch (Exception ex)
            {
                result.Message = "SqlTableClearSchedule delete failed.";
                result.Message = ex.Message;
                return result;
            }
        }

        private async Task<TData> Insert<M>(SqlTableClearScheduleDto<M>? sqlTableClearScheduleDto)
        {
            var metricsType = typeof(M).Name;

            var result = new TData();
            try
            {
                if (sqlTableClearScheduleDto == null || string.IsNullOrEmpty(sqlTableClearScheduleDto.ServerName) || string.IsNullOrEmpty(sqlTableClearScheduleDto.DBName))
                {
                    result.Message = "ServerName And DBName cannot be empty.";
                    return result;
                }


                var eLINKSqlServer = _eLINKSqlServerRepository.GetELINKSqlServerByServerNameAndDBName(sqlTableClearScheduleDto.ServerName, sqlTableClearScheduleDto.DBName, isEnable: false);
                if (eLINKSqlServer == null)
                {
                    result.Message = $"'{sqlTableClearScheduleDto.ServerName}' Server Or '{sqlTableClearScheduleDto.DBName}' DB doesn't exist.";
                    return result;
                }

                var customPolicyDateTimeMetricsSchedule = sqlTableClearScheduleDto.ToCovertSqlTableClearSchedule(); ;
                customPolicyDateTimeMetricsSchedule.SqlServerId = eLINKSqlServer.Id;
                customPolicyDateTimeMetricsSchedule.CreatedAt = DateTime.UtcNow;

                var schedule = _sqlTableClearScheduleRepository.GetSqlTableClearScheduleByPartitionKeyAndServerId(customPolicyDateTimeMetricsSchedule.PartitionKey, customPolicyDateTimeMetricsSchedule.SqlServerId);
                if (schedule != null)
                {
                    result.Message = $"This type of policy already exists in this '{sqlTableClearScheduleDto.DBName}' DB of '{sqlTableClearScheduleDto.ServerName}' Server .{customPolicyDateTimeMetricsSchedule?.JObject}";
                    return result;
                }

                eLINKSqlServer.SqlTableClearSchedules = new List<SqlTableClearSchedule>()
                {
                    customPolicyDateTimeMetricsSchedule
                };

                var clearPlan = await _sqlPlanParserService.BuildClearPlan(eLINKSqlServer);

                if (clearPlan == null || clearPlan.ClearServer == null
                    || string.IsNullOrEmpty(clearPlan.ClearServer.ConnectionStr))
                {
                    result.Message = $"This type of policy cannot build a clear plan. Please check carefully.";
                    return result;
                }

                if (string.IsNullOrEmpty(customPolicyDateTimeMetricsSchedule.PartitionKey) || (customPolicyDateTimeMetricsSchedule.PartitionKey.IndexOf("*") != -1 && customPolicyDateTimeMetricsSchedule.PartitionKey.IndexOf("*") != customPolicyDateTimeMetricsSchedule.PartitionKey.Length - 1))
                {
                    result.Message = @"TableName can't be null or TableName.
                                        There are three formats:

                                        1. Excluding *. such as ""table_name""

                                        2. Only *. such as ""*""

                                        3. * can only be placed last. such as ""table_name_*""";
                    return result;
                }

                var bl = false;
                var plans = clearPlan.GetPlans(isParseOnly: true);
                if (plans.Value == null || plans.Value.Count == 0)
                {
                    result.Message = $"This type of policy cannot generate an executable plan. Please check carefully.";
                    return result;
                }

                var sb = new StringBuilder();
                var sqlColumnTypeName = GetSqlColumnTypeNameByMetricsType<M>();
                foreach (var sqlPlan in plans.Value)
                {
                    bl = _planRepository.IsExistTable(sqlPlan.TableName, sqlTableClearScheduleDto.DBName, clearPlan.ClearServer.ConnectionStr);
                    if (!bl)
                    {
                        result.Message = $"'{sqlPlan.TableName}' table doesn't in '{sqlTableClearScheduleDto.DBName}' DB.";
                        result.Description = $"Has passed the following=>{sb.ToString()}";
                        //Note:This contains sensitive informations(such as password). If you need to display or save it, please hide sensitive information.
                        // result.Description = $"Sql:{plans.Value[0]}->ConnectionString:{clearPlan.ClearServer.ConnectionStr}"; 
                        return result;
                    }


                    bl = _planRepository.IsExistColumn(sqlPlan.TableName, clearPlan.ClearTables.FirstOrDefault().WhereMetrics.ColumnName, clearPlan.ClearServer.ConnectionStr, sqlColumnTypeName);
                    if (!bl)
                    {
                        result.Message = $"this '{clearPlan.ClearTables.FirstOrDefault().WhereMetrics.ColumnName}' column of {sqlColumnTypeName} doesn't in '{sqlPlan.TableName}' Table. Please check whether the column name or type is correct";
                        result.Description = $"Has passed the following=>{sb.ToString()}";
                        // //Note:This contains sensitive informations(such as password). If you need to display or save it, please hide sensitive information.
                        //result.Description = $"Sql:{plans.Value[0]}->ConnectionString:{clearPlan.ClearServer.ConnectionStr}";
                        return result;
                    }

                    bl = _planRepository.ValidateSQL(sqlPlan.Sql, clearPlan.ClearServer.ConnectionStr);
                    if (!bl)
                    {
                        result.Message = $"The generated SQL statement is illegal. Sql: {sqlPlan.Sql}";
                        result.Description = $"Has passed the following=>{sb.ToString()}";
                        //Note:This contains sensitive informations(such as password). If you need to display or save it, please hide sensitive information.
                        //result.Description = $"Sql:{plans.Value[0]}->ConnectionString:{clearPlan.ClearServer.ConnectionStr}";
                        return result;
                    }

                    sb.AppendLine($"TableName:{sqlPlan.TableName}(Such as Sql:{sqlPlan.Sql})");
                }

                if (!InsertServerDBMappingTables(clearPlan))
                {
                    result.Message = $"ServerDBMappingTables insert failed.";
                    return result;
                }
                
                bl = _sqlTableClearScheduleRepository.Insert(customPolicyDateTimeMetricsSchedule);

                result.Tag = bl ? 1 : 0;
                if (bl)
                {
                    result.Message = $"Policy {metricsType} Metrics insert successful.";
                    //result.Description = sb.ToString();
                }
                else
                {
                    result.Message = $"Policy {metricsType} Metrics insert failed.";
                    // result.Description = sb.ToString();
                }

                return result;
            }
            catch (Exception ex)
            {
                result.Message = $"Policy {metricsType} Metrics insert failed.";
                result.Message = ex.Message;
                return result;
            }
        }

        private bool InsertServerDBMappingTables(ClearPlan clearPlan)
        {
            if (clearPlan == null || clearPlan.ClearTables == null || clearPlan.ClearServer == null)
            {
                return false;
            }

            var list = new List<ELinkServerDBMappingTable>();
            foreach (var item in clearPlan.ClearTables)
            {
                list.Add(new ELinkServerDBMappingTable()
                {
                    ServerName = clearPlan.ClearServer.ServerName,
                    DBName = clearPlan.ClearServer.DBName,
                    TableName = item.TableName,
                    ColumnName = item.WhereMetrics.ColumnName,
                    Status=0,
                    CreatedAt = DateTime.UtcNow
                });
            }

            return _eLinkServerDBMappingTableService.BulkInsertSql(list).Tag==1?true:false;
        }

        private async Task<TData> Update<M>(SqlTableClearScheduleDto<M> sqlTableClearScheduleDto)
        {
            var metricsType = typeof(M).Name;
            var result = new TData();
            try
            {
                if (sqlTableClearScheduleDto == null || string.IsNullOrEmpty(sqlTableClearScheduleDto.ServerName) || string.IsNullOrEmpty(sqlTableClearScheduleDto.DBName))
                {
                    result.Message = "ServerName And DBName cannot be empty.";
                    return result;
                }

                var eLINKSqlServer = _eLINKSqlServerRepository.GetELINKSqlServerByServerNameAndDBName(sqlTableClearScheduleDto.ServerName, sqlTableClearScheduleDto.DBName);
                if (eLINKSqlServer == null)
                {
                    result.Message = $"'{sqlTableClearScheduleDto.ServerName}' Server And '{sqlTableClearScheduleDto.DBName}' DB doesn't exist.";
                    return result;
                }

                var customPolicyDateTimeMetricsSchedule = sqlTableClearScheduleDto.ToCovertSqlTableClearSchedule(); ;
                customPolicyDateTimeMetricsSchedule.SqlServerId = eLINKSqlServer.Id;
                customPolicyDateTimeMetricsSchedule.UpdatedAt = DateTime.UtcNow;

                var schedule = _sqlTableClearScheduleRepository.GetSqlTableClearScheduleByPartitionKeyAndServerId(customPolicyDateTimeMetricsSchedule.PartitionKey, customPolicyDateTimeMetricsSchedule.SqlServerId);
                if (schedule == null)
                {
                    result.Message = $"This Policy {metricsType} Metrics doesn't exist.{customPolicyDateTimeMetricsSchedule?.JObject}";
                    return result;
                }

                customPolicyDateTimeMetricsSchedule.Id = schedule.Id;

                eLINKSqlServer.SqlTableClearSchedules = new List<SqlTableClearSchedule>()
                {
                  customPolicyDateTimeMetricsSchedule
                };

                var clearPlan = await _sqlPlanParserService.BuildClearPlan(eLINKSqlServer);

                if (clearPlan == null || clearPlan.ClearServer == null
                    || string.IsNullOrEmpty(clearPlan.ClearServer.ConnectionStr))
                {
                    result.Message = $"This type of policy cannot build a clear plan. Please check carefully.";
                    return result;
                }

                if (string.IsNullOrEmpty(customPolicyDateTimeMetricsSchedule.PartitionKey) || (customPolicyDateTimeMetricsSchedule.PartitionKey.IndexOf("*") != -1 && customPolicyDateTimeMetricsSchedule.PartitionKey.IndexOf("*") != customPolicyDateTimeMetricsSchedule.PartitionKey.Length - 1))
                {
                    result.Message = @"TableName can't be null or TableName.
                                        There are three formats:

                                        1. Excluding *. such as ""table_name""

                                        2. Only *. such as ""*""

                                        3. * can only be placed last. such as ""table_name_*""";
                    return result;
                }


                var bl = false;
                var plans = clearPlan.GetPlans(isParseOnly: true);
                if (plans.Value == null || plans.Value.Count == 0)
                {
                    result.Message = $"This type of policy cannot generate an executable plan. Please check carefully.";
                    return result;
                }

                var sb = new StringBuilder();
                var sqlColumnTypeName = GetSqlColumnTypeNameByMetricsType<M>();
                foreach (var sqlPlan in plans.Value)
                {
                    bl = _planRepository.IsExistTable(sqlPlan.TableName, sqlTableClearScheduleDto.DBName, clearPlan.ClearServer.ConnectionStr);
                    if (!bl)
                    {
                        result.Message = $"'{sqlPlan.TableName}' table doesn't in '{sqlTableClearScheduleDto.DBName}' DB.";
                        result.Description = $"Has passed the following=>{sb.ToString()}";
                        return result;
                    }

                    bl = _planRepository.IsExistColumn(sqlPlan.TableName, clearPlan.ClearTables.FirstOrDefault().WhereMetrics.ColumnName, clearPlan.ClearServer.ConnectionStr, sqlColumnTypeName);
                    if (!bl)
                    {
                        result.Message = $"this '{clearPlan.ClearTables.FirstOrDefault().WhereMetrics.ColumnName}' column of {sqlColumnTypeName} doesn't in '{customPolicyDateTimeMetricsSchedule.PartitionKey}' Table. Please check whether the column name or type is correct";
                        result.Description = $"Has passed the following=>{sb.ToString()}";
                        return result;
                    }

                    bl = _planRepository.ValidateSQL(sqlPlan.Sql, clearPlan.ClearServer.ConnectionStr);
                    if (!bl)
                    {
                        result.Message = $"The generated SQL statement is illegal.";
                        result.Description = $"Has passed the following=>{sb.ToString()}";
                        return result;
                    }

                    sb.AppendLine($"TableName:{sqlPlan.TableName}(Such as Sql:{sqlPlan.Sql})");
                }

                bl = _sqlTableClearScheduleRepository.Update(customPolicyDateTimeMetricsSchedule);
                result.Tag = bl ? 1 : 0;
                if (bl)
                    result.Message = $"Policy {metricsType} Metrics update successful.";
                else
                    result.Message = $"Policy {metricsType} Metrics update failed.";

                return result;
            }
            catch (Exception ex)
            {
                result.Message = $"Policy {metricsType} Metrics update failed.";
                result.Message = ex.Message;
                return result;
            }
        }

        public async Task<TData> EnableOrDisable(SqlTableClearScheduleEnableOrDisableDto sqlTableClearScheduleEnableOrDisableDto)
        {
            var result = new TData();
            try
            {
                if (sqlTableClearScheduleEnableOrDisableDto == null
                    || string.IsNullOrEmpty(sqlTableClearScheduleEnableOrDisableDto.ServerName)
                    || string.IsNullOrEmpty(sqlTableClearScheduleEnableOrDisableDto.DBName)
                    || string.IsNullOrEmpty(sqlTableClearScheduleEnableOrDisableDto.TableName))
                {
                    result.Message = "ServerName And DBName And TableName cannot be empty.";
                    return result;
                }

                var eLINKSqlServer = _eLINKSqlServerRepository.GetELINKSqlServerByServerNameAndDBName(sqlTableClearScheduleEnableOrDisableDto.ServerName, sqlTableClearScheduleEnableOrDisableDto.DBName);
                if (eLINKSqlServer == null)
                {
                    result.Message = $"'{sqlTableClearScheduleEnableOrDisableDto.ServerName}' Server And '{sqlTableClearScheduleEnableOrDisableDto.DBName}' DB doesn't exist.";
                    return result;
                }

                var item = _sqlTableClearScheduleRepository.GetSqlTableClearScheduleByPartitionKeyAndServerId(sqlTableClearScheduleEnableOrDisableDto.TableName, eLINKSqlServer.Id);
                if (item == null)
                {
                    result.Message = $"'{sqlTableClearScheduleEnableOrDisableDto.TableName}' Table doesn't exist.";
                    return result;
                }

                item.Enable = sqlTableClearScheduleEnableOrDisableDto.Enable;

                var bl = _sqlTableClearScheduleRepository.EnableOrDisable(eLINKSqlServer.Id, item.Id, sqlTableClearScheduleEnableOrDisableDto.Enable);

                result.Tag = bl ? 1 : 0;
                var action = sqlTableClearScheduleEnableOrDisableDto.Enable ? "Enable" : "Disable";
                if (bl)
                    result.Message = $"SqlTableClearSchedule {action} update successful.";
                else
                    result.Message = $"SqlTableClearSchedule {action} update failed.";

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                result.Message = $"SqlTableClearSchedule update failed.";
                result.Message = ex.Message;
                return result;
            }
        }
    }
}
