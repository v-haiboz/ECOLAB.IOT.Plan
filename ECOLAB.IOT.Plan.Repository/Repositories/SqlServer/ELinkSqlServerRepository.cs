namespace ECOLAB.IOT.Plan.Repository.Repositories.SqlServer
{
    using Dapper;
    using ECOLAB.IOT.Plan.Entity.Entities.SqlServer;
    using ECOLAB.IOT.Plan.Entity.SqlServer;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;

    public interface IELinkSqlServerRepository
    {
        public bool Insert(ELinkSqlServer eLINKSqlServer);

        public bool Update(ELinkSqlServer eLINKSqlServer);

        public bool Delete(ELinkSqlServer eLINKSqlServer);

        public List<ELinkSqlServer> GetList(string serverName, string dbName);

        public ELinkSqlServer? GetELINKSqlServerByServerNameAndDBName(string serverName, string dbName,bool isEnable= true);

        public List<ELinkSqlServer>? GetELINKSqlServersByServerNameOrDBName(string serverName, string dbName, bool isEnable = true);

        public bool BulkInsertSql(List<ELinkSqlServer> objs);

        public List<ELinkSqlServer> GetDetailList();

        public List<ELinkSqlServer> GetListByServerName(string serverName);
    }

    public class ELinkSqlServerRepository : Repository, IELinkSqlServerRepository
    {
        private readonly string tableName = "ELinkSqlServer";
        public ELinkSqlServerRepository(IConfiguration config) : base(config)
        {
        }
        public bool Insert(ELinkSqlServer eLINKSqlServer)
        {
            return Execute((conn) =>
            {
                var insertSql = @"insert into ELinkSqlServer(ServerName,DBName,UserId,Password,CreatedAt) values(@ServerName,@DBName,@UserId,@Password,@CreatedAt)";
                var execnum = conn.Execute(insertSql, eLINKSqlServer);
                return execnum > 0;
            });
        }

        public bool Update(ELinkSqlServer eLINKSqlServer)
        {
            eLINKSqlServer.UpdatedAt = DateTime.UtcNow;
            return Execute((conn) =>
            {
                var updateSql = @"Update ELinkSqlServer Set UserId=@DBName,Password=@Password,UpdatedAt=@UpdatedAt where ServerName=@ServerName and DBName=@DBName";
                var execnum = conn.Execute(updateSql, eLINKSqlServer);
                return execnum > 0;
            });
        }

        public bool Delete(ELinkSqlServer eLINKSqlServer)
        {
            return Execute((conn, trans) =>
            {
                try
                {
                    var insertSql = @"DELETE FROM ELinkSqlServer WHERE Id=@Id";
                    var execnum = conn.Execute(insertSql, eLINKSqlServer, transaction: trans);
                    execnum = conn.Execute(@$"DELETE  FROM SqlTableClearSchedule WHERE SqlServerId=@Id", eLINKSqlServer, transaction: trans);
                    trans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw;
                }
                
            });
        }

        public List<ELinkSqlServer> GetList(string serverName, string dbName)
        {
            return Execute((conn) =>
            {
                var whereStr ="1=1";

                if (!string.IsNullOrEmpty(serverName))
                {
                    whereStr = whereStr + $" and a.ServerName='{serverName}'";
                }

                if (!string.IsNullOrEmpty(dbName))
                {
                    whereStr = whereStr + $" and a.DBName='{dbName}'";
                }

                var list = conn.Query<ELinkSqlServer>($"select * from ELinkSqlServer where {whereStr}").ToList();
                return list;
            });
        }

        public ELinkSqlServer? GetELINKSqlServerByServerNameAndDBName(string serverName, string dbName, bool isEnable=true)
        {
            return Execute((conn) =>
            {
                if (string.IsNullOrEmpty(serverName) || string.IsNullOrEmpty(serverName))
                {
                    return null;
                }

                var whereStr = isEnable ? $"b.Enable=1 and a.ServerName='{serverName}' and a.DBName='{dbName}'" : $"a.ServerName='{serverName}' and a.DBName='{dbName}'";

                string query = $@"SELECT * FROM [dbo].[ELinkSqlServer] a
                                           LEFT JOIN [dbo].[SqlTableClearSchedule] b
                                           on a.Id=b.SqlServerId where {whereStr} order by a.Id, b.Type ,len(b.PartitionKey) desc";

                ELinkSqlServer lookup = null;
                var list = conn.Query<ELinkSqlServer, SqlTableClearSchedule, ELinkSqlServer>(query,
                    (elinkSqlServer, sqlTableClearSchedule) =>
                    {
                        if (lookup == null || lookup.Id != elinkSqlServer.Id)
                            lookup = elinkSqlServer;
                        if (sqlTableClearSchedule != null)
                            lookup.SqlTableClearSchedules.Add(sqlTableClearSchedule);
                        return lookup;
                    }).Distinct().ToList();
                return list.FirstOrDefault();
            });
        }

        public List<ELinkSqlServer>? GetELINKSqlServersByServerNameOrDBName(string serverName, string dbName, bool isEnable = true)
        {
            return Execute((conn) =>
            {

                var whereStr = isEnable ? "b.Enable=1 " : "1=1";

                if (!string.IsNullOrEmpty(serverName))
                {
                    whereStr = whereStr + $"and a.ServerName='{serverName}'";
                }

                if (!string.IsNullOrEmpty(dbName))
                {
                    whereStr = whereStr + $"and a.DBName='{dbName}'";
                }

                string query = $@"SELECT * FROM [dbo].[ELinkSqlServer] a
                                           LEFT JOIN [dbo].[SqlTableClearSchedule] b
                                           on a.Id=b.SqlServerId where {whereStr} order by a.Id, b.Type ,len(b.PartitionKey) desc";

                ELinkSqlServer lookup = null;
                var list = conn.Query<ELinkSqlServer, SqlTableClearSchedule, ELinkSqlServer>(query,
                    (elinkSqlServer, sqlTableClearSchedule) =>
                    {
                        if (lookup == null || lookup.Id != elinkSqlServer.Id)
                            lookup = elinkSqlServer;
                        if (sqlTableClearSchedule != null)
                            lookup.SqlTableClearSchedules.Add(sqlTableClearSchedule);
                        return lookup;
                    }).Distinct().ToList();
                return list;
            });
        }

        public List<ELinkSqlServer> GetDetailList()
        {
            return Execute((conn) =>
            {
                string query = @"SELECT  * FROM [dbo].[ELinkSqlServer] a
                                           LEFT JOIN [dbo].[SqlTableClearSchedule] b
                                           on a.Id=b.SqlServerId where b.Enable=1 order by a.Id, b.Type ,len(b.PartitionKey) desc";
                ELinkSqlServer lookup = null;
                var list = conn.Query<ELinkSqlServer, SqlTableClearSchedule, ELinkSqlServer>(query,
                    (elinkSqlServer, sqlTableClearSchedule) =>
                    {
                        if (lookup == null || lookup.Id != elinkSqlServer.Id)
                            lookup = elinkSqlServer;
                        if (sqlTableClearSchedule != null)
                            lookup.SqlTableClearSchedules.Add(sqlTableClearSchedule);
                        return lookup;
                    }).Distinct().ToList();
                return list;
            });
        }

        public List<ELinkSqlServer> GetListByServerName(string serverName)
        {
            return Execute((conn) =>
            {
                string query = $@"SELECT  * FROM [dbo].[ELinkSqlServer] a
                                           LEFT JOIN [dbo].[SqlTableClearSchedule] b
                                           on a.Id=b.SqlServerId where a.ServerName={serverName} order by a.Id";
                ELinkSqlServer lookup = null;
                var list = conn.Query<ELinkSqlServer, SqlTableClearSchedule, ELinkSqlServer>(query,
                    (elinkSqlServer, sqlTableClearSchedule) =>
                    {
                        if (lookup == null || lookup.Id != elinkSqlServer.Id)
                            lookup = elinkSqlServer;
                        if (sqlTableClearSchedule != null)
                            lookup.SqlTableClearSchedules.Add(sqlTableClearSchedule);
                        return lookup;
                    }).Distinct().ToList();
                return list;
            });
        }
        public bool BulkInsertSql(List<ELinkSqlServer> objs)
        {
            return Execute((conn, trans) =>
            {
                var bulkSql = SqlBuilderHelper.GenerateBulkInsertSql(objs, tableName);
                var execnum = conn.Execute(bulkSql);
                return execnum > 0;
            });
        }

    }
}
