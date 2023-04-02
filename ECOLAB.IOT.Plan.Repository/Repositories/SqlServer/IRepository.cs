namespace ECOLAB.IOT.Plan.Repository.Repositories.SqlServer
{
    using System;
    using System.Data.SqlClient;

    public interface IRepository 
    {
        public string ConnectionString { get;}

        public T Execute<T>(Func<SqlConnection, T> func);

        public T Execute<T>(string connectionString, Func<SqlConnection, T> func);

        public T Execute<T>(string connectionString, Func<SqlConnection, SqlTransaction, T> func);
    }
}
