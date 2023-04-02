namespace ECOLAB.IOT.Plan.Service.Sql
{
    using ECOLAB.IOT.Plan.Entity;
    using ECOLAB.IOT.Plan.Entity.Entities.SqlServer;
    using ECOLAB.IOT.Plan.Repository.Repositories.SqlServer;
    using System;
    using System.Collections.Generic;

    public interface IELinkServerDBMappingTableService
    {
        public TData Insert(ELinkServerDBMappingTable eLinkServerDBMappingTable);
        public TData BulkInsertSql(List<ELinkServerDBMappingTable> eLinkServerDBMappingTables);

    }

    public class ELinkServerDBMappingTableService : IELinkServerDBMappingTableService
    {
        private readonly IELinkServerDBMappingTableRepository _eLinkServerDBMappingTableRepository;
        public ELinkServerDBMappingTableService(IELinkServerDBMappingTableRepository eLinkServerDBMappingTableRepository)
        {
            _eLinkServerDBMappingTableRepository = eLinkServerDBMappingTableRepository;
        }

        public TData Insert(ELinkServerDBMappingTable eLinkServerDBMappingTable)
        {
            var result = new TData();
            try
            {
                if (eLinkServerDBMappingTable == null 
                    || string.IsNullOrEmpty(eLinkServerDBMappingTable.ServerName) 
                    || string.IsNullOrEmpty(eLinkServerDBMappingTable.DBName)
                    || string.IsNullOrEmpty(eLinkServerDBMappingTable.TableName))
                {
                    result.Message = "ServerName And DBName And TableName cannot be empty.";
                    return result;
                }
               
                var bl = _eLinkServerDBMappingTableRepository.Insert(eLinkServerDBMappingTable);

                result.Tag = bl ? 1 : 0;
                if (bl)
                    result.Message = "ELinkServerDBMappingTable Insert successful";
                return result;
            }
            catch (Exception ex)
            {
                result.Tag = 0;
                result.Message = "ELinkServerDBMappingTable Insert failed";
                result.Description = ex.ToString();
                return result;
            }
        }

     
        public TData BulkInsertSql(List<ELinkServerDBMappingTable> eLinkServerDBMappingTables)
        {
            var result = new TData();
            try
            {
                foreach (var obj in eLinkServerDBMappingTables)
                {
                    if (obj == null
                     || string.IsNullOrEmpty(obj.ServerName)
                     || string.IsNullOrEmpty(obj.DBName)
                     || string.IsNullOrEmpty(obj.TableName))
                    {
                        result.Message = "ServerName And DBName And TableName cannot be empty.";
                        return result;
                    }
                }

                var bl = _eLinkServerDBMappingTableRepository.BulkInsertSql(eLinkServerDBMappingTables);
                result.Tag = bl ? 1 : 0;
                if (bl)
                    result.Message = "ELinkServerDBMappingTable List bulkInsert successful";
                return result;
            }
            catch (Exception ex)
            {
                result.Tag = 0;
                result.Message = "ELinkServerDBMappingTable List bulkInsert failed";
                result.Description = ex.ToString();
                return result;
            }
        }
    }
}
