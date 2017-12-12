using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace BackBone
{
    public class PostgreSQLDBInterface : DBInterface
    {
        private string _ConnectionString;
        public PostgreSQLDBInterface(string Constr)
        {
            _ConnectionString = Constr;
        }



        public object ExecuteScalar(string sql)
        {
            NpgsqlConnection conn = new NpgsqlConnection(_ConnectionString);
            try
            {

                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand();
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
                NpgsqlConnection conn = new NpgsqlConnection(_ConnectionString);


                NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);

                cmd.CommandTimeout = 0;

                NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                conn.Close();
                return dt;
            }
            catch (Exception d)
            {
                throw d;
            }
        }


        public int Execute(String sql)
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            conn.ConnectionString = _ConnectionString;
            try
            {

                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand();
                cmd.Connection = conn;
                cmd.CommandTimeout = 0;
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                var aff = cmd.ExecuteNonQuery();
                conn.Close();
                return aff;
            }
            catch (Exception f)
            {
               
                throw (f);
            }
            finally
            {
                conn.Close();
            }

        }


        public ManagedNpgsqlTransaction getManagedSQLTrasaction()
        {

            try
            {
                var tobject = new ManagedNpgsqlTransaction();
                tobject.currentConnection = new NpgsqlConnection(_ConnectionString);
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
            try
            {
                //COPY pop_grid(GRID_ID, POP_TOT, YEAR, METHD_CL, CNTR_CODE, DATA_SRC) from 'C:\...\popgrid.csv' DELIMITERS ',' CSV;
                var filename = table.TableName + "preload.csv";
                filename = @"C:\TAFC\\" + filename;
         

                using (TextWriter writer = new StreamWriter(filename))
                {
                    Rfc4180Writer.WriteDataTable(table, writer, false);

                }

                //foreach (DataColumn column in destdt.Columns)
                //{
                //    fields.Add(column.ColumnName);
                //}

             //   string headers = string.Join("\",\"", fields);

             //   filename = AppDomain.CurrentDomain.BaseDirectory + "\\" + filename;

                using (NpgsqlConnection conn = new NpgsqlConnection(_ConnectionString))
                {
                    conn.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand();
                    cmd.Connection = conn;
                    var sql = "COPY " + destinationTable + " FROM '" + filename + "' DELIMITERS ',' CSV";
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();

                }
            }

            catch (Exception e)
            {
                try
                {
                    var destdt = getData("select * from " + destinationTable + " limit 1");

                    destdt.Clear();

                    foreach (DataRow row in table.Rows)
                    {
                        var newrow = destdt.NewRow();

                        foreach (DataColumn column in table.Columns)
                        {
                            newrow[column.ColumnName] = row[column.ColumnName];
                        }

                        destdt.Rows.Add(newrow);
                    }

                    CopyDataTableToDB(destdt, destinationTable);
                }
                catch
                {
                    throw (e);
                }
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

            string SqlStm = "Select * from sysobjects ";
            SqlStm += " where id = object_id('dbo." + SrchTable.Trim() + "')";
            SqlStm += " and sysstat & 0xf = 3";

            rst = getData(SqlStm);

            if (rst.Rows.Count != 0)
            {

                FindTable = true;
            }

            return FindTable;
        }

        public void CreateTable(string tablename, DataTable dt)
        {
            NpgsqlConnection conn = new NpgsqlConnection(_ConnectionString);
            try
            {
                conn.Open();
                var builtStr = new StringBuilder();
                builtStr.Append("CREATE TABLE \"" + tablename + "\" (");

                int count = 1;
                foreach (DataColumn column in dt.Columns)
                {
                    var datatype = "text COLLATE pg_catalog.\"default\""; var comma = "";

             
                    if (count < dt.Columns.Count)
                    {
                        comma = ",";
                    }

                    count++;

                    builtStr.Append(string.Concat("\"", column.ColumnName.Replace(".", "_"), "\"", "" + datatype + comma));
                }
                builtStr.Append(") WITH ( OIDS = FALSE) TABLESPACE pg_default; ");

                var sql = builtStr.ToString();

                NpgsqlCommand cmd = new NpgsqlCommand(sql);

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
