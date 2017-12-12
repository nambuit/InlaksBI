using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace BackBone
{
    public class OracleDBInterface : DBInterface
    {
        private string _ConnectionString;
        public OracleDBInterface(string Constr)
        {
            _ConnectionString = Constr;
        }



        public object ExecuteScalar(string sql)
        {
            OracleConnection conn = new OracleConnection(_ConnectionString);
            try
            {

                conn.Open();
                OracleCommand cmd = new OracleCommand();
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
                OracleConnection conn = new OracleConnection(_ConnectionString);


                OracleCommand cmd = new OracleCommand(sql, conn);

                cmd.CommandTimeout = 0;

                OracleDataAdapter da = new OracleDataAdapter(cmd);
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
            OracleConnection conn = new OracleConnection();
            conn.ConnectionString = _ConnectionString;
            try
            {

                conn.Open();
                OracleCommand cmd = new OracleCommand();
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


        public ManagedOracleTransaction getManagedSQLTrasaction()
        {

            try
            {
                var tobject = new ManagedOracleTransaction();
                tobject.currentConnection = new OracleConnection(_ConnectionString);
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
