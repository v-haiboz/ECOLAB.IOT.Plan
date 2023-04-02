namespace ECOLAB.IOT.Plan.Repository.Repositories
{
    using ECOLAB.IOT.Plan.Common.Utilities;
    using ECOLAB.IOT.Plan.Entity.Entities.SqlServer;
    using ECOLAB.IOT.Plan.Entity.SqlServer;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    public class SqlBuilderHelper
    {
        public static string GetPK<T>() where T : class
        {
            string pkName = string.Empty;
            Type objTye = typeof(T);
            PrimaryKeyAttribute pk;
            foreach (Attribute attr in objTye.GetCustomAttributes(true))
            {
                pk = attr as PrimaryKeyAttribute;
                if (pk != null)
                    return pk.Name;
            }
            return pkName;
        }

        public static string UpdateSql<T>(T entity, string tableName) where T : class
        {
            if (entity == null || string.IsNullOrEmpty(tableName))
            {
                return string.Empty;
            }

            string pkName = GetPK<T>();

            if (string.IsNullOrEmpty(pkName))
            {
                return string.Empty;
            }

            string pkValue = string.Empty;

            StringBuilder sb = new StringBuilder();
            sb.Append("update ");
            sb.Append(tableName);
            sb.Append(" set ");
            Type type = entity.GetType();
            PropertyInfo[] props = type.GetProperties();
            List<string> paraList = new List<string>();
            foreach (var prop in props)
            {
                if (prop.Name == (string)pkName)
                {
                    pkValue = (string)prop.GetValue(entity);
                }
                else
                {
                    paraList.Add(GetUpdatePara(prop, entity));
                }
            }

            if (paraList.Count == 0)
            {
                return string.Empty;
            }

            sb.Append(string.Join(",", paraList));

            if (string.IsNullOrEmpty(pkValue))
            {
                throw new Exception("主键不能为空");
            }

            sb.Append(" where ");
            sb.Append(pkName);
            sb.Append(" = ");
            sb.AppendFormat("'{0}'", pkValue);

            return sb.ToString();
        }

        private static string GetUpdatePara<T>(PropertyInfo property, T entity)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" {0}='{1}' ", property.Name, property.GetValue(entity));
            return sb.ToString();
        }

        public static string GenerateInsertSql(ELinkSqlServer eLINKSqlServer,string tableName)
        {

            if (eLINKSqlServer == null)
            {
                return string.Empty;
            }
            string columns = Utility.GetColmons(eLINKSqlServer);
            if (string.IsNullOrEmpty(columns))
            {
                return string.Empty;
            }
            string values = Utility.GetValues(eLINKSqlServer);
            if (string.IsNullOrEmpty(values))
            {
                return string.Empty;
            }
            StringBuilder sql = new StringBuilder();
            sql.Append("Insert into " + tableName);
            sql.Append("(" + columns + ")");
            sql.Append(" values(" + values + ")");

            return sql.ToString();
        }

        public static string GenerateBulkInsertSql(List<ELinkSqlServer> objs, string tableName)
        {
            if (objs == null || objs.Count == 0 || string.IsNullOrEmpty(tableName))
            {
                return string.Empty;
            }
            string columns = Utility.GetColmons(objs[0]);
            if (string.IsNullOrEmpty(columns))
            {
                return string.Empty;
            }
            string values = string.Join(",", objs.Select(p => string.Format("({0})", Utility.GetValues(p))).ToArray());
            StringBuilder sql = new StringBuilder();
            sql.Append("Insert into " + tableName);
            sql.Append("(" + columns + ")");
            sql.Append(" values " + values + "");
            return sql.ToString();
        }
    }
}
