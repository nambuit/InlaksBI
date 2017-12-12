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
    public class DBConnector
    {
        private static string CBA_Credentials;
        private static string Clirec_Credentials;

        public DBConnector(string CBA, string clirecDB)
        {
            CBA_Credentials = CBA;
            Clirec_Credentials = clirecDB;
        }


        public object ExecuteScalar(string sql)
        {
            SqlConnection conn = new SqlConnection(Clirec_Credentials);
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


        public AccountStats getAccountStats(string Acctid, string Mperiod)
        {
            var stat = new AccountStats();
            SqlConnection conn = new SqlConnection(Clirec_Credentials);



            stat.TotalItemsLedger = stat.TotalItemsStmt = stat.LUnmatched = stat.SMatched =
            stat.STDebit = stat.LTCredit = stat.STCredit = stat.LTDebit = 0;
            try
            {

                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandTimeout = 0;
                cmd.Connection = conn;

                var currency = getDataSet("select currency_code from nlbanks where BANKCODE ='" + Acctid + "'").Rows[0]["currency_code"].ToString().Trim();

                var amount = decimal.Zero;

                stat.Currency = currency;

                cmd.CommandText = "select  count(Postdate)  from " + Mperiod + "LedgerDetails where Account_ID ='" + Acctid + "' and isnull(ltrim(MatchType),'')='' and Amount !=0";

                stat.LUnmatched = cmd.ExecuteScalar().toInt();

                cmd.CommandText = "select  count(Postdate)  from " + Mperiod + "StmtDetails where Account_ID ='" + Acctid + "' and isnull(ltrim(MatchType),'')='' and Amount !=0";

                stat.SUnmatched = cmd.ExecuteScalar().toInt();

                cmd.CommandText = "select  count(Postdate)  from " + Mperiod + "LedgerDetails where Account_ID ='" + Acctid + "' and MatchType is not null and ltrim(MatchType)!='' and Amount !=0 ";

                stat.LMatched = cmd.ExecuteScalar().toInt();

                cmd.CommandText = "select  count(Postdate)  from " + Mperiod + "StmtDetails where Account_ID ='" + Acctid + "' and MatchType is not null and ltrim(MatchType)!='' and Amount !=0";

                stat.SMatched = cmd.ExecuteScalar().toInt();

                stat.TotalItemsLedger = stat.LUnmatched + stat.LMatched;

                stat.TotalItemsStmt = stat.SUnmatched + stat.SMatched;

                cmd.CommandText = "select  count(Amount)  from " + Mperiod + "StmtDetails where Account_ID ='" + Acctid + "' and isnull(ltrim(MatchType),'')='' and Crdr = 2 and Amount !=0";

                stat.STCredit = cmd.ExecuteScalar().toInt();

                cmd.CommandText = "select  count(Amount)  from " + Mperiod + "LedgerDetails where Account_ID ='" + Acctid + "' and isnull(ltrim(MatchType),'')='' and Crdr = 2 and Amount !=0";

                stat.LTCredit = cmd.ExecuteScalar().toInt();

                cmd.CommandText = "select  count(Amount)  from " + Mperiod + "StmtDetails where Account_ID ='" + Acctid + "' and isnull(ltrim(MatchType),'')='' and Crdr = 1 and Amount !=0";

                stat.STDebit = cmd.ExecuteScalar().toInt();

                cmd.CommandText = "select  count(Amount)  from " + Mperiod + "LedgerDetails where Account_ID ='" + Acctid + "' and isnull(ltrim(MatchType),'')='' and Crdr = 1 and Amount !=0";

                stat.LTDebit = cmd.ExecuteScalar().toInt();   //ToString("#,##0.00")

                cmd.CommandText = "select  sum(Amount)  from " + Mperiod + "StmtDetails where Account_ID ='" + Acctid + "' and isnull(ltrim(MatchType),'')='' and Crdr = 2 and Amount !=0";

                amount = cmd.ExecuteScalar().toDecimal();

                stat.STCreditA = amount > decimal.Zero ? amount : decimal.Zero;

                cmd.CommandText = "select  sum(Amount)  from " + Mperiod + "LedgerDetails where Account_ID ='" + Acctid + "' and isnull(ltrim(MatchType),'')='' and Crdr = 2 and Amount !=0";

                amount = cmd.ExecuteScalar().toDecimal();

                stat.LTCreditA = amount > decimal.Zero ? amount : decimal.Zero;

                cmd.CommandText = "select  sum(Amount)  from " + Mperiod + "StmtDetails where Account_ID ='" + Acctid + "' and isnull(ltrim(MatchType),'')='' and Crdr = 1 and Amount !=0";

                amount = cmd.ExecuteScalar().toDecimal();

                stat.STDebitA = amount > decimal.Zero ? amount : decimal.Zero;

                cmd.CommandText = "select  sum(Amount)  from " + Mperiod + "LedgerDetails where Account_ID ='" + Acctid + "' and isnull(ltrim(MatchType),'')='' and Crdr = 1 and Amount !=0";

                amount = cmd.ExecuteScalar().toDecimal();

                stat.LTDebitA = amount > decimal.Zero ? amount : decimal.Zero;

            }
            catch (Exception d)
            {
                throw (d);
            }
            finally
            {
                conn.Close();

            }


            return stat;
        }


        public DataTable getSQLServerCBA(String sql)
        {
            try
            {
                SqlConnection conn = new SqlConnection(CBA_Credentials);
              //  Utils.Log(CBA_Credentials);

                SqlCommand cmd = new SqlCommand(sql, conn);
                
                cmd.CommandTimeout = 0;
               // cmd.CommandType = CommandType.Text;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                conn.Close();
                return dt;
            }
            catch
            {
                throw;
            }
        }

        public DataTable getDataSet(String sql)
        {
            try
            {
                SqlConnection conn = new SqlConnection(Clirec_Credentials);

               

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandTimeout = 0;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                conn.Close();
                return dt;
            }
            catch (Exception w)
            {
                if (w.Message.Contains("Invalid object name"))
                {
                    if (this.Execute(Scripts.CreateAlldetailsView) || this.Execute(Scripts.CreateExtractionTemp)
                        || this.Execute(Scripts.CreateDefinitionTable) ||
                        this.Execute(Scripts.CreateExtractionTemp.Replace("ExtractionTemp", "ExemptionTemp")))
                    {
                        return this.getDataSet(sql);
                    }
                }
                throw new Exception("Failed to Fetch Data because: " + w.Message);
            }


        }


        public DataTable fetchCBA(string sql)
        {
            DataTable dt = new DataTable();
          //  DbConnection con = null;

            try
            {
                OracleConnection conn = new OracleConnection(CBA_Credentials);
       
                OracleCommand cmd = new OracleCommand(sql,conn);
         
               // conn.Open();          
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 0;

                OracleDataAdapter da = new OracleDataAdapter(cmd);
                da.Fill(dt);
                    //dt = odb.ExecuteDataSet(cmd).Tables[0];
                conn.Close();
                conn.Dispose();
                return dt;
            }
            catch (Exception w)
            {
                throw new Exception("Failed to Fetch CBA Data because: " + w.Message, w);
            }


        }

        public DataTable fetchCBA(SqlCommand cmd)
        {
            DataTable dt = new DataTable();
            //  DbConnection con = null;

            try
            {


                // conn.Open();          
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 0;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                //dt = odb.ExecuteDataSet(cmd).Tables[0]
                return dt;
            }
            catch (Exception w)
            {
                throw new Exception("Failed to Fetch CBA Data because: " + w.Message, w);
            }


        }

        public DataTable fetchCBA(OracleCommand cmd)
        {
            DataTable dt = new DataTable();
            //  DbConnection con = null;

            try
            {
        

                // conn.Open();          
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 0;
              
                OracleDataAdapter da = new OracleDataAdapter(cmd);
                da.Fill(dt);
                da.Fill(dt);
                //dt = odb.ExecuteDataSet(cmd).Tables[0]
                return dt;
            }
            catch (Exception w)
            {
                throw new Exception("Failed to Fetch CBA Data because: " + w.Message, w);
            }


        }


        public DataTable fetchMySQLCBA(string sql)
        {
            DataTable dt = new DataTable();
            try
            {
                MySqlConnection conn = new MySqlConnection(CBA_Credentials);
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.CommandTimeout = 0;
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(dt);
                conn.Close();
                return dt;
            }
            catch (Exception w)
            {
                throw new Exception("Failed to Fetch CBA Data because: " + w.Message, w);
            }


        }


        public bool Execute(String sql)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = Clirec_Credentials;
            try
            {

                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandTimeout = 0;
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                conn.Close();
                return true;
            }
            catch(Exception f)
            {
               // if (w.Message.Contains("There is already an object named")) { conn.Close(); return false; }
                conn.Close();
               // Utils.Log("Failed to Execute an Sql Command because: " + w.Message);
                return false;
            }

        }


        public AcctSummary getAcctSummary(char table, String AccountID, String MonthlyPeriod, String period, String constr)
        {
            AcctSummary summary = new AcctSummary();
            String Table = "";
            switch (table)
            {
                case 'L':
                    Table = "Ledger";
                    break;
                case 'S':
                    Table = "Stmt";
                    break;
            }

            try
            {

                DataTable dt;

                dt = getDataSet("select sum(amount) as total from " + MonthlyPeriod + Table + "Details where Account_ID ='" + AccountID + "' and CrDr =1 and (MatchType='' or MatchType is  null)");
                string value = "";
                if (dt.Rows.Count > 0)
                {

                    value = getString(dt.Rows[0]["total"].ToString());

                    summary.TotalDebits = string.IsNullOrEmpty(value) ? decimal.Zero : (decimal)dt.Rows[0]["total"];
                }

                dt = getDataSet("select sum(amount) as total from " + MonthlyPeriod + Table + "Details where Account_ID ='" + AccountID + "' and CrDr ='2' and (MatchType='' or MatchType is  null)");
                if (dt.Rows.Count > 0)
                {
                    value = getString(dt.Rows[0]["total"].ToString());

                    summary.TotalCredits = string.IsNullOrEmpty(value) ? decimal.Zero : (decimal)dt.Rows[0]["total"];
                }



                dt = getDataSet("select * from " + Table + "Bal where Account_ID ='" + AccountID + "' and period ='" + period + "'");
                if (dt.Rows.Count > 0)
                {

                    value = getString(dt.Rows[0]["OpenBal"].ToString());

                    summary.Obalance = string.IsNullOrEmpty(value) ? decimal.Zero : (decimal)dt.Rows[0]["OpenBal"];

                    value = getString(dt.Rows[0]["CloseBal"].ToString());

                    summary.Cbalance = string.IsNullOrEmpty(value) ? decimal.Zero : (decimal)dt.Rows[0]["CloseBal"];

                }


                return summary;
            }
            catch (Exception e)
            {
                throw (e);

            }

        }


        public bool IsReversedStatement(string acctid)
        {
            try
            {
                var sql = "select ForceLoginOnDownload from BranchList  where BranchCode = (select BranchCode from nlbanks where bankcode ='" + acctid + "')";
                var a = getDataSet(sql);

                if (a.Rows.Count > 0)
                {
                    var value = a.Rows[0]["ForceLoginOnDownload"].ToString().isNull() ? false : a.Rows[0]["ForceLoginOnDownload"].toBoolean();

                    return value;

                }
                else
                {
                    return false;
                }
            }
            catch
            {

                return false;
            }
        }





        public bool LoadTemp(List<Transaction> txns, bool valid)
        {
            string Table = ((valid) ? "ExtractionTemp" : "ExemptionTemp");
            try
            {

                SqlConnection conn = new SqlConnection(Clirec_Credentials);
                conn.Open();
                foreach (var txn in txns) {
                    string sql = "insert into {0} values ('{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}')";
                    SqlCommand cmd = new SqlCommand(string.Format(sql, new object[]
                    {
                     Table,
                     txn.SN,
                     txn.Postdate,
                     txn.Valdate,
                     txn.Details,
                     txn.Debit,
                     txn.Credit,
                     txn.Credit+txn.Debit,
                     txn.CrDr,
                     txn.ud1,
                     txn.ud2,
                     txn.ud3,
                     txn.ud4,
                     txn.ud5,
                     txn.ID,
                     txn.user
                }


                        ), conn);


                    cmd.ExecuteNonQuery();
                }
                conn.Close();
                return true;

            }
            catch (Exception w)
            {

                throw new Exception("Failed to Load " + Table + " Table because: " + w.Message);
            }

        }

        public AccountDetails getAccountDetails(string acctid, bool restorepoint)
        {
            try
            {
                AccountDetails details = new AccountDetails();
                string sql = "select distinct BANKCODE, AccountName, Domain, Affiliate, CUR_DATE, LASTDATE, LASTSTMT, LOCAL_NO, CurrentUser, Account_Active, ACCTYPE, CA_ACCTNO, FOREIGN_NO, currency, SHORTNAME,ledger,statement  from alldetails where BANKCODE ='{0}'";
                var data = this.getDataSet(string.Format(sql, new object[] { acctid }));
                List<string> cba = new List<string>();
                List<string> stmtcba = new List<string>();
                if (data.Rows.Count > 0)
                {
                    var row = data.Rows[0];
                    details.CurrentDate = (DateTime)row["CUR_DATE"];
                    details.LastLedgerDate = (DateTime)row["LASTDATE"];
                    details.LastStmtDate = (DateTime)row["LASTSTMT"];
                    details.AccountName = row["AccountName"].ToString();
                    details.DomainName = row["Domain"].ToString();
                    details.AffiliateName = row["Affiliate"].ToString();
                    details.DefinitionID = row["LOCAL_NO"].ToString();
                    details.CurrentUser = row["CurrentUser"].ToString();
                    details.CA_ACCTNO = row["CA_ACCTNO"].ToString();
                    details.FOREIGN_NO = row["FOREIGN_NO"].ToString();
                    details.currency = row["currency"].ToString();
                    details.SHORTNAME = row["SHORTNAME"].ToString();
                    details.InternalRecord = row["ledger"].ToString();
                    details.ExternalRecod = row["statement"].ToString();
                    details.Acct_Active = row["Account_Active"].ToString();
                    details.AccountCode = acctid;
                    var actType = string.IsNullOrEmpty(row["ACCTYPE"].ToString()) ? "normal" : row["ACCTYPE"].ToString();
                    details.isSuspense = actType.Trim().ToLower().Equals("s");

                    sql = "select FBN_Account from Account_Maintenance where Clirec_Account='{0}' and side ='L'";
                    var mappings = this.getDataSet(string.Format(sql, new object[] { acctid }));
                    foreach (DataRow map in mappings.Rows)
                    {
                        cba.Add(map["FBN_Account"].ToString());
                    }

                    details.CBAccount = ((cba.Count > 0) ? cba : null);


                    sql = "select FBN_Account from Account_Maintenance where Clirec_Account='{0}' and side ='S'";
                    mappings = this.getDataSet(string.Format(sql, new object[] { acctid }));
                    foreach (DataRow map in mappings.Rows)
                    {
                        stmtcba.Add(map["FBN_Account"].ToString());
                    }

                    details.StmtCBAccount = ((stmtcba.Count > 0) ? stmtcba : null);

                    string period = details.CurrentDate.ToString("yyyyMM");

                    var baldata = this.getDataSet(string.Format(Scripts.Balances, new object[] { acctid, period }));
                    details.Balance = ((baldata.Rows.Count != 0) ? (decimal)baldata.Rows[0]["ledgerbal"] : decimal.Zero);
                    details.stmtBalmce = ((baldata.Rows.Count != 0) ? (decimal)baldata.Rows[0]["stmtbal"] : decimal.Zero);

                }

                try
                {
                    sql = "select Ledger_Creation_Balance,Statement_Creation_Balance from nlbanks where BANKCODE ='" + acctid + "'";

                    var dt = getDataSet(sql);
                    if (dt.Rows.Count > 0)
                    {
                        details.Ledger_Creation_Balance = string.IsNullOrEmpty(dt.Rows[0]["Ledger_Creation_Balance"].ToString()) ? decimal.Zero : Convert.ToDecimal(dt.Rows[0]["Ledger_Creation_Balance"].ToString());
                        details.Statement_Creation_Balance = string.IsNullOrEmpty(dt.Rows[0]["Statement_Creation_Balance"].ToString()) ? decimal.Zero : Convert.ToDecimal(dt.Rows[0]["Statement_Creation_Balance"].ToString());
                    }
                }
                catch
                {

                }
                if (restorepoint)
                {
                    string ltable = details.CurrentDate.ToString("MMMyyyy") + "LedgerDetails";
                    string stable = details.CurrentDate.ToString("MMMyyyy") + "StmtDetails";

                    sql = "select TOP 1 tran_id as Lmark from {0} where account_id ='{1}' order by Tran_ID desc";

                    var position = this.getDataSet(string.Format(sql, new object[] { ltable, acctid }));

                    details.LedgerLastID = ((position.Rows.Count > 0) ? getString(position.Rows[0]["Lmark"].ToString()) : "0");

                    sql = "select TOP 1 tran_id as Smark from {0} where account_id ='{1}' order by Tran_ID desc";

                    position = this.getDataSet(string.Format(sql, new object[] { stable, acctid }));

                    details.StmtLastID = ((position.Rows.Count > 0) ? getString(position.Rows[0]["Smark"].ToString()) : "0");
                }
                return details;

            }
            catch (Exception w)
            {
                throw new Exception("Failed to Retrieve AccountDetails for " + acctid + " because: " + w.Message, w);
            }

        }

        public AccountDetails getAccountDetails(string acctid)
        {
            try
            {
                AccountDetails details = new AccountDetails();
                string sql = "select distinct BANKCODE, AccountName, Domain, Affiliate, CUR_DATE, LASTDATE, LASTSTMT, LOCAL_NO, CurrentUser, Account_Active, ACCTYPE, CA_ACCTNO, FOREIGN_NO, currency, SHORTNAME,ledger,statement  from alldetails where BANKCODE ='{0}'";
                var data = this.getDataSet(string.Format(sql, new object[] { acctid }));
                List<string> cba = new List<string>();
                List<string> stmtcba = new List<string>();
                if (data.Rows.Count > 0)
                {
                    var row = data.Rows[0];
                    details.CurrentDate = (DateTime)row["CUR_DATE"];
                    details.LastLedgerDate = (DateTime)row["LASTDATE"];
                    details.LastStmtDate = (DateTime)row["LASTSTMT"];
                    details.AccountName = row["AccountName"].ToString();
                    details.DomainName = row["Domain"].ToString();
                    details.AffiliateName = row["Affiliate"].ToString();
                    details.DefinitionID = row["LOCAL_NO"].ToString();
                    details.CurrentUser = row["CurrentUser"].ToString();
                    details.CA_ACCTNO = row["CA_ACCTNO"].ToString();
                    details.FOREIGN_NO = row["FOREIGN_NO"].ToString();
                    details.currency = row["currency"].ToString();
                    details.SHORTNAME = row["SHORTNAME"].ToString();
                    details.InternalRecord = row["ledger"].ToString();
                    details.ExternalRecod = row["statement"].ToString();
                    details.Acct_Active = row["Account_Active"].ToString();
                    details.AccountCode = acctid;
                    var actType = string.IsNullOrEmpty(row["ACCTYPE"].ToString()) ? "normal" : row["ACCTYPE"].ToString();
                    details.isSuspense = actType.Trim().ToLower().Equals("s");

                    sql = "select FBN_Account from Account_Maintenance where Clirec_Account='{0}' and side ='L'";
                    var mappings = this.getDataSet(string.Format(sql, new object[] { acctid }));
                    foreach (DataRow map in mappings.Rows) {
                        cba.Add(map["FBN_Account"].ToString());
                    }

                    details.CBAccount = ((cba.Count > 0) ? cba : null);


                    sql = "select FBN_Account from Account_Maintenance where Clirec_Account='{0}' and side ='S'";
                    mappings = this.getDataSet(string.Format(sql, new object[] { acctid }));
                    foreach (DataRow map in mappings.Rows)
                    {
                        stmtcba.Add(map["FBN_Account"].ToString());
                    }

                    details.StmtCBAccount = ((stmtcba.Count > 0) ? stmtcba : null);

                    string period = details.CurrentDate.ToString("yyyyMM");

                    var baldata = this.getDataSet(string.Format(Scripts.Balances, new object[] { acctid, period }));
                    details.Balance = ((baldata.Rows.Count != 0) ? (decimal)baldata.Rows[0]["ledgerbal"] : decimal.Zero);
                    details.stmtBalmce = ((baldata.Rows.Count != 0) ? (decimal)baldata.Rows[0]["stmtbal"] : decimal.Zero);

                }

                try
                {
                    sql = "select Ledger_Creation_Balance,Statement_Creation_Balance from nlbanks where BANKCODE ='" + acctid + "'";

                    var dt = getDataSet(sql);
                    if (dt.Rows.Count > 0)
                    {
                        details.Ledger_Creation_Balance = string.IsNullOrEmpty(dt.Rows[0]["Ledger_Creation_Balance"].ToString()) ? decimal.Zero : Convert.ToDecimal(dt.Rows[0]["Ledger_Creation_Balance"].ToString());
                        details.Statement_Creation_Balance = string.IsNullOrEmpty(dt.Rows[0]["Statement_Creation_Balance"].ToString()) ? decimal.Zero : Convert.ToDecimal(dt.Rows[0]["Statement_Creation_Balance"].ToString());
                    }
                }
                catch
                {

                }
                string ltable = details.CurrentDate.ToString("MMMyyyy") + "LedgerDetails";
                string stable = details.CurrentDate.ToString("MMMyyyy") + "StmtDetails";

                sql = "select TOP 1 tran_id as Lmark from {0} where account_id ='{1}' order by Tran_ID desc";

                var position = this.getDataSet(string.Format(sql, new object[] { ltable, acctid }));

                details.LedgerLastID = ((position.Rows.Count > 0) ? getString(position.Rows[0]["Lmark"].ToString()) : "0");

                sql = "select TOP 1 tran_id as Smark from {0} where account_id ='{1}' order by Tran_ID desc";

                position = this.getDataSet(string.Format(sql, new object[] { stable, acctid }));

                details.StmtLastID = ((position.Rows.Count > 0) ? getString(position.Rows[0]["Smark"].ToString()) : "0");

                return details;

            }
            catch (Exception w)
            {
                throw new Exception("Failed to Retrieve AccountDetails for " + acctid + " because: " + w.Message, w);
            }

        }

        public InterfaceDefinition getDefinition(string defID)
        {
            try {
                InterfaceDefinition def = new InterfaceDefinition();
                string sql = "select * from InterfaceDefinition where ID ='{0}'";
                var data = this.getDataSet(string.Format(sql, new object[] { defID }));
                if (data.Rows.Count == 0) { throw new Exception("No Extraction Definition found for ID '" + defID + "'"); }
                var defRow = data.Rows[0];

                def.id = defID;
                def.postDateCol = defRow["PostDate_Col"].ToString();
                def.valDateCol = defRow["ValDate_Col"].ToString();
                def.naration = defRow["Naration"].ToString();
                def.narationType = defRow["Naration_Type"].ToString();
                def.script = defRow["script"].ToString();
                def.balScript = defRow["bal_script"].ToString();
                def.amountCol = defRow["Amount_Col"].ToString();
                def.directionCol = defRow["direction_Col"].ToString();
                def.ud1Col = defRow["ud1_Col"].ToString();
                def.ud2Col = defRow["ud2_Col"].ToString();
                def.ud3Col = defRow["ud3_Col"].ToString();
                def.ud4Col = defRow["ud4_Col"].ToString();
                def.ud5Col = defRow["ud5_Col"].ToString();
                def.balCol = defRow["bal_Col"].ToString();

                return def;
            }
            catch (Exception e) { throw new Exception("Failed to fetch Interface Definition because : " + e.Message); }
        }






        public void LoadMonthlyTable(string dataID, string closebal, AccountDetails details, string user)
        {
            string Table = details.CurrentDate.ToString("MMMyyyy") + "LedgerDetails";

            var data = this.getDataSet(string.Format("select TOP 1 postdate as maxdate from ExtractionTemp where ID ='{0}' order by postdate desc", new object[] { dataID }));

            var maxdate = ((data.Rows[0]["maxdate"] == null) ? details.LastLedgerDate : data.Rows[0]["maxdate"].toDateTime());

            Utils.Log("Logging Account Restore Data");



            var tobject = getManagedSQLTrasaction(Clirec_Credentials);

            var cmd = tobject.currentCommand;

            cmd.CommandTimeout = 0;


            try
            {
                cmd.CommandText = string.Format(Scripts.RestorePoint, new object[] {

            details.AccountCode,
            details.Balance,
            details.stmtBalmce,
            details.LedgerLastID,
            details.StmtLastID,
            DateTime.Now.getValidFormat(),
            user,
            user+" of data between "+details.LastLedgerDate.ToString("dd-MMM-yyyy")+" and "+details.Endate,
            "1",
            details.LastLedgerDate.getValidFormat(),
            "L"
            });


                cmd.ExecuteNonQuery();

                if (!details.isSuspense)
                {

                    cmd.CommandText = string.Format(Scripts.updateMonthlyTable, new object[] {

                Table,
                details.AccountCode,
                dataID
                });

                    cmd.ExecuteNonQuery();

                }
                else
                {



                    var ltable = details.CurrentDate.ToString("MMMyyyy") + "LedgerDetails";

                    var stable = details.CurrentDate.ToString("MMMyyyy") + "StmtDetails";

                    cmd.CommandText = string.Format(Scripts.updateMonthlyTable + " and Crdr =2", new object[] {

                stable,
                details.AccountCode,
                dataID
                });

                    cmd.ExecuteNonQuery();


                    cmd.CommandText = string.Format(Scripts.updateMonthlyTable + " and Crdr =1", new object[] {

                ltable,
                details.AccountCode,
                dataID
                });

                    cmd.ExecuteNonQuery();




                }


                cmd.CommandText = string.Format("update NLbanks set LASTDATE ='{0}' where BANKCODE ='{1}'", new object[] {

                maxdate.ToString("dd-MMM-yyyy"),
                details.AccountCode
                });


                cmd.ExecuteNonQuery();

                cmd.CommandText = string.Format("update ledgerbal set CloseBal ='{0}' where Account_ID ='{1}' and Period ='{2}'", new object[] {
                 closebal,
                 details.AccountCode,
                 details.CurrentDate.ToString("yyyyMM")
                });

                cmd.ExecuteNonQuery();
                tobject.currentTransaction.Commit();

                Utils.Log("Transactions Successfully Loaded into Monthly Table");

             }
            catch (Exception w)
            {
                try
                {
                    tobject.currentTransaction.Rollback();
                }
                catch
                {

                }
                Utils.Log("Failed to Update Monthly Table beacuse: " + w.Message);

                throw new Exception("Failed to Update Monthly Table beacuse: " + w.Message);


            }
            finally
            {
                this.Execute(string.Format("delete from ExtractionTemp where Id='{0}'", new object[] { dataID }));
            }

        }


        public void LoadStmtTable(string dataID, string closebal, AccountDetails details, string user)
        {
            string Table = details.CurrentDate.ToString("MMMyyyy") + "StmtDetails";

            var data = this.getDataSet(string.Format("select TOP 1 postdate as maxdate from ExtractionTemp where ID ='{0}' order by postdate desc", new object[] { dataID }));

            var maxdate = ((data.Rows[0]["maxdate"] == null) ? details.LastStmtDate : data.Rows[0]["maxdate"].toDateTime());

            Utils.Log("Logging Account Restore Data");


            var tobject = getManagedSQLTrasaction(Clirec_Credentials);

            var cmd = tobject.currentCommand;
            cmd.CommandTimeout = 0;

            try
            {
                cmd.CommandText = string.Format(Scripts.RestorePoint, new object[] {

            details.AccountCode,
            details.Balance,
            details.stmtBalmce,
            details.LedgerLastID,
            details.StmtLastID,
            DateTime.Now.ToString("MM/dd/yyyy h:mm:ss"),
            user,
            user+" of data between "+details.LastStmtDate.ToString("dd-MMM-yyyy")+" and "+maxdate.ToString("dd-MMM-yyyy"),
            "1",
            details.LastStmtDate,
            "S"
            });


                cmd.ExecuteNonQuery();



                cmd.CommandText = string.Format(Scripts.updateMonthlyTable, new object[] {

                Table,
                details.AccountCode,
                dataID
                });

                cmd.ExecuteNonQuery();


                cmd.CommandText = string.Format("update NLbanks set LASTSTMT ='{0}' where BANKCODE ='{1}'", new object[] {

                maxdate.ToString("dd-MMM-yyyy"),
                details.AccountCode
                });


                cmd.ExecuteNonQuery();

                cmd.CommandText = string.Format("update stmtbal set CloseBal ='{0}' where Account_ID ='{1}' and Period ='{2}'", new object[] {
                 closebal,
                 details.AccountCode,
                 details.CurrentDate.ToString("yyyyMM")
                });

                cmd.ExecuteNonQuery();
                tobject.currentTransaction.Commit();

                Utils.Log("Transactions Successfully Loaded into Monthly Table");

               

            }
            catch (Exception w)
            {
                try
                {
                    tobject.currentTransaction.Rollback();
                }
                catch
                {

                }
                Utils.Log("Failed to Update Monthly Table beacuse: " + w.Message);

                throw new Exception("Failed to Update Monthly Table beacuse: " + w.Message);
               

            }
            finally
            {
                this.Execute(string.Format("delete from ExtractionTemp where Id='{0}'", new object[] { dataID }));
            }

        }
        
        





        public AcctSummary getAcctSummary(char table, string AccountID, string MonthlyPeriod, string period)
        {
            AcctSummary summary = new AcctSummary();
            string Table = "";
            switch (table)
            {
                case 'L':
                    Table = "Ledger";
                    break;
                case 'S':
                    Table = "Stmt";
                    break;
            }

            try
            {

                DataTable dt;

                dt = getDataSet("select sum(amount) as total from " + MonthlyPeriod + Table + "Details where Account_ID ='" + AccountID + "' and CrDr ='1' and (MatchType='' or MatchType is  null)");
                string value = "";
                if (dt.Rows.Count > 0)
                {

                    value = getString(dt.Rows[0]["total"].ToString());

                    summary.TotalDebits = string.IsNullOrEmpty(value) ? decimal.Zero : (decimal)dt.Rows[0]["total"];
                }

                dt = getDataSet("select sum(amount) as total from " + MonthlyPeriod + Table + "Details where Account_ID ='" + AccountID + "' and CrDr ='2' and (MatchType='' or MatchType is  null)");
                if (dt.Rows.Count > 0)
                {
                    value = getString(dt.Rows[0]["total"].ToString());

                    summary.TotalCredits = string.IsNullOrEmpty(value) ? decimal.Zero : (decimal)dt.Rows[0]["total"];
                }



                dt = getDataSet("select * from " + Table + "Bal where Account_ID ='" + AccountID + "' and period ='" + period + "'");
                if (dt.Rows.Count > 0)
                {

                    value = getString(dt.Rows[0]["OpenBal"].ToString());

                    summary.Obalance = string.IsNullOrEmpty(value) ? decimal.Zero : (decimal)dt.Rows[0]["OpenBal"];

                    value = getString(dt.Rows[0]["OpenBal"].ToString());

                    summary.Cbalance = string.IsNullOrEmpty(value) ? decimal.Zero : (decimal)dt.Rows[0]["CloseBal"];

                }
                return summary;
            }
            catch (Exception e)
            {
                throw (e);

            }

        }






        public ManagedSqlTransaction getManagedSQLTrasaction(string constr)
        {

            try
            {
                var tobject = new ManagedSqlTransaction();
                tobject.currentConnection = new SqlConnection(constr);
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

        //public ManagedOracleTransaction getManagedOracleTrasaction(string constr)
        //{

        //    try
        //    {
        //        var tobject = new ManagedOracleTransaction();
        //        tobject.currentConnection = new OracleConnection(constr);
        //        tobject.currentConnection.Open();
        //        tobject.currentCommand = tobject.currentConnection.CreateCommand();
        //        tobject.currentTransaction = tobject.currentConnection.BeginTransaction();
        //        tobject.currentCommand.Connection = tobject.currentConnection;
        //        tobject.currentCommand.Transaction = tobject.currentTransaction;

        //        return tobject;
        //    }
        //    catch (Exception s)
        //    {
        //        throw (s);
        //    }
        //}



        static String getString(string value)
        {
            try
            {
                return ((value == null || value == "null") ? "" : value);
            }
            catch
            {
                return "";
            }
        }


        public DataTable getMatchedItems(string monthlyperiod, string acctid)
        {
            var AllMatchedItems = new List<MatchedItems>();

            string ltable = monthlyperiod + "LedgerDetails";

            string stable = monthlyperiod + "StmtDetails";

            string sql = "select distinct matchid from (select matchid from " + ltable + " where ltrim(MatchType) !=''" +
                         "and MatchType is not null and Account_ID = '" + acctid + "' union all select matchid from " + stable + " where " +
                         "ltrim(MatchType) != '' and MatchType is not null and Account_ID = '" + acctid + "') matchids ";

            var data = getDataSet(sql);

            if (data.Rows.Count > 0)
            {

                foreach (DataRow row in data.Rows)
                {

                    var matchid = row["matchid"].ToString();


                    var lt = getDataSet("select * from " + ltable + " where matchid ='" + matchid + "' and Account_ID='" + acctid + "' order by Amount desc");

                    var st = getDataSet("select * from " + stable + " where matchid ='" + matchid + "' and Account_ID='" + acctid + "' order by Amount desc");

                    var Mgroup = new List<MatchedItems>();


                    if (lt.Rows.Count > 0 && st.Rows.Count > 0)
                    {
                        if (lt.Rows.Count > st.Rows.Count)
                        {
                            foreach (DataRow l in lt.Rows)
                            {
                                var item = new MatchedItems();

                                item.LPostdate = string.IsNullOrEmpty(getString(l["Postdate"].ToString())) ? "" : Convert.ToDateTime(l["Postdate"]).ToString("dd-MMM-yyyy");

                                item.LValdate = string.IsNullOrEmpty(getString(l["Valdate"].ToString())) ? "" : Convert.ToDateTime(l["Valdate"]).ToString("dd-MMM-yyyy");

                                item.LDetails = l["Details"].ToString();

                                item.LAmount = Convert.ToDecimal(l["Amount"]) > 0 ? Convert.ToDecimal(l["Amount"]).ToString("#,###.00") : "";

                                item.LCrDr = Convert.ToInt32(l["CrDr"]) > 0 ? l["CrDr"].ToString() : "";

                                item.LAmount = item.LCrDr == "1" ? "-" + item.LAmount : item.LAmount;

                                item.userID = l["userid"].ToString();

                                Mgroup.Add(item);

                            }

                            for (int i = 0; i < st.Rows.Count; i++)
                            {
                                Mgroup[i].SAmount = Convert.ToDecimal(st.Rows[i]["Amount"]) > 0 ? Convert.ToDecimal(st.Rows[i]["Amount"]).ToString("#,###.00") : "";

                                Mgroup[i].SPostdate = string.IsNullOrEmpty(getString(st.Rows[i]["Postdate"].ToString())) ? "" : Convert.ToDateTime(st.Rows[i]["Postdate"]).ToString("dd-MMM-yyyy");

                                Mgroup[i].SValdate = string.IsNullOrEmpty(getString(st.Rows[i]["Valdate"].ToString())) ? "" : Convert.ToDateTime(st.Rows[i]["Valdate"]).ToString("dd-MMM-yyyy");

                                Mgroup[i].SDetails = st.Rows[i]["Details"].ToString();

                                Mgroup[i].SCrDr = Convert.ToInt32(st.Rows[i]["CrDr"]) > 0 ? st.Rows[i]["CrDr"].ToString() : "";

                                Mgroup[i].SAmount = Mgroup[i].SCrDr == "1" ? "-" + Mgroup[i].SAmount : Mgroup[i].SAmount;

                            }



                        }
                        else
                        {
                            foreach (DataRow s in st.Rows)
                            {
                                var item = new MatchedItems();

                                item.SPostdate = string.IsNullOrEmpty(getString(s["Postdate"].ToString())) ? "" : Convert.ToDateTime(s["Postdate"]).ToString("dd-MMM-yyyy");

                                item.SValdate = string.IsNullOrEmpty(getString(s["Valdate"].ToString())) ? "" : Convert.ToDateTime(s["Valdate"]).ToString("dd-MMM-yyyy");

                                item.SDetails = s["Details"].ToString();

                                item.SAmount = Convert.ToDecimal(s["Amount"]) > 0 ? Convert.ToDecimal(s["Amount"]).ToString("#,###.00") : "";

                                item.SCrDr = Convert.ToInt32(s["CrDr"]) > 0 ? s["CrDr"].ToString() : "";

                                item.SAmount = item.SCrDr == "1" ? "-" + item.SAmount : item.SAmount;

                                item.userID = s["userid"].ToString();


                                Mgroup.Add(item);

                            }

                            for (int i = 0; i < lt.Rows.Count; i++)
                            {
                                Mgroup[i].LAmount = Convert.ToDecimal(lt.Rows[i]["Amount"]) > 0 ? Convert.ToDecimal(lt.Rows[i]["Amount"]).ToString("#,###.00") : "";

                                Mgroup[i].LPostdate = string.IsNullOrEmpty(getString(lt.Rows[i]["Postdate"].ToString())) ? "" : Convert.ToDateTime(lt.Rows[i]["Postdate"]).ToString("dd-MMM-yyyy");

                                Mgroup[i].LValdate = string.IsNullOrEmpty(getString(lt.Rows[i]["Valdate"].ToString())) ? "" : Convert.ToDateTime(lt.Rows[i]["Valdate"]).ToString("dd-MMM-yyyy");

                                Mgroup[i].LDetails = lt.Rows[i]["Details"].ToString();

                                Mgroup[i].LCrDr = Convert.ToInt32(lt.Rows[i]["CrDr"]) > 0 ? lt.Rows[i]["CrDr"].ToString() : "";

                                Mgroup[i].LAmount = Mgroup[i].LCrDr.Equals("1") ? "-" + Mgroup[i].LAmount : Mgroup[i].LAmount;


                            }



                        }

                    }

                    if (lt.Rows.Count > 0 && st.Rows.Count == 0)
                    {
                        foreach (DataRow l in lt.Rows)
                        {
                            var item = new MatchedItems();

                            item.LPostdate = string.IsNullOrEmpty(getString(l["Postdate"].ToString())) ? "" : Convert.ToDateTime(l["Postdate"]).ToString("dd-MMM-yyyy");

                            item.LValdate = string.IsNullOrEmpty(getString(l["Valdate"].ToString())) ? "" : Convert.ToDateTime(l["Valdate"]).ToString("dd-MMM-yyyy");

                            item.LDetails = l["Details"].ToString();

                            item.LAmount = Convert.ToDecimal(l["Amount"]) > 0 ? Convert.ToDecimal(l["Amount"]).ToString("#,###.00") : "";

                            item.LCrDr = Convert.ToInt32(l["CrDr"]) > 0 ? l["CrDr"].ToString() : "";

                            item.LAmount = item.LCrDr == "1" ? "-" + item.LAmount : item.LAmount;

                            item.userID = l["userid"].ToString();



                            Mgroup.Add(item);

                        }

                    }


                    if (lt.Rows.Count == 0 && st.Rows.Count > 0)
                    {
                        foreach (DataRow s in st.Rows)
                        {
                            var item = new MatchedItems();

                            item.SPostdate = string.IsNullOrEmpty(getString(s["Postdate"].ToString())) ? "" : Convert.ToDateTime(s["Postdate"]).ToString("dd-MMM-yyyy");

                            item.SValdate = string.IsNullOrEmpty(getString(s["Valdate"].ToString())) ? "" : Convert.ToDateTime(s["Valdate"]).ToString("dd-MMM-yyyy");

                            item.SDetails = s["Details"].ToString();

                            item.SAmount = Convert.ToDecimal(s["Amount"]) > 0 ? Convert.ToDecimal(s["Amount"]).ToString("#,###.00") : "";

                            item.SCrDr = Convert.ToInt32(s["CrDr"]) > 0 ? s["CrDr"].ToString() : "";

                            item.SAmount = item.SCrDr == "1" ? "-" + item.SAmount : item.SAmount;

                            item.userID = s["userid"].ToString();

                            Mgroup.Add(item);

                        }

                    }

                    AllMatchedItems.AddRange(Mgroup);

                }

                DataTable dt = new DataTable();
                DataColumn column1 = new DataColumn("LPostDate");
                dt.Columns.Add(column1);
                DataColumn column2 = new DataColumn("LValdate");
                dt.Columns.Add(column2);
                DataColumn column3 = new DataColumn("LDetails");
                dt.Columns.Add(column3);
                DataColumn column4 = new DataColumn("LAmount");
                dt.Columns.Add(column4);
                DataColumn column5 = new DataColumn("SPostDate");
                dt.Columns.Add(column5);
                DataColumn column6 = new DataColumn("SValdate");
                dt.Columns.Add(column6);
                DataColumn column7 = new DataColumn("SDetails");
                dt.Columns.Add(column7);
                DataColumn column8 = new DataColumn("SAmount");
                dt.Columns.Add(column8);
                DataColumn column9 = new DataColumn("UserID");
                dt.Columns.Add(column9);

                foreach (var item in AllMatchedItems)
                {

                    dt.Rows.Add(new object[] {item.LPostdate,item.LValdate,item.LDetails,item.LAmount,
                    item.SPostdate,item.SValdate,item.SDetails,item.SAmount,item.userID});
                }



               


                return dt;

            }
            else
            {
                throw new Exception("No Matched Items Found");
            }

        }

        public List<ValuePair> getValuePair(string id, string value, string sql)
        {


            var pair = new List<ValuePair>();
            try
            {



                var data = getDataSet(sql);


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



        public DataTable parseCSV(string path)
        {
            string[] columns =
               {
                    "A", "B", "C", "D", "E", "F", "G", "H", "I", "J",
                    "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T",
                    "U", "V", "W", "X", "Y", "Z"
                };

            DataTable table = new DataTable();


            foreach (var column in columns)
            {
                table.Columns.Add(createDataColumn(column));
            }

            var csv = new CsvReader(File.OpenText(path));




            while (csv.Read())
            {
                table.Rows.Add(csv.CurrentRecord);
            }

            return table;
        }



        public void CopyDataTableToDB(DataTable table, string destinationTable)
        {

            var conn = new SqlConnection(Clirec_Credentials);

            SqlBulkCopy bulkcopy = new SqlBulkCopy(conn);

            try {
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


        public ConsolidatedSummary getConsolidatedSummary(List<ValuePair> accts, string monthlyPeriod)
        {
            try {
                var mperiod = DateTime.Parse(monthlyPeriod);

                var period = mperiod.ToString("yyyyMM");

                var summary = new ConsolidatedSummary();


                foreach (var act in accts)
                {
                    var L = getAcctSummary('L', act.ID, monthlyPeriod, period);
                    var S = getAcctSummary('S', act.ID, monthlyPeriod, period);

                    summary.TotalLedger = summary.TotalLedger + L.Cbalance;

                    summary.TotalStmt = summary.TotalStmt + S.Cbalance;

                    summary.TotalLedgerCredits = summary.TotalLedgerCredits + L.TotalCredits;

                    summary.TotalStmtCredits = summary.TotalStmtCredits + S.TotalCredits;

                    summary.TotalLedgerDebits = summary.TotalLedgerDebits + L.TotalDebits;

                    summary.TotalStmtDebits = summary.TotalStmtDebits + S.TotalDebits;

                }

                return summary;
            }
            catch (Exception e)
            {
                throw (e);
            }

        }

        public decimal checkIntegrity(AccountDetails details)
        {

            var diff = decimal.Zero;
            try
            {
                var period = details.CurrentDate.ToString("yyyyMM");

                var monthly = details.CurrentDate.ToString("MMMyyyy");

                var Lsummary = getAcctSummary('L', details.AccountCode, monthly, period);

                var Ssummary = getAcctSummary('S', details.AccountCode, monthly, period);

                diff = Convert.ToDecimal((Lsummary.Cbalance - (Lsummary.TotalCredits - Lsummary.TotalDebits)) + (Ssummary.Cbalance - (Ssummary.TotalCredits - Ssummary.TotalDebits)));

                diff = decimal.Round(diff, 2, MidpointRounding.AwayFromZero);


            }
            catch (Exception s)
            {

            }

            return diff;

        }

        private DataColumn createDataColumn(string columnName)
        {
            DataColumn cm = new DataColumn();
            cm.DataType = Type.GetType("System.String");
            cm.ColumnName = columnName;
            cm.Caption = columnName;
            return cm;
        }

        private DataColumn createDataColumn(string columnName, string type)
        {
            DataColumn cm = new DataColumn();
            cm.DataType = Type.GetType(type);
            cm.ColumnName = columnName;
            cm.Caption = columnName;
            return cm;
        }


        public void MassChangeWorkingMonth(List<ValuePair> accts)
        {


            try
            {
                foreach (var acct in accts)
                {
                    var tobject = getManagedSQLTrasaction(Clirec_Credentials);

                    var cmd = tobject.currentCommand;
                    try
                    {


                        var details = getAccountDetails(acct.ID,false);

                        var SqlStm = string.Empty;

                        var newCurdate = details.CurrentDate.AddMonths(1);

                        var currentLedgerTable = details.CurrentDate.ToString("MMMyyyy") + "LedgerDetails";

                        var currentStmTable = details.CurrentDate.ToString("MMMyyyy") + "StmtDetails";

                        var newLastLedgerDate = Utils.getLastDateofMonth(details.CurrentDate);

                        var Curperiod = details.CurrentDate.ToString("yyyyMM");

                        var newperiod = newCurdate.ToString("yyyyMM");

                        var newLedgerTable = newCurdate.ToString("MMMyyyy") + "LedgerDetails";

                        var newStmtTable = newCurdate.ToString("MMMyyyy") + "StmtDetails";

                        SqlStm = " Delete from Account_Restore_Point  where Account_ID = '" + details.AccountCode + "'";
                        SqlStm += " and Month(Action_Time) = " + newCurdate.Month + " and Year(Action_Time) = " + newCurdate.Year;

                        cmd.CommandText = SqlStm;

                        cmd.ExecuteNonQuery();

                        SqlStm = " Delete from Account_Restore_Point_details  where Action_ID in (select Action_ID from Account_Restore_Point where Account_ID = '" + details.AccountCode + "'";
                        SqlStm += " and Month(Action_Time) = " + newCurdate.Month + " and Year(Action_Time) = " + newCurdate.Year + " ) ";

                        cmd.CommandText = SqlStm;

                        cmd.ExecuteNonQuery();


                        SqlStm = "update NLBANKS set CUR_DATE = '" + newCurdate.getValidFormat() + "', LASTDATE = '" + newLastLedgerDate + "' where BANKCODE ='" + details.AccountCode + "'";

                        cmd.CommandText = SqlStm;

                        cmd.ExecuteNonQuery();

                        if (!FindTable(newLedgerTable))
                        {
                            Execute(Scripts.CreateMonthlyTable.Replace("monthlyTable", newLedgerTable));
                            try
                            {
                                var sql = " Create Trigger Generate_Serial_No_LED_" + newperiod + "  On " + newLedgerTable + "  FOR Insert ";
                                sql = sql + " As declare @tranId int;";
                                sql = sql + " select  @tranId = i.tran_Id from inserted i; ";
                                sql = sql + " Update " + newLedgerTable + "  Set serial_no = " + newLedgerTable + ".tran_Id";
                                sql = sql + " Where  " + newLedgerTable + ".Tran_Id in (select tran_Id from inserted)";

                                Execute(sql);
                            }
                            catch { }



                        }
                        else
                        {
                            cmd.CommandText = "delete from " + newLedgerTable + " where Account_ID='" + details.AccountCode + "'";

                            cmd.ExecuteNonQuery();
                        }


                        if (!FindTable(newStmtTable))
                        {
                            Execute(Scripts.CreateMonthlyTable.Replace("monthlyTable", newStmtTable));
                            try
                            {
                                var sql = " Create Trigger Generate_Serial_No_LED_" + newperiod + "  On " + newStmtTable + "  FOR Insert ";
                                sql = sql + " As declare @tranId int;";
                                sql = sql + " select  @tranId = i.tran_Id from inserted i; ";
                                sql = sql + " Update " + newStmtTable + "  Set serial_no = " + newStmtTable + ".tran_Id";
                                sql = sql + " Where  " + newStmtTable + ".Tran_Id in (select tran_Id from inserted)";

                                Execute(sql);
                            }
                            catch { }



                        }
                        else
                        {
                            cmd.CommandText = "delete from " + newStmtTable + " where Account_ID='" + details.AccountCode + "'";

                            cmd.ExecuteNonQuery();
                        }





                        SqlStm = " Insert into " + newLedgerTable + " (Acctno,Details,Matches,MatchType,Remarks,Serial_no,Postdate,Valdate,CrDr, ";
                        SqlStm += " Amount,Analcode,Period,idfields,Tracer_Note,MatchID,UD1,UD2,UD3,UD4,UD5,Account_ID,userid,comment,MatchRef,MatchKeyWord ) ";
                        SqlStm += " Select Acctno,Details,Matches,MatchType,Remarks,Serial_no,Postdate,Valdate,CrDr,Amount,Analcode,Period,idfields,Tracer_Note,MatchID,UD1,UD2,UD3,UD4,UD5,Account_ID,userid,comment,MatchRef,MatchKeyWord";
                        SqlStm += "  from " + currentLedgerTable + "  Where (matchtype is null or matchtype = '') ";
                        SqlStm += " and Account_id = '" + details.AccountCode + "'";

                        cmd.CommandText = SqlStm;

                        cmd.ExecuteNonQuery();

                        SqlStm = " Insert into " + newStmtTable + " (Acctno,Details,Matches,MatchType,Remarks,Serial_no,Postdate,Valdate,CrDr, ";
                        SqlStm += " Amount,Analcode,Period,idfields,Tracer_Note,MatchID,UD1,UD2,UD3,UD4,UD5,Account_ID,userid,comment,MatchRef,MatchKeyWord ) ";
                        SqlStm += " Select Acctno,Details,Matches,MatchType,Remarks,Serial_no,Postdate,Valdate,CrDr,Amount,Analcode,Period,idfields,Tracer_Note,MatchID,UD1,UD2,UD3,UD4,UD5,Account_ID,userid,comment,MatchRef,MatchKeyWord";
                        SqlStm += "  from " + currentStmTable + "  Where (matchtype is null or matchtype = '') ";
                        SqlStm += " and Account_id = '" + details.AccountCode + "'";

                        cmd.CommandText = SqlStm;

                        cmd.ExecuteNonQuery();

                        SqlStm = "delete from LedgerBal where Account_ID ='" + details.AccountCode + "' and Period ='" + newperiod + "'";

                        cmd.CommandText = SqlStm;

                        cmd.ExecuteNonQuery();

                        SqlStm = "delete from StmtBal where Account_ID ='" + details.AccountCode + "' and Period ='" + newperiod + "'";

                        cmd.CommandText = SqlStm;

                        cmd.ExecuteNonQuery();

                        SqlStm = "insert into LedgerBal(OpenBal,Acctno,Account_ID,Period,CloseBal) select OpenBal,Acctno,Account_ID, '" + newperiod + "' ,CloseBal from LedgerBal where Account_ID ='" + details.AccountCode + "' and Period = '" + Curperiod + "'";

                        cmd.CommandText = SqlStm;

                        cmd.ExecuteNonQuery();

                        SqlStm = "insert into StmtBal(OpenBal,Acctno,Account_ID,Period,CloseBal) select OpenBal,Acctno,Account_ID, '" + newperiod + "' ,CloseBal from StmtBal where Account_ID ='" + details.AccountCode + "' and Period = '" + Curperiod + "'";

                        cmd.CommandText = SqlStm;

                        cmd.ExecuteNonQuery();

                        tobject.currentTransaction.Commit();
                    }
                    catch (Exception t)
                    {
                        Utils.Log("Mass Change Working Month Failed for Account " + acct.Value + " because: " + t.Message);
                        try
                        {
                            tobject.currentTransaction.Rollback();
                        }
                        catch
                        {

                        }

                    }
                    finally
                    {
                        tobject.currentConnection.Close();
                        tobject.currentTransaction.Dispose();
                    }

                }
            }
            catch (Exception t)
            {
                throw (t);
            }
        }

        public List<ValuePair> getAcctPeriods(string acctid, HttpContext context)
        {
            var details = getAccountDetails(acctid,false);

            var periods = new List<ValuePair>();

            context.Session["AccountDetails"] = details;

            var startdate = Convert.ToDateTime(getDataSet("select START_DATE from NLBANKS where BANKCODE ='" + details.AccountCode + "' ").Rows[0]["START_DATE"]);

            var lang = (JObject)context.Session["LanguageObject"];

            while (startdate <= details.CurrentDate)
            {
                if (startdate.ToString("MMMyyyy").Equals(details.CurrentDate.ToString("MMMyyyy")))
                {
                    startdate = details.CurrentDate;
                }

                var p = new ValuePair();

                p.ID = startdate.ToString("MMMyyyy");
                p.Value = lang.GetValue(startdate.ToString("MMMMM")).ToString() + " " + startdate.ToString("yyyy");

                startdate = startdate.AddMonths(1);

                periods.Add(p);

            }

            periods.Reverse();

            return periods;

        }

        public List<ValuePair> getAcctPeriods(string acctid, System.Web.UI.Page context)
        {
            var details = getAccountDetails(acctid,false);

            var periods = new List<ValuePair>();

            context.Session["AccountDetails"] = details;

            var startdate = Convert.ToDateTime(getDataSet("select START_DATE from NLBANKS where BANKCODE ='" + details.AccountCode + "' ").Rows[0]["START_DATE"]);

            var lang = (JObject)context.Session["LanguageObject"];

            while (startdate <= details.CurrentDate)
            {
                if (startdate.ToString("MMMyyyy").Equals(details.CurrentDate.ToString("MMMyyyy")))
                {
                    startdate = details.CurrentDate;
                }

                var p = new ValuePair();

                p.ID = startdate.ToString("MMMyyyy");
                p.Value = lang.GetValue(startdate.ToString("MMMMM")).ToString() + " " + startdate.ToString("yyyy");

                startdate = startdate.AddMonths(1);

                periods.Add(p);

            }

            periods.Reverse();

            return periods;

        }



        public bool FindTable(string SrchTable)
        {

            DataTable rst;
            bool FindTable = false;

            string SqlStm = "Select * from sysobjects ";
            SqlStm += " where id = object_id('dbo." + SrchTable.Trim() + "')";
            SqlStm += " and sysstat & 0xf = 3";

            rst = getDataSet(SqlStm);

            if (rst.Rows.Count != 0)
            {

                FindTable = true;
            }

            return FindTable;
        }

        public decimal getPreAdjustmentBalance(string acctid, string Mperiod)
        {
            decimal  prebalance = decimal.Zero;

            var table = Mperiod + "LedgerDetails";

            var period = Mperiod.toDateTime().ToString("yyyyMM");

            var sql = "select (select sum(Amount) from "+table+" where crdr = 2 and Account_ID ='"+acctid+"' and Tracer_Note ='Post Adjustment') - " +
                     " (select sum(Amount) from "+ table + " where crdr = 1 and Account_ID = '"+acctid+"'and Tracer_Note = 'Post Adjustment')";

            var adjustmentTotal = ExecuteScalar(sql).toDecimal();

            sql = "select closebal from LedgerBal where Account_ID ='"+acctid+"' and Period='"+period+"'";

            var periodBalance = ExecuteScalar(sql).toDecimal();

            prebalance = periodBalance - adjustmentTotal;

            return prebalance;
        }

        public bool ISAdjustedPeriod(string acctid, string Mperiod)
        {
            var table = Mperiod + "LedgerDetails";

            var sql = "select Count(Amount) from "+table+" where Account_ID='"+acctid+"' and Tracer_Note ='Post Adjustment'";

            var  nofA = ExecuteScalar(sql).toInt();

            return nofA > 0;
        }

        public UDDefinitions getUDDefinitions(string gDomain)//, String period, String constr)
        {
            UDDefinitions ud = new UDDefinitions();
            //  string ud1 = "";string ud2="";string ud3="";string ud4="";string ud5="";


            DataTable dt;
            try
            {
                dt = getDataSet("select * from uddefs_web where Domain = '" + gDomain + "'");


                if (dt.Rows.Count > 0)
                {

                    ud.ud1 = getString(dt.Rows[0]["ud1def"].ToString());
                    ud.ud2 = getString(dt.Rows[0]["ud2def"].ToString());
                    ud.ud3 = getString(dt.Rows[0]["ud3def"].ToString());
                    ud.ud4 = getString(dt.Rows[0]["ud4def"].ToString());
                    ud.ud5 = getString(dt.Rows[0]["ud5def"].ToString());
                }
                ud.ud1 = string.IsNullOrEmpty(ud.ud1) ? "" : ud.ud1;
                ud.ud2 = string.IsNullOrEmpty(ud.ud2) ? "" : ud.ud2;
                ud.ud3 = string.IsNullOrEmpty(ud.ud3) ? "" : ud.ud3;
                ud.ud4 = string.IsNullOrEmpty(ud.ud4) ? "" : ud.ud4;
                ud.ud5 = string.IsNullOrEmpty(ud.ud5) ? "" : ud.ud5;
            }
            catch { }


            return ud;
        }


    }

}
