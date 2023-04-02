namespace ECOLAB.IOT.Plan.Repository.Repositories.SqlServer
{
    using Dapper;
    using ECOLAB.IOT.Plan.Entity.Entities;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface IELinkPlanHistoryRepository
    {
        public bool Insert(ELinkPlanHistory eCOLinkPlanHistory);

        public List<ELinkPlanHistory> GetListByCreatedAt(DateTime createdAt);
    }

    public class ELinkPlanHistoryRepository : Repository, IELinkPlanHistoryRepository
    {
        private readonly string tableName = "ECOLinkPlanHistory";
        public ELinkPlanHistoryRepository(IConfiguration config):base(config)
        {
        }
        public bool Insert(ELinkPlanHistory eCOLinkPlanHistory)
        {
            return Execute((conn) =>
            {
                var insertSql = @"insert into ELinkPlanHistory(Category,Type,Message,TargetRowData,SourceRowData,CreatedAt) values(@Category,@Type,@Message,@TargetRowData,@SourceRowData,@CreatedAt)";
                var execnum = conn.Execute(insertSql, eCOLinkPlanHistory);
                return execnum > 0;
            });
        }

     
        public List<ELinkPlanHistory> GetListByCreatedAt(DateTime createdAt)
        {
            return Execute((conn) =>
            {
                string query = $@"SELECT * FROM [dbo].[ELinkPlanHistory] where createdAt.ServerName='{createdAt}' order by a.Id";
                var list = conn.Query<ELinkPlanHistory>(query).ToList();
                return list;
            });
        }
    }
}
