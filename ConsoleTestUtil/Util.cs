using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Configuration;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace ConsoleTestUtil
{
    public static class Util
    {
        public static string GetConnectionString() 
        {
            //return ConfigurationManager.ConnectionStrings["DBPath"].ConnectionString;
            return ConfigurationSettings.AppSettings["DBPath"];
        }

        public static CommandType GetCommandType(string sql)
        {
            return "select.update.insert".Contains(sql.Trim().ToLower().Split(" ".ToCharArray())[0]) 
                ? CommandType.Text : CommandType.StoredProcedure;
        }

        public static DataTable GetData(string connectionString, string sql, List<SqlParameter> parms = null, bool throwError = true)
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        conn.Open();
                        cmd.CommandType = GetCommandType(sql);

                        if (parms != null)
                            cmd.Parameters.AddRange(parms.ToArray());

                        using (SqlDataReader dr = cmd.ExecuteReader())
                            if (dr != null) dt.Load(dr);
                    }
                }
            }
            catch (Exception ex) { if (throwError) throw ex; }

            return dt;
        }

        public static List<SqlParameter> UpdateData(string connectionString, string sql, List<SqlParameter> parms = null, 
            bool throwError = true)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        conn.Open();
                        cmd.CommandType = GetCommandType(sql);

                        if (parms != null)
                            cmd.Parameters.AddRange(parms.ToArray());

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex) { if (throwError) throw ex; }

            return parms;
        }

        public static Dictionary<string, Int32> MapColumns(DataRow dr) 
        {
            var columnMap = new Dictionary<string, Int32>();

            for (int i = 0; i < dr.Table.Columns.Count; i++)
                columnMap.Add(dr.Table.Columns[i].ColumnName, i);

            return columnMap;
        }

        public static string GetInsertSQL<T>(T bobj)
        {
            var propList = new List<string>();
            var valueList = new List<string>();

            foreach (PropertyInfo pi in bobj.GetType().GetProperties())
            {
                if (pi.Name.ToLower() != "id") 
                {
                    valueList.Add(string.Format("@{0}", pi.Name));
                    propList.Add(string.Format("[{0}]", pi.Name));
                }
            }
            var sql = string.Format("Insert Into dbo.{0}s ({1}) Values ({2}); ", bobj.GetType().Name,
                string.Join(", ", propList.ToArray()), string.Join(", ", valueList.ToArray()));
            
            return sql;
        }

        public static string GetUpdateSQL<T>(T bobj)
        {
            var pairList = new List<string>();

            foreach (PropertyInfo pi in bobj.GetType().GetProperties())
                if (pi.Name.ToLower() != "id") 
                    pairList.Add(string.Format("[{0}] = @{0}", pi.Name));
            
            var sql = string.Format("Update dbo.{0}s Set {1} Where [ID] = @ID; ", bobj.GetType().Name,
                string.Join(", ", pairList.ToArray()));
            
            return sql;
        }

        public static List<SqlParameter> GetSQLParameters<T>(T bobj, string sql) 
        {
            var paramList = new List<SqlParameter>();

            foreach (PropertyInfo pi in bobj.GetType().GetProperties())
            {
                if (sql.ToLower().Contains("@" + pi.Name.ToLower()))
                {
                    if (sql.ToLower().StartsWith("select"))
                    {
                        var makeString = pi.GetValue(bobj) as string;
                        object value = (makeString != null) ? 
                            string.Format("%{0}%", pi.GetValue(bobj)) : pi.GetValue(bobj);
                        paramList.Add(new SqlParameter("@" + pi.Name, value));
                    }
                     else
                    {
                        if (pi.GetValue(bobj) == null)
                        {
                            paramList.Add(new SqlParameter("@" + pi.Name, string.Empty));
                        }
                        else if (IsNumeric(pi.GetValue(bobj).ToString()))
                        {
                            paramList.Add(new SqlParameter("@" + pi.Name, pi.GetValue(bobj)));
                        }
                        else 
                        {
                            var makeString = pi.GetValue(bobj) as string;
                            object value = (makeString != null) ? pi.GetValue(bobj) : string.Empty;
                            paramList.Add(new SqlParameter("@" + pi.Name, value));
                        }
                    }
                }
            }
            return paramList;
        }

        public static string BuildWhereClause<T>(T bobj, T init)
        {
            var conditionList = new List<string>();
            var whereClause = string.Empty;

            PropertyInfo[] propBobj = (from c in bobj.GetType().GetProperties() select c).ToArray();
            PropertyInfo[] propInit = (from c in init.GetType().GetProperties() select c).ToArray();

            for (int i = 0; i < propBobj.Count(); i++) 
            {
                if (propBobj[i].Name.ToLower() == "id" && propBobj[i].GetValue(bobj).ToString() !="0")
                {
                    //special handling for ID column
                    conditionList.Add(string.Format("[ID] = @ID", propBobj[i].Name));
                    break;
                }

                object value = propBobj[i].GetValue(bobj);
                var testForString = value as string;
                if (testForString == null)
                    continue;                   //only using string fields for selection now!

                if (propBobj[i].GetValue(bobj) != propInit[i].GetValue(init))
                    conditionList.Add(string.Format("[{0}] Like @{0}", propBobj[i].Name));
            }

            if (conditionList.Count() > 0)
                whereClause = string.Format("Where {0}", String.Join(" And ", conditionList.ToArray()));

            return whereClause;
        }

        public static bool IsNumeric(string s)
        {
            double myNum = 0;
            if (Double.TryParse(s, out myNum))
            {
                if (s.Contains(",")) return false;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}


//### to return inserted ID ###
//var outputID = new SqlParameter("@ID", SqlDbType.Int);
//outputID.Direction = ParameterDirection.Output;
//var parmsReturn = Util.UpdateData(Util.GetConnectionString(), sql,
//    new List<SqlParameter>() { outputID });

//outputID = parmsReturn[0];

//var id = Int32.Parse(outputID.Value.ToString());            if 

