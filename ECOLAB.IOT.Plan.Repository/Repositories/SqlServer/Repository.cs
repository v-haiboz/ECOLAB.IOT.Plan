namespace ECOLAB.IOT.Plan.Repository.Repositories.SqlServer
{
    using Microsoft.Extensions.Configuration;
    using System.Data.SqlClient;

    public class Repository : IRepository
    {
        private IConfiguration _config;
        public Repository(IConfiguration config)
        {
            _config = config;
        }

        public string ConnectionString
        {
            get
            {
                return _config["ConnectionStrings:SqlConnectionString"];
            }
        }

        public T Execute<T>(Func<SqlConnection, T> func)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                return func(connection);
            }
        }

        public T Execute<T>(string connectionString, Func<SqlConnection, T> func)
        {
            using (SqlConnection connection = new SqlConnection(connectionString ?? ConnectionString))
            {
                return func(connection);
            }
        }

        public T Execute<T>(Func<SqlConnection, SqlTransaction, T> func)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    return func(transaction.Connection, transaction);
                }
            }
        }

        public T Execute<T>(string connectionString, Func<SqlConnection, SqlTransaction, T> func)
        {
            using (SqlConnection connection = new SqlConnection(connectionString ?? ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    return func(connection, transaction);
                }
            }
        }


    }
}
