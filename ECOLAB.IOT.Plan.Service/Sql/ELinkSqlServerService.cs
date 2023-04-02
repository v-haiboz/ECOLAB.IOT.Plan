namespace ECOLAB.IOT.Plan.Service.Sql
{
    using AutoMapper;
    using ECOLAB.IOT.Plan.Entity;
    using ECOLAB.IOT.Plan.Entity.Dtos.Sql;
    using ECOLAB.IOT.Plan.Entity.SqlServer;
    using ECOLAB.IOT.Plan.Repository.Repositories.SqlServer;
    using System;
    using System.Collections.Generic;

    public interface IELinkSqlServerService
    {
        public TData Insert(ELINKSqlServerDto eLINKSqlServer);

        public TData Update(ELINKSqlServerDto eLINKSqlServer);

        public TData Delete(ELINKSqlServerDeleteDto eLINKSqlServer);

        public TDataPage<List<ELinkSqlServer>> GetList(string serverName, string dbName, int pageIndex = 1, int pageSize = 50);

        public TData<List<ELinkSqlServer>>? GetELINKSqlServersByServerNameOrDBName(string serverName, string dbName);

        public TData<List<ELinkSqlServer>> GetListByServerName(string serverName);

        public TData BulkInsertSql(List<ELINKSqlServerDto> objs);

        public TData<List<ELinkSqlServer>> GetDetailList();
    }

    public class ELinkSqlServerService : IELinkSqlServerService
    {
        private readonly IELinkSqlServerRepository _elinkSqlServerRepository;
        private readonly IMapper _mapper;
        public ELinkSqlServerService(IELinkSqlServerRepository elinkSqlServerRepository, IMapper mapper)
        {
            _elinkSqlServerRepository = elinkSqlServerRepository;
            _mapper = mapper;
        }

        public TData Insert(ELINKSqlServerDto eLINKSqlServer)
        {
            var result = new TData();
            try
            {
                if (eLINKSqlServer == null || string.IsNullOrEmpty(eLINKSqlServer.ServerName) || string.IsNullOrEmpty(eLINKSqlServer.DBName))
                {
                    result.Message = "ServerName And DBName cannot be empty.";
                    return result;
                }
                var exist = _elinkSqlServerRepository.GetELINKSqlServerByServerNameAndDBName(eLINKSqlServer.ServerName, eLINKSqlServer.DBName,false);
                if (exist != null)
                {
                    result.Message = $"{exist.ServerName} Server And {exist.DBName} DB already exist.";
                    return result;
                }

                var item = _mapper.Map<ELinkSqlServer>(eLINKSqlServer);

                var bl = _elinkSqlServerRepository.Insert(item);

                result.Tag = bl ? 1 : 0;
                if (bl)
                    result.Message = "Server Insert successful";
                return result;
            }
            catch (Exception ex)
            {
                result.Tag = 0;
                result.Message = "Server Insert failed";
                result.Description = ex.ToString();
                return result;
            }
        }

        public TData Update(ELINKSqlServerDto eLINKSqlServer)
        {
            var result = new TData();
            try
            {
                if (eLINKSqlServer == null || string.IsNullOrEmpty(eLINKSqlServer.ServerName) || string.IsNullOrEmpty(eLINKSqlServer.DBName))
                {
                    result.Message = "ServerName Or DBName cannot be empty.";
                    return result;
                }

                var exist = _elinkSqlServerRepository.GetELINKSqlServerByServerNameAndDBName(eLINKSqlServer.ServerName, eLINKSqlServer.DBName);
                if (exist == null)
                {
                    result.Message = $"{eLINKSqlServer.ServerName} Server -> {eLINKSqlServer.DBName} DB doesn't exist.";
                    return result;
                }

                var server = _mapper.Map<ELinkSqlServer>(eLINKSqlServer);
                server.Id = exist.Id;
                var bl = _elinkSqlServerRepository.Update(server);

                result.Tag = bl ? 1 : 0;
                if (bl)
                    result.Message = "Server Update successful";
                return result;
            }
            catch (Exception ex)
            {
                result.Tag = 0;
                result.Message = "Server Update failed";
                result.Description = ex.ToString();
                return result;
            }
        }

        public TData Delete(ELINKSqlServerDeleteDto eLINKSqlServer)
        {
            var result = new TData();
            try
            {
                if (eLINKSqlServer == null || string.IsNullOrEmpty(eLINKSqlServer.ServerName) || string.IsNullOrEmpty(eLINKSqlServer.DBName))
                {
                    result.Message = "ServerName Or DBName cannot be empty.";
                    return result;
                }
                var exist = _elinkSqlServerRepository.GetELINKSqlServerByServerNameAndDBName(eLINKSqlServer.ServerName, eLINKSqlServer.DBName);
                if (exist == null)
                {
                    result.Message = $"{eLINKSqlServer?.ServerName} Server -> {eLINKSqlServer?.DBName} DB doesn't exist.";
                    return result;
                }

                var bl = _elinkSqlServerRepository.Delete(new ELinkSqlServer()
                {
                    Id = exist.Id
                });

                result.Tag = bl ? 1 : 0;
                if (bl)
                    result.Message = "Server Delete successful";
                return result;
            }
            catch (Exception ex)
            {
                result.Tag = 0;
                result.Message = "Server Delete failed";
                result.Description = ex.ToString();
                return result;
            }
        }



        public TDataPage<List<ELinkSqlServer>> GetList(string serverName, string dbName, int pageIndex = 1, int pageSize = 50)
        {
            var result = new TDataPage<List<ELinkSqlServer>>();
            try
            {
                var list = _elinkSqlServerRepository.GetList(serverName, dbName);
                result.Tag = 1;
                result.Data =list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                result.PageSize= pageSize;
                result.PageIndex= pageIndex;
                result.Total = list.Count;
                result.Message = "Server GetList successful";
                return result;
            }
            catch (Exception ex)
            {
                result.Tag = 0;
                result.Message = "Server GetList failed";
                result.Description = ex.ToString();
                return result;
            }
        }

        public TData<List<ELinkSqlServer>>? GetELINKSqlServersByServerNameOrDBName(string serverName, string dbName)
        {
            var result = new TData<List<ELinkSqlServer>>();
            try
            {
                var list = _elinkSqlServerRepository.GetELINKSqlServersByServerNameOrDBName(serverName, dbName);
                result.Data = list;
                result.Tag = 1;
                result.Message = "Get ELINKSqlServersByServerNameOrDBName successful";
                return result;
            }
            catch (Exception ex)
            {
                result.Tag = 0;
                result.Message = "Get ELINKSqlServersByServerNameOrDBName failed";
                result.Description = ex.ToString();
                return result;
            }
        }

        public TData<List<ELinkSqlServer>> GetListByServerName(string serverName)
        {
            var result = new TData<List<ELinkSqlServer>>();
            try
            {
                var list = _elinkSqlServerRepository.GetListByServerName(serverName);
                result.Tag = 1;
                result.Data = list;
                result.Message = "Server GetListByServerName successful";
                return result;
            }
            catch (Exception ex)
            {
                result.Tag = 0;
                result.Message = "Server GetListByServerName failed";
                result.Description = ex.ToString();
                return result;
            }
        }
        public TData BulkInsertSql(List<ELINKSqlServerDto> objs)
        {
            var result = new TData();
            try
            {
                foreach (var obj in objs)
                {
                    if (string.IsNullOrEmpty(obj.ServerName) || string.IsNullOrEmpty(obj.DBName))
                    {
                        result.Message = "ServerName Or DBName cannot be empty.";
                        return result;
                    }
                    var exist = _elinkSqlServerRepository.GetELINKSqlServersByServerNameOrDBName(obj.ServerName, obj.DBName);
                    if (exist != null)
                    {
                        result.Message = $"{obj.ServerName} And {obj.DBName} already exist.";
                        return result;
                    }
                }

                var list = _mapper.Map<List<ELinkSqlServer>>(objs);
                var bl = _elinkSqlServerRepository.BulkInsertSql(list);

                result.Tag = bl ? 1 : 0;
                if (bl)
                    result.Message = "Server bulkInsert successful";
                return result;
            }
            catch (Exception ex)
            {
                result.Tag = 0;
                result.Message = "Server bulkInsert failed";
                result.Description = ex.ToString();
                return result;
            }
        }

        public TData<List<ELinkSqlServer>> GetDetailList()
        {
            var result = new TData<List<ELinkSqlServer>>();
            try
            {

                result.Data = _elinkSqlServerRepository.GetDetailList();
                result.Tag = 1;
                return result;
            }
            catch (Exception ex)
            {
                result.Tag = 0;
                result.Message = "Get ELINKSqlServer Detail List failed";
                result.Description = ex.ToString();
                return result;
            }
        }
    }
}
