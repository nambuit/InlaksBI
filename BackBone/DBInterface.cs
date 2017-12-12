using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using CsvHelper;
using System.IO;
using Microsoft.Practices.EnterpriseLibrary.Data.Oracle;
using System.Web;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using MySql.Data.MySqlClient;
using System.Linq;

namespace BackBone
{
    public interface DBInterface
    {


        object ExecuteScalar(string sql);


        DataTable getData(String sql);



        int Execute(String sql);

        




        List<ValuePair> getValuePair(string id, string value, string sql);



        void CopyDataTableToDB(DataTable table, string destinationTable);



        DataColumn createDataColumn(string columnName);


        DataColumn createDataColumn(string columnName, string type);




        bool FindTable(string TableName);



        void CreateTable(string tablename, DataTable dt);



    }
}
