using BackBone;
using InlaksIB.Properties;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace InlaksIB.Classes
{
    public class ExcelReports
    {
        public string generateNPFAntiMoneyLaunderingReport(string startdate, string enddate, string downpath, string reportname)
        {

            //DateTime start = DateTime.ParseExact(startdate, "yyyy-MM-dd", new System.Globalization.CultureInfo("en-US"));
           // DateTime end = DateTime.ParseExact(enddate, "yyyy-MM-dd", new System.Globalization.CultureInfo("en-US"));
            var sql = "select a.*, b.BranchName,b.BranchAddress, c.Narrative,d.BirthDate,d.FirstName as CustomerName,d.DefaultPhone as Telephone,d.Address as CustomerAddress, e.Alt_AcctId as OldAccountNo from  factStmt a  join DimBranch b on (a.BranchId=b.SourceBranchId) join dimStmt c on (a.TransactionCode=TransactionId) join DimCustomer d on (a.CustomerId=d.CustomerId) join DimAccount e on (a.AccountID=e.AccountId) where a.BusinessDate between '" + startdate+ "'  and '" + enddate + "'";

            var db = new InlaksBIContext().getDBInterface( new Settings().warehousedbtype, new Settings().warehousedb);

            var dt = db.getData(sql);

            if(dt.Rows.Count == 0)
            {
                throw new Exception("No records found!");
            }

            downpath = downpath + startdate + "-" + enddate + reportname + ".xlsx";


            ExcelWorksheet oSheet;
            ExcelPackage xlPackage;


            FileInfo newFile = new FileInfo(downpath);

            if (newFile.Exists)
            {
                newFile.Delete();
            }
            xlPackage = new ExcelPackage(newFile);


            oSheet = xlPackage.Workbook.Worksheets.Add(reportname);

            int curRow = 3;


            var headers = new string[] { "EntryID", "AccountID", "OldAccountNo", "BranchId", "Amount", "CustomerId",  "CustomerName", "Telephone", "CustomerAddress", "ValueDate", "Currency", "OurReference", "TransactionReference", "PostDate", "Inputer", "Authoriser", "CrDr", "Time", "BranchName", "BranchAddress", "Narrative", "Birthdate" };


            dt = new DataView(dt).ToTable(true, headers);


            var branchgroups = dt.AsEnumerable().GroupBy(r => r["BranchId"].ToString()).Select(grp => grp.ToList());


            foreach (var branchdata in branchgroups)
            {


                oSheet.Cells[curRow, 2].Value = "NPF MICROFINANCE BANK PLC";
                oSheet.Cells[curRow, 2].Style.Font.Bold = true;
                oSheet.Cells[curRow, 2].Style.Font.UnderLine = false;
                //oSheet.Cells[3, 1, 3, 13].Merge = true;

                curRow = curRow + 1;

                oSheet.Cells[curRow, 2].Value = branchdata[0]["BranchName"].ToString();
                oSheet.Cells[curRow, 2].Style.Font.Bold = true;
                oSheet.Cells[curRow, 2].Style.Font.UnderLine = false;

                curRow = curRow + 1;

                oSheet.Cells[curRow, 2].Value = branchdata[0]["BranchAddress"].ToString();
                oSheet.Cells[curRow, 2].Style.Font.Bold = true;
                oSheet.Cells[curRow, 2].Style.Font.UnderLine = false;

                curRow = curRow + 1;

                oSheet.Cells[curRow, 2].Value = reportname;
                oSheet.Cells[curRow, 2].Style.Font.Bold = true;
                oSheet.Cells[curRow, 2].Style.Font.UnderLine = false;

                curRow = curRow + 2;




                for (int i = 0; i < headers.Length; i++)
                {
                    oSheet.Cells[curRow, 2 + i].Value = headers[i];
                    oSheet.Cells[curRow, 2 + i].Style.Font.Bold = true;
                    oSheet.Cells[curRow, 2 + i].Style.Font.UnderLine = false;

                }

                curRow = curRow + 1;

                foreach (DataRow row in branchdata) { 


                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    oSheet.Cells[curRow, i+2].Value = row[i].ToString();
                        
                }

                    curRow = curRow + 1;

                }
                curRow = curRow + 4;

            }

            xlPackage.Save();

            return downpath;
        }





        public string generateNPFSavingsAccountsBalanceReport(string downpath, string reportname)
        {

            string today = DateTime.Now.ToString("dd-MM-yyyy");

            //DateTime start = DateTime.ParseExact(startdate, "yyyy-MM-dd", new System.Globalization.CultureInfo("en-US"));
            // DateTime end = DateTime.ParseExact(enddate, "yyyy-MM-dd", new System.Globalization.CultureInfo("en-US"));
            var sql = "SELECT a.*,b.CategoryName,(Select LockedAmount from AcLockedEvents where AccountNumber = a.AccountId) as LockedAmount FROM dimAccount a join factCategory b on (a.category = b.categoryid) where a.category in (select * from NPFSavingsAcctBal)";

            var db = new InlaksBIContext().getDBInterface(new Settings().warehousedbtype, new Settings().warehousedb);

            var dt = db.getData(sql);

            if (dt.Rows.Count == 0)
            {
                throw new Exception("No records found!");
            }

            downpath = downpath + today + "_" + reportname + ".xlsx";


            ExcelWorksheet oSheet;
            ExcelPackage xlPackage;


            FileInfo newFile = new FileInfo(downpath);

            if (newFile.Exists)
            {
                newFile.Delete();
            }
            xlPackage = new ExcelPackage(newFile);


            oSheet = xlPackage.Workbook.Worksheets.Add(reportname);

            int curRow = 3;

            DataTable PrintTable = new DataTable();


            var headers = new string[] { "Category Name", "Amount Blocked", "Balance" };

            //dt = new DataView(dt).ToTable(true, headers);

            int data_count;

            decimal Acct_bal_Sum;

            decimal Acct_locked_Sum;

            var branchgroups = dt.AsEnumerable().GroupBy(r => r["CategoryName"].ToString()).Select(grp => grp.ToList());

            oSheet.Cells[curRow, 3].Value = "NPF MICROFINANCE BANK PLC";
            oSheet.Cells[curRow, 3].Style.Font.Bold = true;
            oSheet.Cells[curRow, 3].Style.Font.UnderLine = false;
            //oSheet.Cells[3, 1, 3, 13].Merge = true;

            curRow = curRow + 1;

            oSheet.Cells[curRow, 3].Value = "ALL BRANCHES";
            oSheet.Cells[curRow, 3].Style.Font.Bold = true;
            oSheet.Cells[curRow, 3].Style.Font.UnderLine = false;

            curRow = curRow + 1;

            oSheet.Cells[curRow, 3].Value = "Savings Account Balance Report";
            oSheet.Cells[curRow, 3].Style.Font.Bold = true;
            oSheet.Cells[curRow, 3].Style.Font.UnderLine = false;

            curRow = curRow + 1;


            oSheet.Cells[curRow, 3].Value = "Reporting Date: " + DateTime.Now.ToString("dd/MM/yyyy");
            oSheet.Cells[curRow, 3].Style.Font.Bold = true;
            oSheet.Cells[curRow, 3].Style.Font.UnderLine = false;

            curRow = curRow + 3;

            for (int i = 0; i < headers.Length; i++)
            {
                oSheet.Cells[curRow, 2 + i].Value = headers[i];
                oSheet.Cells[curRow, 2 + i].Style.Font.Bold = true;
                oSheet.Cells[curRow, 2 + i].Style.Font.UnderLine = true;
            }

            //curRow = curRow + 1;

            foreach (var branchdata in branchgroups)
            {

                curRow = curRow + 1;

                //oSheet.Cells[curRow, 2].Value = branchdata[0]["CategoryName"].ToString();
                //oSheet.Cells[curRow, 2].Style.Font.Bold = true;
                //oSheet.Cells[curRow, 2].Style.Font.UnderLine = false;

                //curRow = curRow + 1;

                //oSheet.Cells[curRow, 2].Value = branchdata[0]["BranchAddress"].ToString();
                //oSheet.Cells[curRow, 2].Style.Font.Bold = true;
                //oSheet.Cells[curRow, 2].Style.Font.UnderLine = false;

                //curRow = curRow + 1;

                //oSheet.Cells[curRow, 2].Value = reportname;
                //oSheet.Cells[curRow, 2].Style.Font.Bold = true;
                //oSheet.Cells[curRow, 2].Style.Font.UnderLine = false;

                //curRow = curRow + 2;




                //foreach (DataRow row in branchdata)

                //{

                //for (int i = 0; i < dt.Columns.Count; i++)
                //{
                //    oSheet.Cells[curRow, i + 2].Value = row[i].ToString();

                //}

                //curRow = curRow + 1;

                //oSheet.Cells[curRow, 2].Value = branchdata[0]["BranchAddress"].ToString();

                data_count = branchdata.Count();

                Acct_locked_Sum = branchdata.Sum(r => r["LockedAmount"].toDecimal());

                if (Acct_locked_Sum.isNull())
                {
                    Acct_locked_Sum = 0;
                }

                Acct_bal_Sum = branchdata.Sum(r => r["Balance"].toDecimal());

                oSheet.Cells[curRow, 2].Value = branchdata[0]["CategoryName"].ToString();
                oSheet.Cells[curRow, 2].Style.Font.Bold = true;


                oSheet.Cells[curRow, 3].Value = Acct_locked_Sum;
                oSheet.Cells[curRow, 3].Style.Numberformat.Format = "#,##0.00";

                oSheet.Cells[curRow, 4].Value = Acct_bal_Sum;
                oSheet.Cells[curRow, 4].Style.Numberformat.Format = "#,##0.00";

                curRow = curRow + 1;

                oSheet.Cells[curRow, 2].Value = "Total Number Of accounts : " + data_count.ToString("#,###");
                curRow = curRow + 3;

            }

            xlPackage.Save();

            return downpath;
        }




    }
}