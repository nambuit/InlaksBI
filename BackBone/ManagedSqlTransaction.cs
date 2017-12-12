using MySql.Data.MySqlClient;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace BackBone
{
  public  class ManagedSqlTransaction
    {
        public SqlConnection currentConnection { get; set; }

        public SqlTransaction currentTransaction { get; set; }

        public SqlCommand currentCommand { get; set; }
    }


  public  class ManagedOracleTransaction
    {
        public OracleConnection currentConnection { get; set; }

        public OracleTransaction currentTransaction { get; set; }

        public OracleCommand currentCommand { get; set; }

  
    }

    public class ManagedNpgsqlTransaction
    {
        public NpgsqlConnection currentConnection { get; set; }

        public NpgsqlTransaction currentTransaction { get; set; }

        public NpgsqlCommand currentCommand { get; set; }


    }

    public class ManagedMySQLTransaction
    {
        public MySqlConnection currentConnection { get; set; }

        public MySqlTransaction currentTransaction { get; set; }

        public MySqlCommand currentCommand { get; set; }


    }

}
