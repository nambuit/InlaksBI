using BackBone;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace BackBone
{
   public static class Extraction
    {
        static String getString(string value)
        {
            try
            {
                return ((value == null) ? "" : value.Replace("'",""));
            }
            catch 
            {
                return string.Empty;
            }
        }


        static bool isNull(String value)
        {

            return (value == null || value.Equals(string.Empty));
        }








        static string getNaration(InterfaceDefinition def, DataRow rs)
        {
            StringBuilder naration = new StringBuilder();
            string type = def.narationType.Trim();
            try {
                switch (type.ToLower())
                {


                    case "visaprepaid":
                        var prefixes = def.naration.Split('@');
                        var part = getString(rs["tran_particular"].ToString());
                        var part2 = getString(rs["tran_particular_2"].ToString());
                        var remarks = getString(rs["tran_rmks"].ToString());
                        var ref_num = getString(rs["ref_num"].ToString());
                        var reference = string.Empty;
                        var Details = "";

                        try
                        {
                            if (part.Contains("/"))
                            {

                                var stringarray = part.Split('/');
                                var trans_class = stringarray[0];
                                if (trans_class.CleanUp().Contains("cla") || trans_class.CleanUp().Contains("clf") || trans_class.CleanUp().Contains("atm"))
                                {
                                    reference = stringarray[3];
                                    reference = prefixes[0] + reference;
                                }
                                else { reference = ""; }



                            }
                            else
                            {

                                var stringarray = part.Split(' ');
                                if (stringarray[0].ToString().Trim().ToLower().Contains("tudaf"))
                                {
                                    reference = stringarray[2];
                                    reference = prefixes[0] + reference;
                                }
                                else { reference = ""; }

                            }
                        }
                        catch { }

                        Details = reference + part + " " + part2 + " " + (remarks.isNull()?"":prefixes[1]+remarks) + " " + (ref_num.isNull()?"":prefixes[2]+ref_num);

                        naration.Append(Details);
                  
                        break;

                    case "etranzact":
                        var prefix= def.naration.Split('@');
                        var tranpart = getString(rs["tran_particular_2"].ToString()).isNull()?getString(rs["tran_particular"].ToString()): getString(rs["tran_particular_2"].ToString());

                        naration.Append((prefix.Count() > 0 ? prefix[0] : def.naration) + " ").Append(tranpart).Append(getString(rs["tran_rmks"].ToString()));


                        break;

                    case "upsl":
                        var field = getString(rs["tran_particular"].ToString());

                        if (!field.CleanUp().StartsWith("pos"))
                        {
                            goto default;
                        }

                        var values = field.Split("/".ToCharArray(),StringSplitOptions.RemoveEmptyEntries);

                        var value = string.Empty;

                        if(values.Length > 3)
                        {
                            value = values[3].Trim();
                        }

                        if (!value.isNull())
                        {
                            value = "APR" + value;
                        }

                        naration = getBuiltString(def.naration, rs);

                        return value +" "+ naration.ToString();

                    case "chargeback":

                        var Field = getString(rs["tran_particular"].ToString());



                        var Values = Field.Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                        var Value = string.Empty;

                        if (Values.Length > 2)
                        {
                            Value = Values[1].Trim();

                            if (Value.Contains("-"))
                            {
                                Value = Value.Split('-')[0];
                            }

                        }
                       
                        if (!Value.isNull())
                        {
                            Value = "SRN" + Value;
                        }

                        naration = getBuiltString(def.naration, rs);

                        return Value +" "+ naration.ToString();


                    case "replacespace":
                        naration = getBuiltString(def.naration, rs);

                       naration = naration.Replace(" ", "");

                        break;


                    default:
                        naration = getBuiltString(def.naration, rs);

                        break;

                }
            } catch (Exception e) {
                throw new Exception("Interface Naration not properly Defined because: " + e.Message, e);
            }
            return naration.ToString();
        }


        static void getUDFs(DataRow rs, InterfaceDefinition def)
        {
            try
            {
                def.ud1Col = (string.IsNullOrEmpty(def.ud1Col) ? "normal" : def.ud1Col);

                switch (def.ud1Col.Trim().ToLower())
                {

                    case "1102":
                        rs["ud1"] =  getString(rs["TRAN_PARTICULAR_2"].ToString()).Substring(1, 12);
                        rs["ud2"] =  getString(rs["tran_particular"].ToString());
                        break;


                    default:
                        rs["ud1"] = ((def.ud1Col.ToLower().Equals("normal")) ? "" : getBuiltString(def.ud1Col, rs).ToString());
                        rs["ud2"] = ((isNull(def.ud2Col) ? "" : getBuiltString(def.ud2Col, rs).ToString()));
                        rs["ud3"] = ((isNull(def.ud3Col) ? "" : getBuiltString(def.ud3Col, rs).ToString()));
                        rs["ud4"] = ((isNull(def.ud4Col) ? "" : getBuiltString(def.ud4Col, rs).ToString()));
                        rs["ud5"] = ((isNull(def.ud5Col) ? "" : getBuiltString(def.ud5Col, rs).ToString()));
                        break;

                }

              

            }
            catch(Exception r) 
            {
               Utils.Log("UDFs not properly built up because: "+r.Message);
              
            }

        }


        public static string ExtractData(AccountDetails details, string CBAConstr, string ClirecConstr, string CBAType)
        {
            string uuid = Guid.NewGuid().ToString();
            DataTable data = new DataTable();
            try {
                var db = new DBConnector(CBAConstr, ClirecConstr);

                Utils.Log("Beginning Data Etraction for (" + details.AccountCode + ") using Interface definition ID (" + details.DefinitionID + ")");

                InterfaceDefinition def = db.getDefinition(details.DefinitionID);

                string mappedAccts = ""; var map = new StringBuilder(); int ct = 1;

                foreach(string cba in details.CBAccount)
                {
                    map.Append("'").Append(cba).Append("'");
                    
                    if (ct < details.CBAccount.Count) { map.Append(","); }
                    ct++;
                }
                mappedAccts = map.ToString();

                string sql = def.script.Replace("startdate", getDate(details.LastLedgerDate)).
                    Replace("enddate", details.Endate).Replace("acctid", mappedAccts).
                    Replace("acctCcode",details.currency).Replace("acctBcode",details.CbaBranchCode);

                Utils.Log("Connecting to transaction Database");
                Utils.Log(sql);




                switch (CBAType.CleanUp())
                {
                    case "mysql":
                        data = db.fetchMySQLCBA(sql);
                        break;

                    case "sqlserver":
                        data = db.getSQLServerCBA(sql);
                        break;

                    default:
                        data = db.fetchCBA(sql);
                        break;
                }


                int nofT = data.Rows.Count;
                bool isData = nofT > 0;
                Utils.Log(nofT + " transaction(s) fetched");
                int sn = 1;

                data = Utils.AddExtractionColumns(data);
               
                foreach (DataRow row in data.Rows)
                {
                   

                    row["PostDate"] = row[def.postDateCol].toDateTime();

                    row["Valdate"] = row[def.valDateCol].toDateTime();

                    bool isDebit = row[def.directionCol].ToString().ToUpper().Equals("D");

                    row["CrDr"] = (isDebit ? "1" : "2");

                    row["Debits"] = (isDebit ? row[def.amountCol].toDecimal() : decimal.Zero);

                    row["Credits"] = (isDebit ? decimal.Zero : row[def.amountCol].toDecimal());

                    row["Amount"] = row["Credits"].toDecimal() + row["Debits"].toDecimal();

                    getUDFs(row, def);

                    row["Details"] = getNaration(def, row);

                    row["SN"] = sn++;
                    row["Id"] = uuid;
                    row["username"] = "LedgerDownload";

                }
                var balance = decimal.Zero;
                if (isData)
                {

                    string[] selected = new[] { "SN", "PostDate", "Valdate", "Details", "Debits", "Credits", "Amount", "CrDr", "ud1", "ud2", "ud3", "ud4", "ud5", "Id", "username" };



                    data = new DataView(data).ToTable(false, selected);

                  



                    Utils.Log("Attempting to Fetch Account Balance");
                    
                    foreach (string cba in details.CBAccount)
                    {
                        sql = def.balScript.Replace("startdate", Utils.getFirstDayofMonth(details.CurrentDate))
                            .Replace("enddate", details.Endate).Replace("acctid", cba).
                             Replace("acctCcode", details.currency).Replace("acctBcode", details.CbaBranchCode);

                        var baldata = new DataTable();


                        switch (CBAType.CleanUp())
                        {
                            case "mysql":
                                baldata = db.fetchMySQLCBA(sql);
                                break;

                            case "sqlserver":
                                data = db.getSQLServerCBA(sql);
                                break;

                            default:
                                baldata = db.fetchCBA(sql);
                                break;
                        }

                        if (baldata.Rows.Count > 0)
                        {
                            balance += baldata.Rows[0][def.balCol].toDecimal();
                        }
                    }
                    Utils.Log(sql);
                    Utils.Log("Balance downloaded successfully and = " + balance.ToString("0.00"));
                    db.CopyDataTableToDB(data, "ExtractionTemp");
                }

                var ID = uuid;
               

                return ((isData)?ID + "$" + balance.ToString("0.00"): "No Data Pulled!");
            } catch (Exception e){
                throw new Exception("CBA Download failed for "+details.AccountName+"("+details.AccountCode+") because: "+e.Message);
            }
            finally
            {
                data.Clear();
                data.Dispose();
            }

            }


        public static string ExtractData(AccountDetails details, string CBAConstr, string ClirecConstr, HttpContext context, string CBAType, bool isNIP, string NIPsession)
        {
            string uuid = Guid.NewGuid().ToString();
            DataTable data = new DataTable();
            Utils.Log(CBAConstr);
            try
            {
                var db = new DBConnector(CBAConstr, ClirecConstr);

                Utils.Log("Beginging Data Etraction for (" + details.AccountCode + ") using Interface definition ID (" + details.DefinitionID + ")");

                InterfaceDefinition def = db.getDefinition(details.DefinitionID);

                string mappedAccts = ""; var map = new StringBuilder(); int ct = 1;

                foreach (string cba in details.CBAccount)
                {
                    map.Append("'").Append(cba).Append("'");

                    if (ct < details.CBAccount.Count) { map.Append(","); }
                    ct++;
                }
                mappedAccts = map.ToString();

                //string sql = def.script.Replace("startdate", getDate(details.LastLedgerDate)).
                //    Replace("enddate", details.Endate).Replace("acctid", mappedAccts).
                //    Replace("acctCcode", details.currency).Replace("acctBcode", details.CbaBranchCode);

                Utils.Log("Connecting to transaction Database");


                var sql = "";

                string Startdate, Enddate;

                if (isNIP)
                {
                    switch (NIPsession.toInt()) {

                        case 1:
                    Startdate = details.LastLedgerDate.ToString("dd-MMM-yy") + " 2.00.00 PM";
                    Enddate = details.Endate.toDateTime().ToString("dd-MMM-yy") + " 11.59.59 PM";

                   break;

                       default:
                  Startdate = details.LastLedgerDate.ToString("dd-MMM-yy") + " 12.00.00 AM";
                  Enddate = details.Endate.toDateTime().ToString("dd-MMM-yy") + " 1.59.59 PM";
                            var newdef = db.getDefinition(def.narationType.Trim());

                            def.script = newdef.script;
                            break;
                }

                }
                else
                {
                    Startdate = getDate(details.LastLedgerDate);
                    Enddate = details.Endate.toDateTime().ToString("dd-MMM-yyyy");
                }


                switch (CBAType.CleanUp())
                {
                    case "mysql":
                   sql = def.script.Replace("startdate", details.LastLedgerDate.ToString("yyyy-MM-dd")).
                   Replace("enddate", Convert.ToDateTime(details.Endate).ToString("yyyy-MM-dd")).Replace("acctid", mappedAccts).
                   Replace("acctCcode", details.currency).Replace("acctBcode", details.CbaBranchCode).Replace("MonthlyTable",details.CurrentDate.ToString("MMMyyyy")+"Journals");
                        Utils.Log(sql);
                        data = db.fetchMySQLCBA(sql);
                        break;

                    case "sqlserver":
                        sql = def.script.Replace("startdate", Startdate).
                  Replace("enddate", Enddate).Replace("acctid", mappedAccts).
                  Replace("acctCcode", details.currency).Replace("acctBcode", details.CbaBranchCode).Replace("MonthlyTable", details.CurrentDate.ToString("MMMyyyy") + "Journals"); ;
                        Utils.Log(sql);
                        data = db.getSQLServerCBA(sql);
                        break;

                    default:
                        sql = def.script.Replace("startdate", Startdate).
                   Replace("enddate", Enddate).Replace("acctid", mappedAccts).
                   Replace("acctCcode", details.currency).Replace("acctBcode", details.CbaBranchCode).Replace("MonthlyTable", details.CurrentDate.ToString("MMMyyyy") + "Journals"); ;
                        Utils.Log(sql);
                        data = db.fetchCBA(sql);
                        break;
                }
               

                int nofT = data.Rows.Count;
                bool isData = nofT > 0;
                Utils.Log(nofT + " transaction(s) fetched");
                int sn = 1;

                data = Utils.AddExtractionColumns(data);

                foreach (DataRow row in data.Rows)
                {


                    row["PostDate"] = row[def.postDateCol].toDateTime();

                    row["Valdate"] = row[def.valDateCol].toDateTime();

                    bool isDebit = row[def.directionCol].ToString().ToUpper().Equals("D");

                    row["CrDr"] = (isDebit ? "1" : "2");

                    row["Debits"] = (isDebit ? row[def.amountCol].toDecimal() : decimal.Zero);

                    row["Credits"] = (isDebit ? decimal.Zero : row[def.amountCol].toDecimal());

                    row["Amount"] = row["Credits"].toDecimal() + row["Debits"].toDecimal();

                    getUDFs(row, def);

                    row["Details"] = getNaration(def, row);

                    row["SN"] = sn++;
                    row["Id"] = uuid;
                    row["username"] = "LedgerDownload";

                }
                var balance = decimal.Zero;
                if (isData)
                {

                    if (isNIP && NIPsession.toInt() == 2)
                    {

                    }
                    else
                    {
                        Utils.Log("Attempting to Fetch Account Balance");

                        OracleConnection conn = new OracleConnection();

                        bool logged = false;

                        foreach (string cba in details.CBAccount)
                        {


                            DataTable baldata;


                            switch (CBAType.CleanUp())
                            {
                                case "mysql":
                                    sql = def.balScript.Replace("startdate", Convert.ToDateTime(Utils.getFirstDayofMonth(details.CurrentDate)).ToString("yyyy-MM-dd"))
                               .Replace("enddate", Convert.ToDateTime(details.Endate).ToString("yyyy-MM-dd")).Replace("acctid", cba).
                                Replace("acctCcode", details.currency).Replace("acctBcode", details.CbaBranchCode);
                                    if (!logged)
                                    {
                                        Utils.Log(sql);
                                        logged = true;
                                    }
                                    baldata = db.fetchMySQLCBA(sql);
                                    break;

                                case "sqlserver":
                                    sql = def.balScript.Replace("startdate", Utils.getFirstDayofMonth(details.CurrentDate))
                               .Replace("enddate", Enddate).Replace("acctid", cba).
                                Replace("acctCcode", details.currency).Replace("acctBcode", details.CbaBranchCode).Replace("BalTable", details.CurrentDate.ToString("MMMyyyy") + "glbal");
                                    if (!logged)
                                    {
                                        Utils.Log(sql);
                                        logged = true;
                                    }
                                    baldata = db.getSQLServerCBA(sql);
                                    break;

                                default:
                                    conn.ConnectionString = CBAConstr;
                                    OracleCommand cmd = new OracleCommand();
                                    cmd.Connection = conn;
                                    cmd.CommandType = CommandType.Text;
                                    cmd.CommandTimeout = 0;
                                    sql = def.balScript.Replace("startdate", Utils.getFirstDayofMonth(details.CurrentDate))
                               .Replace("enddate", Enddate).Replace("acctid", cba).
                                Replace("acctCcode", details.currency).Replace("acctBcode", details.CbaBranchCode).Replace("BalTable", details.CurrentDate.ToString("MMMyyyy") + "glbal");
                                    if (!logged)
                                    {
                                        Utils.Log(sql);
                                        logged = true;
                                    }
                                    cmd.CommandText = sql; 
                                    baldata = db.fetchCBA(cmd);

                                    break;
                            }





                            if (baldata.Rows.Count > 0)
                            {
                                balance += baldata.Rows[0][def.balCol].toDecimal();
                            }
                        }
                        //  Utils.Log(sql);
                        Utils.Log("Balance downloaded successfully and = " + balance.ToString("0.00"));
                        conn.Close();
                        conn.Dispose();

                    }

                    string[] selected = new[] { "SN", "PostDate", "Valdate", "Details", "Debits", "Credits", "Amount", "CrDr", "ud1", "ud2", "ud3", "ud4", "ud5", "Id", "username" };



                    data = new DataView(data).ToTable(false, selected);


                    context.Session["TotalExempted"] = 0;
                    context.Session["side"] = "L";
                    context.Session["TotalExtraction"] = data.Rows.Count;

                    context.Session["ToatlCredits"] = Convert.ToDecimal(data.Compute("Sum(Credits)", ""));
                    context.Session["ToatlDebits"] = Convert.ToDecimal(data.Compute("Sum(Debits)", ""));
                    context.Session["latest"] = Convert.ToDateTime(data.Compute("Max(PostDate)", ""));
                    context.Session["CreditCount"] =  data.Compute("count(CrDr)", "CrDr ='2' ").toInt();
                    context.Session["DebitCount"] =   data.Compute("count(CrDr)", "CrDr ='1' ").toInt();
                    var fp = from row in data.AsEnumerable() where Convert.ToInt32(row["SN"]) <= 1000 orderby row["SN"] select row;

                    context.Session["FirstPage"] = fp.CopyToDataTable();



                    db.CopyDataTableToDB(data, "ExtractionTemp");
                }

                var ID = uuid;


                return ((isData) ? ID + "$" + balance.ToString("0.00") : "No Data Pulled!");
            }
            catch (Exception e)
            {
                throw new Exception("CBA Download failed for " + details.AccountName + "(" + details.AccountCode + ") because: " + e.Message);
            }
            finally
            {
                data.Clear();
                data.Dispose();
            }

        }


        public static string ExtractStmtData(AccountDetails details, string CBAConstr, string ClirecConstr, HttpContext context, string CBAType, string MsgType)
        {
            string uuid = Guid.NewGuid().ToString();
            DataTable data = new DataTable();
            try
            {
                var db = new DBConnector(CBAConstr, ClirecConstr);

                Utils.Log("Beginging Data Extraction for (" + details.AccountCode + ") using Interface definition ID (" + details.DefinitionID + ")");

                InterfaceDefinition def = db.getDefinition(details.DefinitionID);

                string mappedAccts = ""; var map = new StringBuilder(); int ct = 1;

                foreach (string cba in details.StmtCBAccount)
                {
                    map.Append("'").Append(cba).Append("'");

                    if (ct < details.StmtCBAccount.Count) { map.Append(","); }
                    ct++;
                }
                mappedAccts = map.ToString();

                //string sql = def.script.Replace("startdate", getDate(details.LastLedgerDate)).
                //    Replace("enddate", details.Endate).Replace("acctid", mappedAccts).
                //    Replace("acctCcode", details.currency).Replace("acctBcode", details.CbaBranchCode);

                Utils.Log("Connecting to transaction Database");


                var sql = "";

                string Startdate, Enddate;

               
              
                    Startdate = getDate(details.LastStmtDate);
                    Enddate = details.Endate.toDateTime().ToString("dd-MMM-yyyy");
           


                switch (CBAType.CleanUp())
                {
                    case "mysql":
                        sql = def.script.Replace("startdate", details.LastStmtDate.ToString("yyyy-MM-dd")).
                        Replace("enddate", Convert.ToDateTime(details.Endate).ToString("yyyy-MM-dd")).Replace("acctid", mappedAccts).
                        Replace("acctCcode", details.currency).Replace("acctBcode", details.CbaBranchCode).Replace("mtype",MsgType);
                        Utils.Log(sql);
                        data = db.fetchMySQLCBA(sql);
                        break;

                    case "sqlserver":
                        sql = def.script.Replace("startdate", Startdate).
                  Replace("enddate", Enddate).Replace("acctid", mappedAccts).
                  Replace("acctCcode", details.currency).Replace("acctBcode", details.CbaBranchCode).Replace("mtype", MsgType);
                        Utils.Log(sql);
                        data = db.getSQLServerCBA(sql);
                        break;

                    default:
                        sql = def.script.Replace("startdate", Startdate).
                   Replace("enddate", Enddate).Replace("acctid", mappedAccts).
                   Replace("acctCcode", details.currency).Replace("acctBcode", details.CbaBranchCode).Replace("mtype", MsgType);
                        Utils.Log(sql);
                        data = db.fetchCBA(sql);
                        break;
                }


                int nofT = data.Rows.Count;
                bool isData = nofT > 0;
                Utils.Log(nofT + " transaction(s) fetched");
                int sn = 1;

                data = Utils.AddExtractionColumns(data);

                foreach (DataRow row in data.Rows)
                {


                    row["PostDate"] = row[def.postDateCol].toDateTime();

                    row["Valdate"] = row[def.valDateCol].toDateTime();

                    bool isDebit = row[def.directionCol].ToString().ToUpper().Equals("D");

                    row["CrDr"] = (isDebit ? "1" : "2");

                    row["Debits"] = (isDebit ? row[def.amountCol].toDecimal() : decimal.Zero);

                    row["Credits"] = (isDebit ? decimal.Zero : row[def.amountCol].toDecimal());

                    row["Amount"] = row["Credits"].toDecimal() + row["Debits"].toDecimal();

                    getUDFs(row, def);

                    row["Details"] = getNaration(def, row);

                    row["SN"] = sn++;
                    row["Id"] = uuid;
                    row["username"] = "LedgerDownload";

                }
                var balance = decimal.Zero;
                if (isData)
                {

            
                        Utils.Log("Attempting to Fetch Account Balance");
                    SqlConnection conn = new SqlConnection();
             

                    foreach (string cba in details.StmtCBAccount)
                        {


                            DataTable baldata;


                            switch (CBAType.CleanUp())
                            {
                                case "mysql":
                                    sql = def.balScript.Replace("startdate", Convert.ToDateTime(Utils.getFirstDayofMonth(details.CurrentDate)).ToString("yyyy-MM-dd"))
                               .Replace("enddate", Convert.ToDateTime(details.Endate).ToString("yyyy-MM-dd")).Replace("acctid", cba).
                                Replace("acctCcode", details.currency).Replace("acctBcode", details.CbaBranchCode);
                                    baldata = db.fetchMySQLCBA(sql);
                                    break;

                                case "sqlserver":
                                conn.ConnectionString = CBAConstr;
                                SqlCommand cmd = new SqlCommand();
                                cmd.Connection = conn;
                                cmd.CommandType = CommandType.Text;
                                cmd.CommandTimeout = 0;
                                sql = def.balScript.Replace("startdate", Utils.getFirstDayofMonth(details.CurrentDate))                               
                               .Replace("enddate", Enddate).Replace("acctid", cba).
                                Replace("acctCcode", details.currency).Replace("acctBcode", details.CbaBranchCode);
                                Utils.Log(sql);
                                cmd.CommandText = sql;
                                baldata = db.fetchCBA(cmd);
                                break;

                                default:
                                    sql = def.balScript.Replace("startdate", Utils.getFirstDayofMonth(details.CurrentDate))
                               .Replace("enddate", Enddate).Replace("acctid", cba).
                                Replace("acctCcode", details.currency).Replace("acctBcode", details.CbaBranchCode);
                                    baldata = db.fetchCBA(sql);
                                    break;
                            }





                            if (baldata.Rows.Count > 0)
                            {
                                balance += baldata.Rows[0][def.balCol].toDecimal();
                            }
                      
                       // Utils.Log(sql);
                        Utils.Log("Balance downloaded successfully and = " + balance.ToString("0.00"));
                        conn.Close();


                    }

                    string[] selected = new[] { "SN", "PostDate", "Valdate", "Details", "Debits", "Credits", "Amount", "CrDr", "ud1", "ud2", "ud3", "ud4", "ud5", "Id", "username" };



                    data = new DataView(data).ToTable(false, selected);


                    context.Session["TotalExempted"] = 0;
                    context.Session["side"] = "S";
                    context.Session["TotalExtraction"] = data.Rows.Count;

                    context.Session["ToatlCredits"] = Convert.ToDecimal(data.Compute("Sum(Credits)", ""));
                    context.Session["ToatlDebits"] = Convert.ToDecimal(data.Compute("Sum(Debits)", ""));
                    context.Session["latest"] = Convert.ToDateTime(data.Compute("Max(PostDate)", ""));
                    context.Session["CreditCount"] = data.Compute("count(CrDr)", "CrDr ='2' ").toInt();
                    context.Session["DebitCount"] = data.Compute("count(CrDr)", "CrDr ='1' ").toInt();
                    var fp = from row in data.AsEnumerable() where Convert.ToInt32(row["SN"]) <= 1000 orderby row["SN"] select row;

                    context.Session["FirstPage"] = fp.CopyToDataTable();



                    db.CopyDataTableToDB(data, "ExtractionTemp");
                }

                var ID = uuid;


                return ((isData) ? ID + "$" + balance.ToString("0.00") : "No Data Pulled!");
            }
            catch (Exception e)
            {
                throw new Exception("CBA Download failed for " + details.AccountName + "(" + details.AccountCode + ") because: " + e.Message);
            }
            finally
            {
                data.Clear();
                data.Dispose();
            }

        }


        public static String getDate(DateTime date)
        {
            return date.ToString("dd-MMM-yyyy");
        }


        public static StringBuilder getBuiltString(string formula, DataRow rs)
        {
            StringBuilder naration = new StringBuilder();

            try {

                var columns = formula.Split('@');

                foreach (string column in columns)
                {
                    var p = column.Split(':');

                    if (p.Count() > 1)
                    {
                        string prefix = ((p[0].StartsWith("~")) ? " " + p[0].Replace("~", "") : p[0]);

                        string field = ((p[1].StartsWith("~")) ? " " + getString(rs[p[1].Replace("~", "")].ToString()) : getString(rs[p[1]].ToString()));

                        naration.Append(((field.Trim().Equals(string.Empty)) ? "" : prefix + field));
                    }
                    else
                    {
                        string field = ((p[0].StartsWith("~")) ? " " + getString(rs[p[0].Replace("~", "")].ToString()) : getString(rs[p[0]].ToString()));

                        naration.Append(field);
                    }

                }

            }

            catch
            {

            }

            return naration;

        }

        public static ExctractionDetails ExtractData(AccountDetails details, string CBAConstr, string ClirecConstr, string CBAType, bool isNIP, string NIPsession)
        {
            string uuid = Guid.NewGuid().ToString();
            DataTable data = new DataTable();
            var Edetails = new ExctractionDetails();
    
            Edetails.Data = new DataTable();
            try
            {
                var db = new DBConnector(CBAConstr, ClirecConstr);

                Utils.Log("Beginging Data Etraction for (" + details.AccountCode + ") using Interface definition ID (" + details.DefinitionID + ")");

                InterfaceDefinition def = db.getDefinition(details.DefinitionID);

                string mappedAccts = ""; var map = new StringBuilder(); int ct = 1;

                foreach (string cba in details.CBAccount)
                {
                    map.Append("'").Append(cba).Append("'");

                    if (ct < details.CBAccount.Count) { map.Append(","); }
                    ct++;
                }
                mappedAccts = map.ToString();

                //string sql = def.script.Replace("startdate", getDate(details.LastLedgerDate)).
                //    Replace("enddate", details.Endate).Replace("acctid", mappedAccts).
                //    Replace("acctCcode", details.currency).Replace("acctBcode", details.CbaBranchCode);

                Utils.Log("Connecting to transaction Database");


                var sql = "";

                string Startdate, Enddate;

                if (isNIP)
                {
                    switch (NIPsession.toInt())
                    {

                        case 1:
                            Startdate = details.LastLedgerDate.ToString("dd-MMM-yy") + " 2.00.00 PM";
                            Enddate = details.Endate.toDateTime().ToString("dd-MMM-yy") + " 11.59.59 PM";

                            break;

                        default:
                            Startdate = details.LastLedgerDate.ToString("dd-MMM-yy") + " 12.00.00 AM";
                            Enddate = details.Endate.toDateTime().ToString("dd-MMM-yy") + " 1.59.59 PM";

                            break;
                    }

                }
                else
                {
                    Startdate = getDate(details.LastLedgerDate);
                    Enddate = details.Endate.toDateTime().ToString("dd-MMM-yyyy");
                }


                switch (CBAType.CleanUp())
                {
                    case "mysql":
                        sql = def.script.Replace("startdate", details.LastLedgerDate.ToString("yyyy-MM-dd")).
                        Replace("enddate", Convert.ToDateTime(details.Endate).ToString("yyyy-MM-dd")).Replace("acctid", mappedAccts).
                        Replace("acctCcode", details.currency).Replace("acctBcode", details.CbaBranchCode);
                        Utils.Log(sql);
                        data = db.fetchMySQLCBA(sql);
                        break;

                    case "sqlserver":
                        sql = def.script.Replace("startdate", Startdate).
                  Replace("enddate", Enddate).Replace("acctid", mappedAccts).
                  Replace("acctCcode", details.currency).Replace("acctBcode", details.CbaBranchCode);
                        Utils.Log(sql);
                        data = db.getSQLServerCBA(sql);
                        break;

                    default:
                        sql = def.script.Replace("startdate", Startdate).
                   Replace("enddate", Enddate).Replace("acctid", mappedAccts).
                   Replace("acctCcode", details.currency).Replace("acctBcode", details.CbaBranchCode);
                        Utils.Log(sql);
                        data = db.fetchCBA(sql);
                        break;
                }


                int nofT = data.Rows.Count;
                bool isData = nofT > 0;
                Utils.Log(nofT + " transaction(s) fetched");
                int sn = 1;

                data = Utils.AddExtractionColumns(data);

                foreach (DataRow row in data.Rows)
                {


                    row["PostDate"] = row[def.postDateCol].toDateTime();

                    row["Valdate"] = row[def.valDateCol].toDateTime();

                    bool isDebit = row[def.directionCol].ToString().ToUpper().Equals("D");

                    row["CrDr"] = (isDebit ? "1" : "2");

                    row["Debits"] = (isDebit ? row[def.amountCol].toDecimal() : decimal.Zero);

                    row["Credits"] = (isDebit ? decimal.Zero : row[def.amountCol].toDecimal());

                    row["Amount"] = row["Credits"].toDecimal() + row["Debits"].toDecimal();

                    getUDFs(row, def);

                    row["Details"] = getNaration(def, row);

                    row["SN"] = sn++;
                    row["Id"] = uuid;
                    row["username"] = "LedgerDownload";

                }
                var balance = decimal.Zero;
                if (isData)
                {
                    try
                    {
                        Utils.Log("Computing Max Submited Date");
                        Edetails.LastSubmitedDate = data.Rows[0]["SubmittedOn"].ToString();
                        Utils.Log("Max Submitted Date =" + Edetails.LastSubmitedDate);
                       
                    }
                    catch( Exception e)
                    {
                        Utils.Log("Failed to obtain Max submitted date because:"+e.Message);
                    }




                    if (isNIP && NIPsession.toInt() == 2)
                    {

                    }
                    else
                    {
                        Utils.Log("Attempting to Fetch Account Balance");

                        foreach (string cba in details.CBAccount)
                        {


                            DataTable baldata;

                            if (isNIP)
                            {
                                Enddate = Enddate.toDateTime().ToString("dd-MMM-yyyy");
                            }


                            switch (CBAType.CleanUp())
                            {
                                case "mysql":
                                    sql = def.balScript.Replace("startdate", Convert.ToDateTime(Utils.getFirstDayofMonth(details.CurrentDate)).ToString("yyyy-MM-dd"))
                               .Replace("enddate", Convert.ToDateTime(details.Endate).ToString("yyyy-MM-dd")).Replace("acctid", cba).
                                Replace("acctCcode", details.currency).Replace("acctBcode", details.CbaBranchCode);
                                    baldata = db.fetchMySQLCBA(sql);
                                    break;

                                case "sqlserver":
                                    sql = def.balScript.Replace("startdate", Utils.getFirstDayofMonth(details.CurrentDate))
                               .Replace("enddate", Enddate).Replace("acctid", cba).
                                Replace("acctCcode", details.currency).Replace("acctBcode", details.CbaBranchCode);
                                    baldata = db.getSQLServerCBA(sql);
                                    break;

                                default:
                                    sql = def.balScript.Replace("startdate", Utils.getFirstDayofMonth(details.CurrentDate))
                               .Replace("enddate", Enddate).Replace("acctid", cba).
                                Replace("acctCcode", details.currency).Replace("acctBcode", details.CbaBranchCode);
                                    baldata = db.fetchCBA(sql);
                                    break;
                            }

                    
        


                            if (baldata.Rows.Count > 0)
                            {
                                balance += baldata.Rows[0][def.balCol].toDecimal();
                            }
                        }
                        Utils.Log(sql);
                        Utils.Log("Balance downloaded successfully and = " + balance.ToString("0.00"));


                    }

                    string[] selected = new[] { "SN", "PostDate", "Valdate", "Details", "Debits", "Credits", "Amount", "CrDr", "ud1", "ud2", "ud3", "ud4", "ud5", "Id", "username" };



                    data = new DataView(data).ToTable(false, selected);


                    
                 
                   

                    Edetails.TotalCredits = Convert.ToDecimal(data.Compute("Sum(Credits)", ""));
                    Edetails.TotalDebits = Convert.ToDecimal(data.Compute("Sum(Debits)", ""));
                    Edetails.Latest = Convert.ToDateTime(data.Compute("Max(PostDate)", ""));
                    Edetails.CreditCount = data.Compute("count(CrDr)", "CrDr ='2' ").toInt();
                    Edetails.DebitCount = data.Compute("count(CrDr)", "CrDr ='1' ").toInt();
              
                    Edetails.DataID = uuid;

                    Edetails.Balance = balance;



                    db.CopyDataTableToDB(data, "ExtractionTemp");

            
                }


                return Edetails;
            }
            catch (Exception e)
            {
                throw new Exception("CBA Download failed for " + details.AccountName + "(" + details.AccountCode + ") because: " + e.Message);
            }
            finally
            {
                data.Clear();
                data.Dispose();
            }

        }

        public static ExctractionDetails ExtractAjustedEntries(AccountDetails details, string CBAConstr, string ClirecConstr, string CBAType, string submitedDateCol)
        {
            string uuid = Guid.NewGuid().ToString();
            DataTable data = new DataTable();
            var Edetails = new ExctractionDetails();

            Edetails.Data = new DataTable();
            try
            {
                var db = new DBConnector(CBAConstr, ClirecConstr);

                Utils.Log("Beginging Data Etraction for (" + details.AccountCode + ") using Interface definition ID (" + details.DefinitionID + ")");

                InterfaceDefinition def = db.getDefinition(details.DefinitionID);

                string mappedAccts = ""; var map = new StringBuilder(); int ct = 1;

                foreach (string cba in details.CBAccount)
                {
                    map.Append("'").Append(cba).Append("'");

                    if (ct < details.CBAccount.Count) { map.Append(","); }
                    ct++;
                }
                mappedAccts = map.ToString();


                Utils.Log("Connecting to transaction Database");


                var sql = "";


                sql = "select lastdate from AdjustmentPosition where AccountID='"+details.AccountCode+"'";

                var  dt = db.getDataSet(sql);



                string Startdate, Enddate, StartSubmitedDate, EndSubmited;

                Startdate = Utils.getFirstDayofMonth(details.CurrentDate);

                Enddate = Utils.getLastDateofMonth(details.CurrentDate);

                StartSubmitedDate =  dt.Rows.Count>0? dt.Rows[0]["lastdate"].ToString(): Enddate;

                EndSubmited = Utils.getLastDateofMonth(details.CurrentDate.AddMonths(1));
                           


                switch (CBAType.CleanUp())
                {
                    case "mysql":
                        sql = def.script.Replace("startdate", details.LastLedgerDate.ToString("yyyy-MM-dd")).
                        Replace("enddate", Convert.ToDateTime(details.Endate).ToString("yyyy-MM-dd")).Replace("acctid", mappedAccts).
                        Replace("acctCcode", details.currency).Replace("acctBcode", details.CbaBranchCode);
                        Utils.Log(sql);
                        data = db.fetchMySQLCBA(sql);
                        break;

                    case "sqlserver":
                        sql = def.script.Replace("startdate", Startdate).
                   Replace("enddate", Enddate).Replace("acctid", mappedAccts).
                   Replace("acctCcode", details.currency).Replace("acctBcode", details.CbaBranchCode).Replace("StartSubDate", StartSubmitedDate).Replace("EndSubDate", EndSubmited);
                        Utils.Log(sql);
                        data = db.getSQLServerCBA(sql);
                        break;


                    default:
                        sql = def.script.Replace("startdate", Startdate).
                   Replace("enddate", Enddate).Replace("acctid", mappedAccts).
                   Replace("acctCcode", details.currency).Replace("acctBcode", details.CbaBranchCode).Replace("StartSubDate",StartSubmitedDate).Replace("EndSubDate",EndSubmited);
                        Utils.Log(sql);
                        data = db.fetchCBA(sql);
                        break;
                }


                int nofT = data.Rows.Count;
                bool isData = nofT > 0;
                Utils.Log(nofT + " transaction(s) fetched");
                int sn = 1;

                data = Utils.AddExtractionColumns(data);

                foreach (DataRow row in data.Rows)
                {


                    row["PostDate"] = row[def.postDateCol].toDateTime();

                    row["Valdate"] = row[def.valDateCol].toDateTime();

                    bool isDebit = row[def.directionCol].ToString().ToUpper().Equals("D");

                    row["CrDr"] = (isDebit ? "1" : "2");

                    row["Debits"] = (isDebit ? row[def.amountCol].toDecimal() : decimal.Zero);

                    row["Credits"] = (isDebit ? decimal.Zero : row[def.amountCol].toDecimal());

                    row["Amount"] = row["Credits"].toDecimal() + row["Debits"].toDecimal();

                    getUDFs(row, def);

                    row["Details"] = getNaration(def, row);

                    row["SN"] = sn++;
                    row["Id"] = uuid;
                    row["username"] = "LedgerDownload";

                }
                var balance = decimal.Zero;
                if (isData)
                {

                    Utils.Log("Computing Max Submited Date");
                    Edetails.LastSubmitedDate = data.Rows[0]["SubmittedOn"].ToString();
                    Utils.Log("Max Submitted Date =" + Edetails.LastSubmitedDate);

                    string[] selected = new[] { "SN", "PostDate", "Valdate", "Details", "Debits", "Credits", "Amount", "CrDr", "ud1", "ud2", "ud3", "ud4", "ud5", "Id", "username" };



                    data = new DataView(data).ToTable(false, selected);


                    balance = details.Balance+ data.Compute("sum(Credits)", "").toDecimal() - data.Compute("sum(Debits)", "").toDecimal();

                    



                    Edetails.TotalCredits = Convert.ToDecimal(data.Compute("Sum(Credits)", ""));
                    Edetails.TotalDebits = Convert.ToDecimal(data.Compute("Sum(Debits)", ""));
                    Edetails.Latest = Convert.ToDateTime(data.Compute("Max(PostDate)", ""));
                    Edetails.CreditCount = data.Compute("count(CrDr)", "CrDr ='2' ").toInt();
                    Edetails.DebitCount = data.Compute("count(CrDr)", "CrDr ='1' ").toInt();

                    Edetails.DataID = uuid;

                    Edetails.Balance = balance;



                    db.CopyDataTableToDB(data, "ExtractionTemp");


                }


                return Edetails;
            }
            catch (Exception e)
            {
                throw new Exception("CBA Download failed for " + details.AccountName + "(" + details.AccountCode + ") because: " + e.Message);
            }
            finally
            {
                data.Clear();
                data.Dispose();
            }

        }

    }


}


public struct ExctractionDetails
    {
        public DataTable Data;

        public decimal Balance;

        public decimal TotalCredits;

        public decimal TotalDebits;

        public int CreditCount;

        public int DebitCount;

        public DateTime Latest;

        public string DataID;

        public string LastSubmitedDate;

}

