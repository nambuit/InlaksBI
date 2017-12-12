using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace BackBone
{
    public class SQLServerDBInterfac : DBInterface
    {
        private string _ConnectionString;
        public SQLServerDBInterfac(string Constr)
        {
            _ConnectionString = Constr;
        }



        public object ExecuteScalar(string sql)
        {
            SqlConnection conn = new SqlConnection(_ConnectionString);
            try
            {

                conn.Open();
                SqlCommand cmd = new SqlCommand();
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
                SqlConnection conn = new SqlConnection(_ConnectionString);


                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.CommandTimeout = 0;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
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
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = _ConnectionString;
            try
            {

                conn.Open();
                SqlCommand cmd = new SqlCommand();
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
                conn.Close();
                throw (f);
            }

        }


        public ManagedSqlTransaction getManagedSQLTrasaction()
        {

            try
            {
                var tobject = new ManagedSqlTransaction();
                tobject.currentConnection = new SqlConnection(_ConnectionString);
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

            var conn = new SqlConnection(_ConnectionString);

            SqlBulkCopy bulkcopy = new SqlBulkCopy(conn);

            try
            {
                conn.Open();

                bulkcopy = new SqlBulkCopy(conn);

                bulkcopy.BulkCopyTimeout = 0;

                bulkcopy.DestinationTableName = destinationTable;
                bulkcopy.WriteToServer(table);
                bulkcopy.Close();
                conn.Close();
            }
            catch (Exception e)
            {
                bulkcopy.Close();
                conn.Close();
                throw (e);
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

        }
    }
}
