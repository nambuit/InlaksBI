using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace BackBone
{
    public class MySQLDBInterface:DBInterface
    {
        private string _ConnectionString;
        public MySQLDBInterface(string Constr)
        {
            _ConnectionString = Constr;
        }



        public object ExecuteScalar(string sql)
        {
            MySqlConnection conn = new MySqlConnection(_ConnectionString);
            try
            {

                conn.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandTimeout = 0;
                cmd.Connection = conn;
                cmd.CommandText = sql;

                return cmd.ExecuteScalar();
            }
            catch (Exception a)
            {
                throw (a);
            }
            finally
            {
                conn.Close();
            }

        }



        public DataTable getData(String sql)
        {
            try
            {
                MySqlConnection conn = new MySqlConnection(_ConnectionString);
                

                MySqlCommand cmd = new MySqlCommand(sql, conn);

                cmd.CommandTimeout = 0;

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                conn.Close();
                return dt;
            }
            catch(Exception d)
            {
                throw d;
            }

        }


        public int Execute(String sql)
        {
            MySqlConnection conn = new MySqlConnection();
            conn.ConnectionString = _ConnectionString;
            try
            {

                conn.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandTimeout = 0;
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
              var aff =  cmd.ExecuteNonQuery();
                conn.Close();
                return aff;
            }
            catch (Exception f)
            {                
                conn.Close();
                throw (f);
            }

        }


        public ManagedMySQLTransaction getManagedSQLTrasaction()
        {

            try
            {
                var tobject = new ManagedMySQLTransaction();
                tobject.currentConnection = new MySqlConnection(_ConnectionString);
                tobject.currentConnection.Open();
                tobject.currentCommand = tobject.currentConnection.CreateCommand();
                tobject.currentCommand.CommandTimeout = 0;
                tobject.currentTransaction = tobject.currentConnection.BeginTransaction();
                tobject.currentCommand.Connection = tobject.currentConnection;
                tobject.currentCommand.Transaction = tobject.currentTransaction;

                return tobject;
            }
            catch (Exception s)
            {
                throw (s);
            }
        }

        public List<ValuePair> getValuePair(string id, string value, string sql)
        {


            var pair = new List<ValuePair>();
            try
            {



                var data = getData(sql);


                foreach (DataRow row in data.Rows)
                {
                    ValuePair valuePair = new ValuePair();
                    valuePair.ID = row[id].ToString();
                    valuePair.Value = row[value].ToString();

                    pair.Add(valuePair);
                }

                return pair;

            }
            catch (Exception e)
            {
                throw (e);
            }

        }


        public void CopyDataTableToDB(DataTable table, string destinationTable)
        {
            var filename = table.TableName + "preload.csv";
            using (TextWriter writer = new StreamWriter(filename))
            {
                Rfc4180Writer.WriteDataTable(table, writer, false);

            }

            using (MySqlConnection conn = new MySqlConnection(_ConnectionString))
            {
                conn.Open();


                //  CreateTable(dt.TableName, dt, conn);

                var bl = new MySqlBulkLoader(conn);
                bl.TableName = destinationTable;
                bl.FieldTerminator = ",";
                bl.FieldQuotationCharacter = '"';
                bl.LineTerminator = "\r\n";
                bl.FileName = filename;
                // bl.NumberOfLinesToSkip = 1;
                bl.Load();
                File.Delete(filename);
            }
        }

        public DataColumn createDataColumn(string columnName)
        {
            DataColumn cm = new DataColumn();
            cm.DataType = Type.GetType("System.String");
            cm.ColumnName = columnName;
            cm.Caption = columnName;
            return cm;
        }

        public DataColumn createDataColumn(string columnName, string type)
        {
            DataColumn cm = new DataColumn();
            cm.DataType = Type.GetType(type);
            cm.ColumnName = columnName;
            cm.Caption = columnName;
            return cm;
        }

        public bool FindTable(string SrchTable)
        {

            DataTable rst;
            bool FindTable = false;

            string SqlStm = "SELECT  table_schema, table_name from information_schema.TABLES where table_name='"+SrchTable.Trim()+"'"; 

            rst = getData(SqlStm);

            if (rst.Rows.Count != 0)
            {

                FindTable = true;
            }

            return FindTable;
        }


        public void CreateTable(string tablename, DataTable dt)
        {
            MySqlConnection conn = new MySqlConnection(_ConnectionString);
            try
            {
                
                var builtStr = new StringBuilder();
                builtStr.Append("CREATE TABLE `" + tablename + "` (");

                int count = 1;
                foreach (DataColumn column in dt.Columns)
                {
                    var datatype = "longtext"; var comma = "";

                    if (column.ColumnName.Equals("ID"))
                    {
                        datatype = "varchar(40) NOT NULL";

                    }
                    if (count < dt.Columns.Count)
                    {
                        comma = ",";
                    }

                    count++;

                    builtStr.Append(string.Concat("`", column.ColumnName.Replace(".", "_"), "` ", "" + datatype + comma));
                }
                builtStr.Append(") ENGINE = InnoDB AUTO_INCREMENT = 2 DEFAULT CHARSET = utf8");

                var sql = builtStr.ToString();

                MySqlCommand cmd = new MySqlCommand(sql);

                cmd.Connection = conn;

                cmd.ExecuteNonQuery();



            }
            catch (Exception w)
            {
                throw (w);
            }
            finally
            {
                conn.Close();
            }


        }



    }
}
