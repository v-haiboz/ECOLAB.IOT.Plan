namespace ECOLAB.IOT.Plan.Repository.Repositories.SqlServer
{
    using Dapper;
    using ECOLAB.IOT.Plan.Entity.Entities.SqlServer;
    using Microsoft.Extensions.Configuration;
    using System.Collections.Generic;

    public interface IUserWhiteListRepository
    {
        public bool Create(UserWhiteList userWhiteList);

        public bool Update(UserWhiteList userWhiteList);

        public bool Delete(UserWhiteList userWhiteList);

        public bool CheckExpiredByEmailAndSecretKey(string email,string secretKey);

        public List<UserWhiteList>  GetUserWhiteList(string email = null, int pageIndex = 1, int pageSize = 50);

        public int CountUserWhiteList(string email = null);
    }

    public class UserWhiteListRepository : Repository, IUserWhiteListRepository
    {
        private readonly string tableName = "UserWhiteList";
        public UserWhiteListRepository(IConfiguration config) : base(config)
        {
        }
        public bool Create(UserWhiteList userWhiteList)
        {
            return Execute((conn) =>
            {
                var insertSql = @$"insert into {tableName}(Email,SecretKey,ExpiredAt,CreatedAt) values(@Email,@SecretKey,@ExpiredAt,@CreatedAt)";
                var execnum = conn.Execute(insertSql, userWhiteList);
                return execnum > 0;
            });
        }

        public bool Update(UserWhiteList userWhiteList)
        {
            return Execute((conn) =>
            {
                var insertSql = @$"Update {tableName} Set SecretKey=@SecretKey,ExpiredAt=@ExpiredAt where Email=@Email";
                var execnum = conn.Execute(insertSql, userWhiteList);
                return execnum > 0;
            });
        }

        public bool Delete(UserWhiteList userWhiteList)
        {
            return Execute((conn) =>
            {
                var insertSql = @$"DELETE FROM {tableName} Where Email=@Email";
                var execnum = conn.Execute(insertSql, userWhiteList);
                return execnum > 0;
            });
        }

        public bool CheckExpiredByEmailAndSecretKey(string email, string secretKey)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(secretKey))
            {
                return false;
            }

            return Execute((conn) =>
            {
                var insertSql = $"select ExpiredAt from {tableName} where Email='{email}' and SecretKey='{secretKey}' order by CreatedAt";
                var expiredAt = conn.Query<DateTime>(insertSql).FirstOrDefault();
                if (expiredAt == default(DateTime) || expiredAt<DateTime.UtcNow)
                {
                    return false;
                }

                return true;
            });
        }

        public List<UserWhiteList> GetUserWhiteList(string email = null, int pageIndex = 1, int pageSize = 50)
        {
            var sql = $"select * from {tableName} where Email='{email}' order by CreatedAt";
            if (string.IsNullOrEmpty(email))
            {
                sql = $"select * from {tableName} order by CreatedAt";
            }

            return Execute((conn) =>
            {
                var list = conn.Query<UserWhiteList>(sql).ToList();
                list = list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                return list;
            });
        }

        public int CountUserWhiteList(string email = null)
        {
            var sql = $"select * from {tableName} where Email='{email}' order by CreatedAt";
            if (string.IsNullOrEmpty(email))
            {
                sql = $"select * from {tableName} order by CreatedAt";
            }

            return Execute((conn) =>
            {
                var list = conn.Query<UserWhiteList>(sql).ToList();
                return list.Count;
            });
        }
    }
}
