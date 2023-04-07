namespace ECOLAB.IOT.Plan.Repository.Repositories.SqlServer
{
    using Dapper;
    using ECOLAB.IOT.Plan.Entity.Entities.SqlServer;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json.Linq;
    using System.Collections.Generic;
    using System.Transactions;

    public interface IELinkServerDBMappingTableRepository
    {
        public bool Insert(ELinkServerDBMappingTable eLinkServerDBMappingTable);
        public bool BulkInsertSql(List<ELinkServerDBMappingTable> eLinkServerDBMappingTables);

        public bool BulkUpdateSql(List<ELinkServerDBMappingTable> eLinkServerDBMappingTables);


        public bool IsFinish(string serverName,string dbName, string tableName);

        public ELinkServerDBMappingTable? GetELinkServerDBMappingTable(string serverName, string dbName, string tableName);

    }

    public class ELinkServerDBMappingTableRepository : Repository, IELinkServerDBMappingTableRepository
    {
        private readonly string tableName = "[dbo].[ELinkServerDBMappingTable]";
        public ELinkServerDBMappingTableRepository(IConfiguration config) : base(config)
        {
        }
        public bool Insert(ELinkServerDBMappingTable eLinkServerDBMappingTable)
        {
            return Execute((conn) =>
            {
                var insertSql = @"insert into ELinkServerDBMappingTable(ServerName,DBName,TableName,ColumnName,UpdatedAt,CreatedAt) values(@ServerName,@DBName,@TableName,@ColumnName,@UpdatedAt,@CreatedAt)";
                var execnum = conn.Execute(insertSql, eLinkServerDBMappingTable);
                return execnum > 0;
            });
        }

        public bool BulkInsertSql(List<ELinkServerDBMappingTable> eLinkServerDBMappingTables)
        {
            return Execute((conn, transaction) =>
            {
                var insertSql = @"insert into ELinkServerDBMappingTable(ServerName,DBName,TableName,ColumnName,Status,UpdatedAt,CreatedAt) values(@ServerName,@DBName,@TableName,@ColumnName,@Status,@UpdatedAt,@CreatedAt)";

                foreach (var item in eLinkServerDBMappingTables)
                {
                    var row = conn.Query($"select * from ELinkServerDBMappingTable where ServerName=@ServerName and DBName=@DBName and TableName=@TableName", item, transaction: transaction);
                    if (row != null && row.Count() > 0)
                    {
                        transaction.Rollback();
                        return false;
                       // conn.Execute($"delete from ELinkServerDBMappingTable where ServerName=@ServerName and DBName=@DBName and TableName=@TableName", item, transaction: transaction);
                    }

                    conn.Execute(insertSql, item, transaction: transaction);
                }
                
                transaction.Commit();
                return true;
            });
        }

        public bool BulkUpdateSql(List<ELinkServerDBMappingTable> eLinkServerDBMappingTables)
        {
            return Execute((conn, transaction) =>
            {
                var updatedSql = @"Update ELinkServerDBMappingTable set ColumnName=@ColumnName,Status=@Status,UpdatedAt=@UpdatedAt where ServerName=@ServerName and DBName=@DBName and TableName=@TableName";

                foreach (var item in eLinkServerDBMappingTables)
                {
                    conn.Execute(updatedSql, item, transaction: transaction);
                }

                transaction.Commit();
                return true;
            });
        }

        public bool IsFinish(string serverName, string dbName, string tableName)
        {
            return Execute((conn) =>
            {
                var sql = $"select * from ELinkServerDBMappingTable where ServerName='{serverName}' and DBName='{dbName}' and TableName='{tableName}' and Status=1";
                var row = conn.Query(sql);
                if (row != null && row.Count() > 0)
                {
                    return true;
                }

                return false;
            });
        }

        public ELinkServerDBMappingTable? GetELinkServerDBMappingTable(string serverName, string dbName, string tableName)
        {
            return Execute((conn) =>
            {
                var sql = $"select * from ELinkServerDBMappingTable where ServerName='{serverName}' and DBName='{dbName}' and TableName='{tableName}'";
                var item = conn.Query<ELinkServerDBMappingTable>(sql).FirstOrDefault();
                return item;
            });
        }
    }
}
