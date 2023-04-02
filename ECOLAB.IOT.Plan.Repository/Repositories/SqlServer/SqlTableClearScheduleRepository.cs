namespace ECOLAB.IOT.Plan.Repository.Repositories.SqlServer
{
    using Dapper;
    using ECOLAB.IOT.Plan.Entity;
    using ECOLAB.IOT.Plan.Entity.Entities.SqlServer;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json.Linq;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Xml.Linq;

    public interface ISqlTableClearScheduleRepository
    {
        public bool Insert(SqlTableClearSchedule sqlTableClearSchedule);

        public bool BulkInsertSql(List<SqlTableClearSchedule> sqlTableClearSchedules);

        public bool Update(SqlTableClearSchedule sqlTableClearSchedule);

        public bool Delete(SqlTableClearSchedule sqlTableClearSchedule);

        public bool EnableOrDisable(int sqlServerId,int id, bool enable);

        public SqlTableClearSchedule? GetSqlTableClearScheduleByServerId(int serverId);

        public SqlTableClearSchedule? GetSqlTableClearScheduleByObjectAndServerId(string? jobject, int serverId);

        public SqlTableClearSchedule? GetSqlTableClearScheduleByPartitionKeyAndServerId(string? partitionKey, int serverId);

        public SqlTableClearSchedule? GetDynamicSqlTableClearScheduleByType(ClearScheduleType? clearScheduleType, int serverId);
    }

    public class SqlTableClearScheduleRepository : Repository, ISqlTableClearScheduleRepository
    {
        private readonly string tableName = "[dbo].[SqlTableClearSchedule]";

        public SqlTableClearScheduleRepository(IConfiguration config) : base(config)
        {
        }
        public bool Insert(SqlTableClearSchedule sqlTableClearSchedule)
        {
            return Execute((conn) =>
            {
                var insertSql = @"insert into SqlTableClearSchedule(SqlServerId,Type,PartitionKey,JObject,Enable,CreatedAt) values(@SqlServerId,@Type,@PartitionKey,@JObject,@Enable,@CreatedAt)";
                var execnum = conn.Execute(insertSql, sqlTableClearSchedule);
                return execnum > 0;
            });
        }

        public bool BulkInsertSql(List<SqlTableClearSchedule> sqlTableClearSchedules)
        {
            return Execute((conn, transaction) =>
            {

                var insertSql = @"insert into SqlTableClearSchedule(SqlServerId,Type,PartitionKey,JObject,Enable,CreatedAt) values(@SqlServerId,@Type,@PartitionKey,@JObject,@Enable,@CreatedAt)";

                foreach (var item in sqlTableClearSchedules)
                {
                    conn.Execute(insertSql, item);
                }
                
                transaction.Commit();
                return true;
            });
        }

        public bool Update(SqlTableClearSchedule sqlTableClearSchedule)
        {
            return Execute((conn) =>
            {
                var updateSql = @$"UPDATE SqlTableClearSchedule Set 
                                         Type=@Type,
                                         PartitionKey=@PartitionKey,
                                         JObject=@JObject,
                                         Enable=@Enable,
                                         UpdatedAt=@UpdatedAt
                                         WHERE Id ={sqlTableClearSchedule.Id} and SqlServerId=@SqlServerId";
                var execnum = conn.Execute(updateSql, sqlTableClearSchedule);
                return execnum > 0;
            });
        }

        public bool Delete(SqlTableClearSchedule sqlTableClearSchedule)
        {
            return Execute((conn) =>
            {
                var deleteSql = @$"Delete from  SqlTableClearSchedule WHERE Id ={sqlTableClearSchedule.Id}";
                var execnum = conn.Execute(deleteSql);
                return execnum > 0;
            });
        }

        public SqlTableClearSchedule? GetSqlTableClearScheduleByObjectAndServerId(string? jobject, int serverId)
        {
            return Execute((conn) =>
            {
                string query = $@"SELECT  * FROM [dbo].[SqlTableClearSchedule] a
                                             where a.SqlServerId={serverId} and a.JObject='{jobject}' order by a.Id";
                var item = conn.Query<SqlTableClearSchedule>(query).FirstOrDefault();
                return item;
            });
        }

        public SqlTableClearSchedule? GetSqlTableClearScheduleByServerId(int serverId)
        {
            return Execute((conn) =>
            {
                string query = $@"SELECT * FROM [dbo].[SqlTableClearSchedule] a
                                             where a.ServerId={serverId} order by a.Id";
                var item = conn.Query<SqlTableClearSchedule>(query).FirstOrDefault();
                return item;
            });
        }

        public SqlTableClearSchedule? GetSqlTableClearScheduleByPartitionKeyAndServerId(string? partitionKey, int serverId)
        {
            return Execute((conn) =>
            {
                string query = $@"SELECT  * FROM [dbo].[SqlTableClearSchedule] a
                                             where a.SqlServerId={serverId} and a.PartitionKey='{partitionKey}' order by a.Id";
                var item = conn.Query<SqlTableClearSchedule>(query).FirstOrDefault();
                return item;
            });
        }

        public SqlTableClearSchedule? GetDynamicSqlTableClearScheduleByType(ClearScheduleType? clearScheduleType, int serverId)
        {
            return Execute((conn) =>
            {
                string query = $@"SELECT  * FROM [dbo].[SqlTableClearSchedule] a
                                             where a.Type={clearScheduleType} and a.SqlServerId={serverId}";
                var item = conn.Query<SqlTableClearSchedule>(query).FirstOrDefault();
                return item;
            });
        }

        public bool EnableOrDisable(int sqlServerId, int id, bool enable)
        {
            return Execute((conn) =>
            {
                var enab = enable ? 1 : 0;
                var updateSql = @$"UPDATE SqlTableClearSchedule Set 
                                         Enable={enab},
                                         UpdatedAt='{DateTime.UtcNow}'
                                         WHERE Id={id} and SqlServerId={sqlServerId}";

                var execnum = conn.Execute(updateSql);
                return execnum > 0;
            });
        }
    }
}
