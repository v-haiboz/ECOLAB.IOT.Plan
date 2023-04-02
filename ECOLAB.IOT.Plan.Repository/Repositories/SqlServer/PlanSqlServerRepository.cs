namespace ECOLAB.IOT.Plan.Repository.Repositories.SqlServer
{
    using Dapper;
    using Microsoft.Extensions.Configuration;

    public interface IPlanSqlServerRepository
    {
        public bool Delete(string connectionStr, string sqlCommand);
    }


    public class PlanSqlServerRepository : Repository, IPlanSqlServerRepository
    {
        public PlanSqlServerRepository(IConfiguration config) : base(config)
        {
        }
        public bool Delete(string connectionStr, string sqlCommand)
        {
            return Execute(connectionStr, (conn) => {
                var execnum = conn.Execute(sqlCommand);
                return execnum > 0;
            });
        }
    }
}
