namespace ECOLAB.IOT.Plan.Repository.Repositories.SqlServer
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using XAct;

    public interface IPlanRepository
    {
        public bool IsExistColumn(string? pTableName, string colName, string connectionStr, string columnTypeName = "datetime2");

        public bool IsExistTable(string? pTableName, string pDB_NAME, string connectionStr);

        public List<string> GetUserTableNameInDB(string connectionStr);

        public bool ValidateSQL(string sql, string connectionStr);

        public int ExecutePlan(string sql, string connectionStr, DateTime expiryTime=default(DateTime));
    }

    public class PlanRepository: IPlanRepository
    {
        public bool IsExistColumn(string? pTableName, string colName, string connectionStr, string columnTypeNames = "datetime2,datetime")
        {

            if (string.IsNullOrEmpty(pTableName) || string.IsNullOrEmpty(colName))
            {
                return false;
            }

            columnTypeNames = columnTypeNames.Replace(",", "','");
            columnTypeNames = string.Format("'{0}'", columnTypeNames);

            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                SqlCommand cmd = connection.CreateCommand();
                string isExitColSql = @$"select count(1) as [Count] from sys.columns where [object_id] = object_id('{pTableName}') and [name] = '{colName}' and TYPE_NAME(system_type_id) in ({columnTypeNames})";
                try
                {
                    cmd.CommandText = isExitColSql;
                    int count = (int)cmd.ExecuteScalar();
                    return count == 1;
                }
                catch (Exception ex)
                {
                    return false;
                }
                finally
                {
                    connection.Close();
                }

            }
        }

        public bool IsExistTable(string? pTableName, string pDB_NAME,string connectionStr)
        {
            if (string.IsNullOrEmpty(pTableName) || string.IsNullOrEmpty(pDB_NAME))
            {
                return false;
            }

            bool bResult;

            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                SqlCommand cmd = connection.CreateCommand();
                string isExitTableSql = @$"select * from [{pDB_NAME}]..sysobjects where name = '{pTableName}'";
                try
                {
                    cmd.CommandText = isExitTableSql;
                    object result = cmd.ExecuteScalar();
                    if (result == null || result == System.DBNull.Value) 
                        return false;
                    bResult = true;
                }
                catch (Exception ex)
                {
                    bResult = false;
                }
                finally
                {
                    cmd.CommandText = "SET PARSEONLY OFF";
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }

                return bResult;
            }
        }

        public List<string> GetUserTableNameInDB(string connectionStr)
        {
            if (string.IsNullOrEmpty(connectionStr))
            {
                return null;
            }

            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                SqlCommand cmd = connection.CreateCommand();
                string sql = @$"select name from sysobjects where xtype='U'";
                var list = new List<string>();
                try
                {
                    cmd.CommandText = sql;
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(reader["name"].ToString());
                        }
                    }

                    return list;
                }
                catch (Exception ex)
                {
                    return list;
                }
                finally
                {
                    cmd.CommandText = "SET PARSEONLY OFF";
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }

            }
        }

        public bool ValidateSQL(string sql, string connectionStr)
        {
            bool bResult;
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                connection.Open();
                SqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = "SET PARSEONLY ON";
                cmd.ExecuteNonQuery();
                try
                {
                    cmd.CommandText = sql;
                    var result= cmd.ExecuteNonQuery();
                    bResult = true;
                }
                catch (Exception ex)
                {
                    bResult = false;
                }
                finally
                {
                    cmd.CommandText = "SET PARSEONLY OFF";
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
            }

            return bResult;
        }

        public int ExecutePlan(string sql, string connectionStr, DateTime expiryTime=default(DateTime))
        {
            var commandTimeout = 60;
            if (expiryTime != default(DateTime))
            {
                var nowUtc = DateTime.UtcNow;
                if (nowUtc >= expiryTime)
                {
                    return -1;
                }

                var timeSpan = expiryTime.Subtract(nowUtc);
                commandTimeout = (int)timeSpan.TotalSeconds;
            }
           
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                connection.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand(sql, connection);
                    cmd.CommandTimeout = commandTimeout;
                    return cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    return -1;
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
}
