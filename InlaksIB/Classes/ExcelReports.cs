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
            var sql = "select a.*, b.BranchName,b.BranchAddress, c.Narrative,d.BirthDate,d.FirstName as CustomerName from  factStmt a  join DimBranch b on (a.BranchId=b.SourceBranchId) join dimStmt c on (a.TransactionCode=TransactionId) join DimCustomer d on (a.CustomerId=d.CustomerId) where a.BusinessDate between '"+startdate+ "'  and '" + enddate + "'";

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


            var headers = new string[] { "EntryID", "AccountID", "BranchId", "Amount", "CustomerId",  "CustomerName", "ValueDate", "Currency", "OurReference", "TransactionReference", "PostDate", "Inputer", "Authoriser", "CrDr", "Time", "BranchName", "BranchAddress", "Narrative", "Birthdate" };


            dt = new DataView(dt).ToTable(true,headers);


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



    }
}