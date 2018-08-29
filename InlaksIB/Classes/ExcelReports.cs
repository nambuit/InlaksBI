using BackBone;
using InlaksIB.Properties;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;

namespace InlaksIB.Classes
{
    public class ExcelReports
    {



        public string generateConsolSpecExtractReport(string startdate, string enddate, string downpath, string reportname, string company)
        {

            //DateTime start = DateTime.ParseExact(startdate, "yyyy-MM-dd", new System.Globalization.CultureInfo("en-US"));
            // DateTime end = DateTime.ParseExact(enddate, "yyyy-MM-dd", new System.Globalization.CultureInfo("en-US"));

            // var filter = filtervalue.CleanUp().Equals("null") ? "" : " and CategoryName='" + filtervalue + "'";

            var filter = (company.CleanUp().Equals("null") ? "" : " and BranchId='" + company + "'");

            var sql = "select * from ConsolSpecExtract where BusinessDate between '" + startdate + "'  and '" + enddate + "'" + filter;

            var db = new InlaksBIContext().getDBInterface(new Settings().warehousedbtype, new Settings().warehousedb);

            var dt = db.getData(sql);

            if (dt.Rows.Count == 0)
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


            var headers = new string[dt.Columns.Count];// { "EntryID", "AccountID", "OldAccountNo", "BranchId", "Amount", "CustomerId",  "CustomerName", "Telephone", "CustomerAddress", "ValueDate", "Currency", "OurReference", "TransactionReference", "PostDate", "Inputer", "Authoriser", "CrDr", "Time", "BranchName", "BranchAddress", "Narrative", "Birthdate" };
            var Column = new string[dt.Columns.Count];

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                Column[i] = dt.Columns[i].ColumnName;
                headers[i] = dt.Columns[i].ColumnName;

            }



            //dt = new DataView(dt).ToTable(false, headers);


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


                oSheet.Cells[curRow, 2].Value = reportname;
                oSheet.Cells[curRow, 2].Style.Font.Bold = true;
                oSheet.Cells[curRow, 2].Style.Font.UnderLine = false;

                curRow = curRow + 2;


                System.Drawing.Color green = new System.Drawing.Color();
                for (int i = 0; i < headers.Length; i++)
                {
                    oSheet.Cells[curRow, i + 1].Value = headers[i];
                    oSheet.Cells[curRow, i + 1].Style.Font.Bold = true;
                    oSheet.Cells[curRow, i + 1].Style.Font.UnderLine = true;
                    oSheet.Cells[curRow, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.LightGray;
                    oSheet.Cells[curRow, i + 1].Style.Fill.BackgroundColor.SetColor(green);
                }

                curRow = curRow + 1;

                for (int i = 0; i < headers.Length; i++)
                {

                    oSheet.Cells[curRow, i + 1].Value = "";
                }

                if (dt.Rows.Count != 0)
                {
                    int count = 0;

                    foreach (var drow in branchdata.AsEnumerable())
                    {
                        count = count + 1;
                        //throw new Exception("No records found!");
                        for (int i = 0; i < headers.Length; i++)
                        {
                            if (Column[i].isNull())
                            {
                                Column[i] = "";
                            }

                            if (Column[i] != "")
                            {
                                if (Column[i].Trim() == "SN")
                                {
                                    oSheet.Cells[curRow, i + 1].Value = count;
                                }
                                else
                                {


                                    oSheet.Cells[curRow, i + 1].Value = drow[Column[i]].ToString();

                                    if (Column[i].Trim() == "Amount")
                                    {
                                        oSheet.Cells[curRow, i + 1].Value = drow[Column[i]].toDecimal();
                                        oSheet.Cells[curRow, i + 1].Style.Numberformat.Format = "#,##0.00";
                                    }

                                }




                            }

                            else

                            {

                                oSheet.Cells[curRow, i + 1].Value = "";

                            }
                        }

                        curRow = curRow + 1;

                    }
                }


                curRow = curRow + 4;

            }

            xlPackage.Save();

            return downpath;
        }


        public string generateStmtExtractReport(string startdate, string enddate, string downpath, string reportname, string company)
        {

            //DateTime start = DateTime.ParseExact(startdate, "yyyy-MM-dd", new System.Globalization.CultureInfo("en-US"));
            // DateTime end = DateTime.ParseExact(enddate, "yyyy-MM-dd", new System.Globalization.CultureInfo("en-US"));

           // var filter = filtervalue.CleanUp().Equals("null") ? "" : " and CategoryName='" + filtervalue + "'";

          var  filter =  (company.CleanUp().Equals("null") ? "" : " and BranchId='" + company + "'");

            var sql = "select * from StmtExtract where BusinessDate between '" + startdate + "'  and '" + enddate + "'"+filter;

            var db = new InlaksBIContext().getDBInterface(new Settings().warehousedbtype, new Settings().warehousedb);

            var dt = db.getData(sql);

            if (dt.Rows.Count == 0)
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


            var headers = new string[dt.Columns.Count];// { "EntryID", "AccountID", "OldAccountNo", "BranchId", "Amount", "CustomerId",  "CustomerName", "Telephone", "CustomerAddress", "ValueDate", "Currency", "OurReference", "TransactionReference", "PostDate", "Inputer", "Authoriser", "CrDr", "Time", "BranchName", "BranchAddress", "Narrative", "Birthdate" };
            var Column = new string[dt.Columns.Count];

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                Column[i] = dt.Columns[i].ColumnName;
                headers[i] = dt.Columns[i].ColumnName;

            }



            //dt = new DataView(dt).ToTable(false, headers);


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


                oSheet.Cells[curRow, 2].Value = reportname;
                oSheet.Cells[curRow, 2].Style.Font.Bold = true;
                oSheet.Cells[curRow, 2].Style.Font.UnderLine = false;

                curRow = curRow + 2;


                System.Drawing.Color green = new System.Drawing.Color();
                for (int i = 0; i < headers.Length; i++)
                {
                    oSheet.Cells[curRow, i + 1].Value = headers[i];
                    oSheet.Cells[curRow, i + 1].Style.Font.Bold = true;
                    oSheet.Cells[curRow, i + 1].Style.Font.UnderLine = true;
                    oSheet.Cells[curRow, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.LightGray;
                    oSheet.Cells[curRow, i + 1].Style.Fill.BackgroundColor.SetColor(green);
                }

                curRow = curRow + 1;

                for (int i = 0; i < headers.Length; i++)
                {

                    oSheet.Cells[curRow, i + 1].Value = "";
                }

                if (dt.Rows.Count != 0)
                {
                    int count = 0;

                    foreach (var drow in branchdata.AsEnumerable())
                    {
                        count = count + 1;
                        //throw new Exception("No records found!");
                        for (int i = 0; i < headers.Length; i++)
                        {
                            if (Column[i].isNull())
                            {
                                Column[i] = "";
                            }

                            if (Column[i] != "")
                            {
                                if (Column[i].Trim() == "SN")
                                {
                                    oSheet.Cells[curRow, i + 1].Value = count;
                                }
                                else
                                {


                                    oSheet.Cells[curRow, i + 1].Value = drow[Column[i]].ToString();

                                    if (Column[i].Trim() == "Amount")
                                    {
                                        oSheet.Cells[curRow, i + 1].Value = drow[Column[i]].toDecimal();
                                        oSheet.Cells[curRow, i + 1].Style.Numberformat.Format = "#,##0.00";
                                    }

                                }




                            }

                            else

                            {

                                oSheet.Cells[curRow, i + 1].Value = "";

                            }
                        }

                        curRow = curRow + 1;

                    }
                }


                curRow = curRow + 4;

            }

            xlPackage.Save();

            return downpath;
        }



        public string generateCategExtractReport(string startdate, string enddate, string downpath, string reportname, string company,string filtervalue, string product)
        {

            //DateTime start = DateTime.ParseExact(startdate, "yyyy-MM-dd", new System.Globalization.CultureInfo("en-US"));
            // DateTime end = DateTime.ParseExact(enddate, "yyyy-MM-dd", new System.Globalization.CultureInfo("en-US"));

            var filter = filtervalue.CleanUp().Equals("null") ? "" : " and CategoryName='"+filtervalue+"'";

            filter = filter+(company.CleanUp().Equals("null") ? "" : " and BranchId='" + company + "'");

            filter = filter + (product.CleanUp().Equals("null") ? "" : " and Product='" + product + "'");

            var sql = "select * from CategExtract where BusinessDate between '" + startdate + "'  and '" + enddate + "'"+filter;

            var db = new InlaksBIContext().getDBInterface(new Settings().warehousedbtype, new Settings().warehousedb);

            var dt = db.getData(sql);

            if (dt.Rows.Count == 0)
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


            var headers = new string[dt.Columns.Count];// { "EntryID", "AccountID", "OldAccountNo", "BranchId", "Amount", "CustomerId",  "CustomerName", "Telephone", "CustomerAddress", "ValueDate", "Currency", "OurReference", "TransactionReference", "PostDate", "Inputer", "Authoriser", "CrDr", "Time", "BranchName", "BranchAddress", "Narrative", "Birthdate" };
            var Column = new string[dt.Columns.Count];

            for(int i =0;i< dt.Columns.Count;i++)
            {
                Column[i] = dt.Columns[i].ColumnName;
                headers[i] = dt.Columns[i].ColumnName;

            }

         

            //dt = new DataView(dt).ToTable(false, headers);


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


                oSheet.Cells[curRow, 2].Value = reportname;
                oSheet.Cells[curRow, 2].Style.Font.Bold = true;
                oSheet.Cells[curRow, 2].Style.Font.UnderLine = false;

                curRow = curRow + 2;


                System.Drawing.Color green = new System.Drawing.Color();
                for (int i = 0; i < headers.Length; i++)
                {
                    oSheet.Cells[curRow, i + 1].Value = headers[i];
                    oSheet.Cells[curRow, i + 1].Style.Font.Bold = true;
                    oSheet.Cells[curRow, i + 1].Style.Font.UnderLine = true;
                    oSheet.Cells[curRow, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.LightGray;
                    oSheet.Cells[curRow, i + 1].Style.Fill.BackgroundColor.SetColor(green);
                }

                curRow = curRow + 1;

                for (int i = 0; i < headers.Length; i++)
                {

                    oSheet.Cells[curRow, i + 1].Value = "";
                }

                if (dt.Rows.Count != 0)
                {
                    int count = 0;

                    foreach (var drow in branchdata.AsEnumerable())
                    {
                        count = count + 1;
                        //throw new Exception("No records found!");
                        for (int i = 0; i < headers.Length; i++)
                        {
                            if (Column[i].isNull())
                            {
                                Column[i] = "";
                            }

                            if (Column[i] != "")
                            {
                                if (Column[i].Trim() == "SN")
                                {
                                    oSheet.Cells[curRow, i + 1].Value = count;
                                }
                                else
                                {


                                    oSheet.Cells[curRow, i + 1].Value = drow[Column[i]].ToString();

                                    if (Column[i].Trim() == "Amount")
                                    {
                                        oSheet.Cells[curRow, i + 1].Value = drow[Column[i]].toDecimal();
                                        oSheet.Cells[curRow, i + 1].Style.Numberformat.Format = "#,##0.00";
                                    }

                                }




                            }

                            else

                            {

                                oSheet.Cells[curRow, i + 1].Value = "";

                            }
                        }

                        curRow = curRow + 1;

                    }
                }


                curRow = curRow + 4;

            }

            xlPackage.Save();

            return downpath;
        }








        public string generateNPFAntiMoneyLaunderingReport(string startdate, string enddate, string downpath, string reportname)
        {

            //DateTime start = DateTime.ParseExact(startdate, "yyyy-MM-dd", new System.Globalization.CultureInfo("en-US"));
            // DateTime end = DateTime.ParseExact(enddate, "yyyy-MM-dd", new System.Globalization.CultureInfo("en-US"));
            var sql = "select * from atmldetails where PostDate between '" + startdate + "'  and '" + enddate + "'";

            var db = new InlaksBIContext().getDBInterface(new Settings().warehousedbtype, new Settings().warehousedb);

            var dt = db.getData(sql);

            if (dt.Rows.Count == 0)
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


            //var headers = new string[] { "EntryID", "AccountID", "OldAccountNo", "BranchId", "Amount", "CustomerId",  "CustomerName", "Telephone", "CustomerAddress", "ValueDate", "Currency", "OurReference", "TransactionReference", "PostDate", "Inputer", "Authoriser", "CrDr", "Time", "BranchName", "BranchAddress", "Narrative", "Birthdate" };


            //dt = new DataView(dt).ToTable(false, headers);


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


                //}     IDENTIFICATION	REGISTRATION	DATE OF	PLACE OF	ISSUING	CUSTOMER 	FIRST ADDRESS	SECOND ADDRESS
                //NUMBER CERTIFICATE NO.ISSUE ISSUE   AUTHORITY ADDRESS  TYPE LINE    LINE


                var headers = new string[] { "S/N", "BRANCH CODE", "BANK CODE", "REPORT TYPE", "CUSTOMER TYPE", "SURNAME/NAME OF ORGANISATION", "FIRST NAME", "MIDDLE NAME", "NATIONALITY", "D.O.B", "D.O.I","OCCUPATION","LINE OF BUS.","TYPE OF INDENTIFICATION", "IDENTIFICATION NUMBER","REGISTRATION CERTIFICATE NO.", "DATE OF ISSUE","PLACE OF ISSUE","ISSUING AUTHORITY",
                           "CUSTOMER ADDRESS TYPE","FIRST ADDRESS LINE","SECOND ADDRESS LINE","TOWN/CITY","STATE","TELEPHONE","EMAIL","ACCOUNT TYPE","ACCOUNT NO.","ACCOUNT STATUS","DATE ACCOUNT WAS OPENED","LINKED/CONNECTED ACCOUNTS","TRANSACTION NUMBER","TRANSACTION DATE","TRANSACTION TYPE","DR/CR","TRANSACTION PARTICULARS","CURRENCY TYPE","AMOUNT","PURPOSE OF TRANSACTION","SOURCE/ORIGIN OF FUND","NAME OF BENEFICIARY","ADDRESS OF BENEFICIARY", "REASON FOR SUSPICION" };

                var Column = new string[] { "SN", "BranchId", "bankcode", "reportype", "", "CustomerName", "", "", "nationality", "Birthdate", "Birthdate", "", "", "", "", "", "", "", "", "", "CustomerAddress", "", "", "", "Telephone", "", "", "AccountID", "", "OpenDate", "", "OurReference", "BusinessDate", "transactiontype", "CrDr", "Narrative", "currencytype", "Amount", "", "", "", "", "" };


                System.Drawing.Color green = new System.Drawing.Color();
                for (int i = 0; i < headers.Length; i++)
                {
                    oSheet.Cells[curRow, i + 1].Value = headers[i];
                    oSheet.Cells[curRow, i + 1].Style.Font.Bold = true;
                    oSheet.Cells[curRow, i + 1].Style.Font.UnderLine = true;
                    oSheet.Cells[curRow, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.LightGray;
                    oSheet.Cells[curRow, i + 1].Style.Fill.BackgroundColor.SetColor(green);
                }

                curRow = curRow + 1;

                for (int i = 0; i < headers.Length; i++)
                {

                    oSheet.Cells[curRow, i + 1].Value = "";
                }

                if (dt.Rows.Count != 0)
                {
                    int count = 0;

                    foreach (var drow in branchdata.AsEnumerable())
                    {
                        count = count + 1;
                        //throw new Exception("No records found!");
                        for (int i = 0; i < headers.Length; i++)
                        {
                            if (Column[i].isNull())
                            {
                                Column[i] = "";
                            }

                            if (Column[i] != "")
                            {
                                if (Column[i].Trim() == "SN")
                                {
                                    oSheet.Cells[curRow, i + 1].Value = count;
                                }
                                else
                                {


                                    oSheet.Cells[curRow, i + 1].Value = drow[Column[i]].ToString();

                                    if (Column[i].Trim() == "Amount")
                                    {
                                        oSheet.Cells[curRow, i + 1].Value = drow[Column[i]].toDecimal();
                                        oSheet.Cells[curRow, i + 1].Style.Numberformat.Format = "#,##0.00";
                                    }

                                }




                            }

                            else

                            {

                                oSheet.Cells[curRow, i + 1].Value = "";

                            }
                        }

                        curRow = curRow + 1;

                    }
                }


                curRow = curRow + 4;

            }

            xlPackage.Save();

            return downpath;
        }

        
          public string generateNPFManagementAccounts(string downpath, string reportname)
        {

            var sql = "select a.*,b.* from ma_details a, npf_ma_params b where a.Classification = b.Item_Description order by sheetname";

            var db = new InlaksBIContext().getDBInterface(new Settings().warehousedbtype, new Settings().warehousedb);

            var dt = db.getData(sql);
            
            downpath = downpath + DateTime.Now.ToString("dd-MMM-yyyy") + reportname + ".xlsx";


            ExcelWorksheet currentsheet;
            
            ExcelPackage xlPackage;


            FileInfo newFile = new FileInfo(downpath);

            if (newFile.Exists)
            {
                newFile.Delete();
            }
            xlPackage = new ExcelPackage(newFile);

            var referenceid = Guid.NewGuid().ToString();

            int curRow = 2;

            var summarydata = db.getData("select top 1 * from summarytemp");

            summarydata = summarydata.Clone();

            


            var sheetgroups = dt.AsEnumerable().GroupBy(r => r["SheetName"].ToString()).Select(grp => grp.ToList());

            foreach (var sheetgrp in sheetgroups)
            {
                curRow = 2;

                currentsheet = xlPackage.Workbook.Worksheets.Add(sheetgrp[0]["SheetName"].ToString());

                

                var acctclassgroup = sheetgrp.GroupBy(r => r["AcctClass"].ToString()).Select(grp => grp.ToList());


                foreach (var acctclass in acctclassgroup)
                {
                    currentsheet.Cells[curRow, 2].Value = acctclass[0]["AcctClass"].ToString().ToUpper();
                    currentsheet.Cells[curRow, 2].Style.Font.Bold = true;
                    currentsheet.Cells[curRow, 2].Style.Font.Size = 18;
                    currentsheet.Cells[curRow, 2].Style.Font.UnderLine = true;

                    curRow = curRow +2;

                    var acctsubclassgroup = acctclass.GroupBy(r => r["AcctSubClass"].ToString()).Select(grp => grp.ToList());

                    foreach(var acctsubclass in acctsubclassgroup)
                    {

                        currentsheet.Cells[curRow, 2].Value = acctsubclass[0]["AcctSubClass"].ToString().ToUpper();
                        currentsheet.Cells[curRow, 2].Style.Font.Bold = true;
                        currentsheet.Cells[curRow, 2].Style.Font.Size = 16;
                        currentsheet.Cells[curRow, 2].Style.Font.UnderLine = false;

                        curRow = curRow + 1;

                        var acctSubsubclassgroup = acctsubclass.GroupBy(r => r["AcctSubsubClass"].ToString()).Select(grp => grp.ToList());


                        foreach (var acctsubsubclass in acctSubsubclassgroup)
                        {

                            currentsheet.Cells[curRow, 2].Value = acctsubsubclass[0]["AcctSubsubClass"].ToString().ToUpper();
                            currentsheet.Cells[curRow, 2].Style.Font.Bold = true;
                            currentsheet.Cells[curRow, 2].Style.Font.Size = 14;
                            currentsheet.Cells[curRow, 2].Style.Font.UnderLine = false;

                            curRow = curRow + 1;


                            foreach(DataRow row in acctsubsubclass)
                            {

                                var desc = row["Item_Description"].ToString();
                                bool isSummary = row["SummaryMarker"].ToString().CleanUp() == "summaryitem";

                                string title = ""; decimal amt=decimal.Zero;


                                if (desc.Contains("$"))
                                {
                                    var key = desc.Split('$')[0];
                                    var value = desc.Split('$')[1];

                                    IEnumerable<DataRow> valid; 
                                     title = row["Totals"].ToString().ToUpper().Trim();

                                    switch (key.CleanUp())
                                    {
                                        case "sub-total":
                                            currentsheet.Cells[curRow, 2].Value = key;
                                            currentsheet.Cells[curRow, 2].Style.Font.Bold = true;
                                            currentsheet.Cells[curRow, 2].Style.Font.UnderLine = false;

                                            valid = dt.AsEnumerable().Where(r => r["Totals"].ToString().Trim() == value.Trim()&&!r["Item_Description"].ToString().Contains("$"));

                                            amt = valid.Sum(r => r["Amount"].toDecimal());

                                            currentsheet.Cells[curRow, 3].Value = amt;
                                            currentsheet.Cells[curRow, 3].Style.Font.Bold = true;
                                            currentsheet.Cells[curRow, 3].Style.Numberformat.Format = "#,##0.00";
                                            break;

                                        case "grandtotal":

                                            currentsheet.Cells[curRow, 2].Value = title;
                                            currentsheet.Cells[curRow, 2].Style.Font.Bold = true;
                                            currentsheet.Cells[curRow, 2].Style.Font.Size = 12;
                                            currentsheet.Cells[curRow, 2].Style.Font.UnderLine = false;

                                            valid = dt.AsEnumerable().Where(r => r["AcctSubClass"].ToString().Trim() == value.Trim() && !r["Item_Description"].ToString().Contains("$"));

                                            amt = valid.Sum(r => r["Amount"].toDecimal());

                                            currentsheet.Cells[curRow, 3].Value = amt;
                                            currentsheet.Cells[curRow, 3].Style.Font.Bold = true;
                                            currentsheet.Cells[curRow, 2].Style.Font.Size = 12;
                                            currentsheet.Cells[curRow, 3].Style.Numberformat.Format = "#,##0.00";



                                            break;


                                        case "customsum":

                                            currentsheet.Cells[curRow, 2].Value = title;
                                            currentsheet.Cells[curRow, 2].Style.Font.Bold = true;
                                            currentsheet.Cells[curRow, 2].Style.Font.Size = 12;
                                            currentsheet.Cells[curRow, 2].Style.Font.UnderLine = false;

                                            valid = dt.AsEnumerable().Where(r => r["CustomTotals"].ToString().Trim() == value.Trim() && !r["Item_Description"].ToString().Contains("$"));

                                            amt = valid.Sum(r => r["Amount"].toDecimal());

                                            currentsheet.Cells[curRow, 3].Value = amt;
                                            currentsheet.Cells[curRow, 3].Style.Font.Bold = true;
                                            currentsheet.Cells[curRow, 2].Style.Font.Size = 12;
                                            currentsheet.Cells[curRow, 3].Style.Numberformat.Format = "#,##0.00";



                                            break;

                                        case "categsum":

                                            currentsheet.Cells[curRow, 2].Value = title;
                                            currentsheet.Cells[curRow, 2].Style.Font.Bold = true;
                                            currentsheet.Cells[curRow, 2].Style.Font.Size = 12;
                                            currentsheet.Cells[curRow, 2].Style.Font.UnderLine = false;

                                            valid = dt.AsEnumerable().Where(r => r["CategTotals"].ToString().Trim() == value.Trim() && !r["Item_Description"].ToString().Contains("$"));

                                            amt = valid.Sum(r => r["Amount"].toDecimal());

                                            currentsheet.Cells[curRow, 3].Value = amt;
                                            currentsheet.Cells[curRow, 3].Style.Font.Bold = true;
                                            currentsheet.Cells[curRow, 2].Style.Font.Size = 12;
                                            currentsheet.Cells[curRow, 3].Style.Numberformat.Format = "#,##0.00";



                                            break;

                                        case "globalsum":

                                            currentsheet.Cells[curRow, 2].Value = title;
                                            currentsheet.Cells[curRow, 2].Style.Font.Bold = true;
                                            currentsheet.Cells[curRow, 2].Style.Font.Size = 12;
                                            currentsheet.Cells[curRow, 2].Style.Font.UnderLine = false;

                                            valid = dt.AsEnumerable().Where(r => r["GlobalSum"].ToString().Trim() == value.Trim() && !r["Item_Description"].ToString().Contains("$"));

                                            amt = valid.Sum(r => r["Amount"].toDecimal());

                                            currentsheet.Cells[curRow, 3].Value = amt;
                                            currentsheet.Cells[curRow, 3].Style.Font.Bold = true;
                                            currentsheet.Cells[curRow, 2].Style.Font.Size = 12;
                                            currentsheet.Cells[curRow, 3].Style.Numberformat.Format = "#,##0.00";



                                            break;

                                        case "netsum":

                                            currentsheet.Cells[curRow, 2].Value = title;
                                            currentsheet.Cells[curRow, 2].Style.Font.Bold = true;
                                            currentsheet.Cells[curRow, 2].Style.Font.Size = 12;
                                            currentsheet.Cells[curRow, 2].Style.Font.UnderLine = false;

                                            valid = dt.AsEnumerable().Where(r => r["NetSum"].ToString().Trim() == value.Trim() && !r["Item_Description"].ToString().Contains("$"));

                                            amt = valid.Sum(r => r["Amount"].toDecimal());

                                            currentsheet.Cells[curRow, 3].Value = amt;
                                            currentsheet.Cells[curRow, 3].Style.Font.Bold = true;
                                            currentsheet.Cells[curRow, 2].Style.Font.Size = 12;
                                            currentsheet.Cells[curRow, 3].Style.Numberformat.Format = "#,##0.00";



                                            break;


                                    }

                                }
                                else
                                {
                                    title = row["Item_Description"].ToString();
                                    amt = row["Amount"].toDecimal();
                                    currentsheet.Cells[curRow, 2].Value = row["Item_Description"].ToString();
                                    currentsheet.Cells[curRow, 2].Style.Font.Bold = false;
                                    currentsheet.Cells[curRow, 2].Style.Font.UnderLine = false;


                                    currentsheet.Cells[curRow, 3].Value = row["Amount"].toDecimal();
                                    currentsheet.Cells[curRow, 3].Style.Font.Bold = false;
                                    currentsheet.Cells[curRow, 3].Style.Numberformat.Format = "#,##0.00";
                                }


                                if (isSummary)
                                {
                                    var srow = summarydata.NewRow();

                                    srow["Item_Description"] = title;
                                    srow["Reference"] = referenceid;
                                    srow["Amount"] = amt;
                                    srow["Reporting_Month"] = DateTime.Now.ToString("MMM yyyy");
                                    srow["UniqueReference"] = srow["Item_Description"].ToString() + srow["Reporting_Month"].ToString();

                                    summarydata.Rows.Add(srow);
                                }

                                curRow = curRow + 1;
                            }

                            curRow = curRow + 1;
                        }
                        curRow = curRow + 1;
                    }

                    curRow = curRow + 2;

                }

            }



            db.CopyDataTableToDB(summarydata, "summarytemp");

            currentsheet = xlPackage.Workbook.Worksheets.Add("SUMMARY");

            curRow = 2;

            sql = "  select a.*, (select v.Amount from (select * from summarytemp where reference ='"+referenceid+"') v where ltrim(rtrim(a.SummaryMap))= ltrim(rtrim(Item_Description))) as Amount  from [dbo].[NPF_MA_SUMMARY_PARAMS] a ";

            dt = db.getData(sql);

           var summclassgroup = dt.AsEnumerable().GroupBy(r => r["AcctClass"].ToString()).Select(grp => grp.ToList());


            foreach (var acctclass in summclassgroup)
            {
                currentsheet.Cells[curRow, 2].Value = acctclass[0]["AcctClass"].ToString().ToUpper();
                currentsheet.Cells[curRow, 2].Style.Font.Bold = true;
                currentsheet.Cells[curRow, 2].Style.Font.Size = 18;
                currentsheet.Cells[curRow, 2].Style.Font.UnderLine = true;

                curRow = curRow + 2;



                        foreach (DataRow row in acctclass)
                        {

                            var desc = row["Item_Description"].ToString();
                      

                            string title = ""; decimal amt = decimal.Zero;


                            if (desc.Contains("$"))
                            {
                                var key = desc.Split('$')[0];
                                var value = desc.Split('$')[1];

                                IEnumerable<DataRow> valid;
                                title = row["TotalDesc"].ToString().ToUpper().Trim();

                                switch (key.CleanUp())
                                {
                                    case "sub-total":
                                        currentsheet.Cells[curRow, 2].Value = title;
                                        currentsheet.Cells[curRow, 2].Style.Font.Bold = true;
                                currentsheet.Cells[curRow, 2].Style.Font.Size = 12;
                                currentsheet.Cells[curRow, 2].Style.Font.UnderLine = false;

                                        valid = dt.AsEnumerable().Where(r => r["SubTotals"].ToString().Trim() == value.Trim() && !r["Item_Description"].ToString().Contains("$"));

                                        amt = valid.Sum(r => r["Amount"].toDecimal());

                                        currentsheet.Cells[curRow, 3].Value = amt;
                                        currentsheet.Cells[curRow, 3].Style.Font.Bold = true;
                                        currentsheet.Cells[curRow, 3].Style.Numberformat.Format = "#,##0.00";
                                        break;

                                    case "grandtotal":

                                        currentsheet.Cells[curRow, 2].Value = title;
                                        currentsheet.Cells[curRow, 2].Style.Font.Bold = true;
                                        currentsheet.Cells[curRow, 2].Style.Font.Size = 14;
                                        currentsheet.Cells[curRow, 2].Style.Font.UnderLine = false;

                                        valid = dt.AsEnumerable().Where(r => r["AcctClass"].ToString().Trim() == value.Trim() && !r["Item_Description"].ToString().Contains("$"));

                                        amt = valid.Sum(r => r["Amount"].toDecimal());

                                        currentsheet.Cells[curRow, 3].Value = amt;
                                        currentsheet.Cells[curRow, 3].Style.Font.Bold = true;
                                        currentsheet.Cells[curRow, 2].Style.Font.Size = 14;
                                        currentsheet.Cells[curRow, 3].Style.Numberformat.Format = "#,##0.00";



                                        break;

                            case "customsum":

                                currentsheet.Cells[curRow, 2].Value = title;
                                currentsheet.Cells[curRow, 2].Style.Font.Bold = true;
                                currentsheet.Cells[curRow, 2].Style.Font.Size = 16;
                                currentsheet.Cells[curRow, 2].Style.Font.UnderLine = false;

                                valid = dt.AsEnumerable().Where(r => r["CustomSum"].ToString().Trim() == value.Trim() && !r["Item_Description"].ToString().Contains("$"));

                                amt = valid.Sum(r => r["Amount"].toDecimal());

                                currentsheet.Cells[curRow, 3].Value = amt;
                                currentsheet.Cells[curRow, 3].Style.Font.Bold = true;
                                currentsheet.Cells[curRow, 2].Style.Font.Size = 16;
                                currentsheet.Cells[curRow, 3].Style.Numberformat.Format = "#,##0.00";



                                break;



                        }

                            }
                            else
                            {
                                title = row["Item_Description"].ToString();
                                amt = row["Amount"].toDecimal();
                                currentsheet.Cells[curRow, 2].Value = row["Item_Description"].ToString();
                                currentsheet.Cells[curRow, 2].Style.Font.Bold = false;
                                currentsheet.Cells[curRow, 2].Style.Font.UnderLine = false;


                                currentsheet.Cells[curRow, 3].Value = row["Amount"].toDecimal();
                                currentsheet.Cells[curRow, 3].Style.Font.Bold = false;
                                currentsheet.Cells[curRow, 3].Style.Numberformat.Format = "#,##0.00";
                            }



                            curRow = curRow + 1;
                        

                        curRow = curRow + 1;
                    }
      

                curRow = curRow + 2;

            }

            var details = new MergeDetails();

            details.destDB = "inlaksbiwarehouse";
            details.sourceDB = "inlaksbiwarehouse";
            details.sourcereference = "UniqueReference";
            details.destreference = "UniqueReference";
            details.destTb = "factnpfmasummary";
            details.sourceTb = "summarytemp";

            var count = new SQLServerDBInterfac(new Settings().warehousedb).MergeData(details);

            db.Execute("truncate table summarytemp");


        xlPackage.Save();

            return downpath;
        }


        public string generateTrialBalanceExtract( string downpath, string reportname)
        {

            //DateTime start = DateTime.ParseExact(startdate, "yyyy-MM-dd", new System.Globalization.CultureInfo("en-US"));
           // DateTime end = DateTime.ParseExact(enddate, "yyyy-MM-dd", new System.Globalization.CultureInfo("en-US"));
            var sql = "select GLId, REPORTING_LINE,LINE_DESCRIPTION,BranchName, sum(GLAmount) as Amount from gl_line_branch where REPORTING_LINE is not null and  GLAmount is not null and Currency is not null group by REPORTING_LINE,LINE_DESCRIPTION,BranchName,GLId  order by REPORTING_LINE ";

            var db = new InlaksBIContext().getDBInterface( new Settings().warehousedbtype, new Settings().warehousedb);

            var dt1 = db.getData(sql);

            sql = "select GLId, REPORTING_LINE,LINE_DESCRIPTION,sum(Amount) as Amount from gl_line_consolidated where REPORTING_LINE is not null and  Amount is not null group by REPORTING_LINE,LINE_DESCRIPTION,GLId  order by REPORTING_LINE ";

            var dt2 = db.getData(sql);

            if (dt1.Rows.Count == 0)
            {
                throw new Exception("No records found!");
            }

            downpath = downpath + DateTime.Now.ToString("dd-MMM-yyyy") + reportname + ".xlsx";


            ExcelWorksheet consolidated;
            ExcelWorksheet bybranch;
            ExcelPackage xlPackage;


            FileInfo newFile = new FileInfo(downpath);

            if (newFile.Exists)
            {
                newFile.Delete();
            }
            xlPackage = new ExcelPackage(newFile);


            consolidated = xlPackage.Workbook.Worksheets.Add("Consolidated");
            bybranch = xlPackage.Workbook.Worksheets.Add("By Branch");

            int curRow = 3;


            //var headers = new string[] { "EntryID", "AccountID", "OldAccountNo", "BranchId", "Amount", "CustomerId",  "CustomerName", "Telephone", "CustomerAddress", "ValueDate", "Currency", "OurReference", "TransactionReference", "PostDate", "Inputer", "Authoriser", "CrDr", "Time", "BranchName", "BranchAddress", "Narrative", "Birthdate" };


            //dt = new DataView(dt).ToTable(false, headers);


            var branchgroups = dt1.AsEnumerable().GroupBy(r => r["BranchName"].ToString()).Select(grp => grp.ToList());

            var allgroups = dt2.AsEnumerable().GroupBy(r => r["REPORTING_LINE"].ToString()).Select(grp => grp.ToList());


            consolidated.Cells[curRow, 2].Value = "NPF MICROFINANCE BANK PLC";
            consolidated.Cells[curRow, 2].Style.Font.Bold = true;
            consolidated.Cells[curRow, 2].Style.Font.UnderLine = false;
            //consolidated.Cells[3, 1, 3, 13].Merge = true;

            curRow = curRow + 1;

            //consolidated.Cells[curRow, 2].Value = branchdata[0]["BranchName"].ToString();
            //consolidated.Cells[curRow, 2].Style.Font.Bold = true;
            //consolidated.Cells[curRow, 2].Style.Font.UnderLine = false;

            //curRow = curRow + 1;

            //consolidated.Cells[curRow, 2].Value = branchdata[0]["BranchAddress"].ToString();
            //consolidated.Cells[curRow, 2].Style.Font.Bold = true;
            //consolidated.Cells[curRow, 2].Style.Font.UnderLine = false;

          //  curRow = curRow + 1;

            consolidated.Cells[curRow, 2].Value = reportname+" (Consolidated)";
            consolidated.Cells[curRow, 2].Style.Font.Bold = true;
            consolidated.Cells[curRow, 2].Style.Font.UnderLine = false;

            curRow = curRow + 2;


        

            foreach (var reportingline in allgroups)
            {

                consolidated.Cells[curRow, 2].Value = reportingline[0]["REPORTING_LINE"].ToString();
                consolidated.Cells[curRow, 2].Style.Font.Bold = true;
                consolidated.Cells[curRow, 2].Style.Font.UnderLine = true;


                consolidated.Cells[curRow, 3].Value = reportingline.Sum(r => r["Amount"].toDecimal());
                consolidated.Cells[curRow, 3].Style.Font.Bold = true;
                consolidated.Cells[curRow, 3].Style.Numberformat.Format = "#,##0.00";

                curRow = curRow + 2;

                var headers = new string[] {  "S/N", "GLId", "Item Description", "Amount", };

                var Column = new string[] { "SN", "GLId", "Item Description", "Amount" };


                var dest = new DataTable();

                dest.AddTableColumns(headers);

                var descgroup = reportingline.GroupBy(r => r["LINE_DESCRIPTION"]).Select(grp => grp.ToList());


                foreach (var desc in descgroup)
                {
                    var row = dest.NewRow();

                    row["GLId"] = desc[0]["GLId"];
                    row["Item Description"] = desc[0]["LINE_DESCRIPTION"].ToString();
                    row["Amount"] = desc.Sum(r => r["Amount"].toDecimal());

                    dest.Rows.Add(row);
                }


                System.Drawing.Color green = new System.Drawing.Color();
                for (int i = 0; i < headers.Length; i++)
                {
                    consolidated.Cells[curRow, i + 1].Value = headers[i];
                    consolidated.Cells[curRow, i + 1].Style.Font.Bold = true;
                    consolidated.Cells[curRow, i + 1].Style.Font.UnderLine = true;
                    consolidated.Cells[curRow, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.LightGray;
                    consolidated.Cells[curRow, i + 1].Style.Fill.BackgroundColor.SetColor(green);
                }

                curRow = curRow + 1;

                for (int i = 0; i < headers.Length; i++)
                {

                    consolidated.Cells[curRow, i + 1].Value = "";
                }

                if (dt2.Rows.Count != 0)
                {
                    int count = 0;

                    foreach (var drow in dest.AsEnumerable().OrderBy(r => r["GLId"]))
                    {
                        count = count + 1;
                        //throw new Exception("No records found!");
                        for (int i = 0; i < headers.Length; i++)
                        {
                            if (Column[i].isNull())
                            {
                                Column[i] = "";
                            }

                            if (Column[i] != "")
                            {
                                if (Column[i].Trim() == "SN")
                                {
                                    consolidated.Cells[curRow, i + 1].Value = count;
                                }
                                else
                                {


                                    consolidated.Cells[curRow, i + 1].Value = drow[Column[i]].ToString();

                                    if (Column[i].Trim() == "Amount")
                                    {
                                        consolidated.Cells[curRow, i + 1].Value = drow[Column[i]].toDecimal();
                                        consolidated.Cells[curRow, i + 1].Style.Numberformat.Format = "#,##0.00";
                                    }

                                }




                            }

                            else

                            {

                                consolidated.Cells[curRow, i + 1].Value = "";

                            }
                        }

                        curRow = curRow + 1;

                    }
                }


                curRow = curRow + 4;

            }



            curRow = 3;

            foreach (var branchdata in branchgroups)
            {


                bybranch.Cells[curRow, 2].Value = "NPF MICROFINANCE BANK PLC";
                bybranch.Cells[curRow, 2].Style.Font.Bold = true;
                bybranch.Cells[curRow, 2].Style.Font.UnderLine = false;
                //bybranch.Cells[3, 1, 3, 13].Merge = true;

                curRow = curRow + 1;

                bybranch.Cells[curRow, 2].Value = branchdata[0]["BranchName"].ToString();
                bybranch.Cells[curRow, 2].Style.Font.Bold = true;
                bybranch.Cells[curRow, 2].Style.Font.UnderLine = false;

                //curRow = curRow + 1;

                //bybranch.Cells[curRow, 2].Value = branchdata[0]["BranchAddress"].ToString();
                //bybranch.Cells[curRow, 2].Style.Font.Bold = true;
                //bybranch.Cells[curRow, 2].Style.Font.UnderLine = false;

                curRow = curRow + 1;

                bybranch.Cells[curRow, 2].Value = reportname;
                bybranch.Cells[curRow, 2].Style.Font.Bold = true;
                bybranch.Cells[curRow, 2].Style.Font.UnderLine = false;

                curRow = curRow + 2;


                var reportinglinegrp = branchdata.GroupBy(r => r["REPORTING_LINE"]).Select(grp => grp.ToList());    

                foreach (var reportingline in reportinglinegrp) {

                    bybranch.Cells[curRow, 2].Value = reportingline[0]["REPORTING_LINE"].ToString();
                    bybranch.Cells[curRow, 2].Style.Font.Bold = true;
                    bybranch.Cells[curRow, 2].Style.Font.UnderLine = true;


                    bybranch.Cells[curRow, 3].Value = reportingline.Sum(r => r["Amount"].toDecimal());
                    bybranch.Cells[curRow, 3].Style.Font.Bold = true;
                    bybranch.Cells[curRow, 3].Style.Numberformat.Format = "#,##0.00";

                    curRow = curRow +2;

                    var headers = new string[] { "SN", "GLId", "Item Description", "Amount" };

                    var Column = new string[] { "SN", "GLId", "Item Description", "Amount" };


                    var dest = new DataTable();

                    dest.AddTableColumns(headers);

                    var descgroup = reportingline.GroupBy(r => r["LINE_DESCRIPTION"]).Select(grp => grp.ToList());


                    foreach (var desc in descgroup)
                    {
                        var row = dest.NewRow();

                        row["GLId"] = desc[0]["GLId"];
                        row["Item Description"] = desc[0]["LINE_DESCRIPTION"].ToString();
                        row["Amount"] = desc.Sum(r => r["Amount"].toDecimal());

                        dest.Rows.Add(row);
                    }


                    System.Drawing.Color green = new System.Drawing.Color();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        bybranch.Cells[curRow, i + 1].Value = headers[i];
                        bybranch.Cells[curRow, i + 1].Style.Font.Bold = true;
                        bybranch.Cells[curRow, i + 1].Style.Font.UnderLine = true;
                        bybranch.Cells[curRow, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.LightGray;
                        bybranch.Cells[curRow, i + 1].Style.Fill.BackgroundColor.SetColor(green);
                    }

                    curRow = curRow + 1;

                    for (int i = 0; i < headers.Length; i++)
                    {

                        bybranch.Cells[curRow, i + 1].Value = "";
                    }

                    if (dt1.Rows.Count != 0)
                    {
                        int count = 0;

                        foreach (var drow in dest.AsEnumerable().OrderBy(r => r["GLId"]))
                        {
                            count = count + 1;
                            //throw new Exception("No records found!");
                            for (int i = 0; i < headers.Length; i++)
                            {
                                if (Column[i].isNull())
                                {
                                    Column[i] = "";
                                }

                                if (Column[i] != "")
                                {
                                    if (Column[i].Trim() == "SN")
                                    {
                                        bybranch.Cells[curRow, i + 1].Value = count;
                                    }
                                    else
                                    {


                                        bybranch.Cells[curRow, i + 1].Value = drow[Column[i]].ToString();

                                        if (Column[i].Trim() == "Amount")
                                        {
                                            bybranch.Cells[curRow, i + 1].Value = drow[Column[i]].toDecimal();
                                            bybranch.Cells[curRow, i + 1].Style.Numberformat.Format = "#,##0.00";
                                        }

                                    }




                                }

                                else

                                {

                                    bybranch.Cells[curRow, i + 1].Value = "";

                                }
                            }

                            curRow = curRow + 1;

                        }
                    }


                    curRow = curRow + 4;

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



        public string generateNPFUniformConsumerFormatReport(string downpath, string reportname)
        {

            string today = DateTime.Now.ToString("dd-MM-yyyy");

            downpath = downpath + today + "_" + reportname + ".xlsx";

            ExcelWorksheet oSheet;
            ExcelWorksheet CreditInformationSheet;
            ExcelPackage xlPackage;

            FileInfo newFile = new FileInfo(downpath);

            if (newFile.Exists)
            {
                newFile.Delete();
            }

            xlPackage = new ExcelPackage(newFile);

            oSheet = xlPackage.Workbook.Worksheets.Add("Individual Borrower sheet");

            CreditInformationSheet = xlPackage.Workbook.Worksheets.Add("Credit Information Sheet");

            int curRow = 1;

            DataTable PrintTable = new DataTable();



            //implement individual browser method

            var sql = "select * from individual_loan_customers";

            var db = new InlaksBIContext().getDBInterface(new Settings().warehousedbtype, new Settings().warehousedb);

            var dt = db.getData(sql);

            //if (dt.Rows.Count == 0)
            //{
            //    throw new Exception("No records found!");
            //}

            var headers = new string[] { "CustomerID", "Branch Code", "Surname", "First Name", "Middle Name", "Date of Birth", "Natinonal Identity Number", "Drivers License No", "BVN No", "Passport NO", "Gender", "Nationality", "Marital Status", "Mobile Number", "Primary Address Line 1", "Primary Address Line 2", "Primary city/LGA", "Primary State", "Primary Country", "Employment Status", "Occupation", "Business Category", "Business Sector", "Borrower Type", "Other ID", "Tax ID", "Picture File", "E-mail Address", "Employer name", "Employer Address Line 1", "Employer Address Line 2", "Employer City", "Employer State", "Employer Country", "Title", "Place of Birth", "Work Phone", "Home Phone", "Secondary Address Line 1", "Secondary Address Line 2", "Secondary Address City/LGA", "Secondary Address State", "Secondary Address Country", "Spouse's Surname", "Spouse's First Name", "Spouse's Middle Name" };

            var Column = new string[] { "CustomerID", "CO_CODE", "LastName", "FirstName", "MiddleName", "BirthDate", "NationalIdentityNum", "", "BVN", "", "Gender", "", "MaritalStatus", "DefaultPhone", "Address", "", "", "", "", "IsEmployee", "Occupation", "", "", "CustomerType", "", "", "", "EmailAddress", "", "", "", "", "", "", "AccountTitle", "", "", "", "", "", "", "", "", "", "", "" };

            System.Drawing.Color green = new System.Drawing.Color();
            for (int i = 0; i < headers.Length; i++)
            {
                oSheet.Cells[curRow, i + 1].Value = headers[i];
                oSheet.Cells[curRow, i + 1].Style.Font.Bold = true;
                oSheet.Cells[curRow, i + 1].Style.Font.UnderLine = true;
                oSheet.Cells[curRow, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.LightGray;
                oSheet.Cells[curRow, i + 1].Style.Fill.BackgroundColor.SetColor(green);
            }

            curRow = curRow + 1;

            for (int i = 0; i < headers.Length; i++)
            {

                oSheet.Cells[curRow, i + 1].Value = "";
            }

            if (dt.Rows.Count != 0)
            {

                foreach (var drow in dt.AsEnumerable())
                {

                    //throw new Exception("No records found!");
                    for (int i = 0; i < headers.Length; i++)
                    {
                        if (Column[i].isNull())
                        {
                            Column[i] = "";
                        }

                        if (Column[i] != "")
                        {

                            oSheet.Cells[curRow, i + 1].Value = drow[Column[i]].ToString();

                        }

                        else

                        {

                            oSheet.Cells[curRow, i + 1].Value = "";

                        }
                    }

                    curRow = curRow + 1;

                }
            }


            //implement Credit Information method for individual


            curRow = 1;

            sql = "select a.*,b.RATE from loansummary a, parsummary b where a.ArrangementId = b.LOANID and ((ltrim(rtrim(a.CustomerType)) != 'NI' or a.CustomerType is null))";

            db = new InlaksBIContext().getDBInterface(new Settings().warehousedbtype, new Settings().warehousedb);

            dt = db.getData(sql);

            headers = new string[] { "CustomerID", "Account Number", "Account Status", "Account Status date", "Date of loan (facility) Disbursement/loan effective date", "Credit limit/Facility amount/Global limit", "Loan (facility) Amount / Availed Limit", "Outstanding Balance", "Instalment amount","Interest Rate (%)", "Currency", "Days in arrears", "Overdue amount", "Loan (Facility) type", "Loan (Facility) Tenor", "Repayment frequency", "Last payment date", "Last Payment amount", "Maturity", "Loan Classification", "Legal Challenge Status", "Litigation Date", "Consent Status", "Loan Security Status", "Collateral Type", "Previous Account Number", "Previous Name", "Previous Customer ID", "Previous Branch Code" };

            Column = new string[] { "CustomerID", "AccountId", "Overdue_Status", "", "ProdEffDate", "InternalAmount", "OpenBalance1", "OutstandingBalance", "Amount","RATE", "Currency", "Total_Days", "Average_Amt", "", "", "PaymentFrequency", "DateLastDrAuto", "AmntLastDrAuto", "MaturityDate", "", "", "", "", "", "", "Alt_AcctId", "", "", "" };

            for (int i = 0; i < headers.Length; i++)
            {
                CreditInformationSheet.Cells[curRow, i + 1].Value = headers[i];
                CreditInformationSheet.Cells[curRow, i + 1].Style.Font.Bold = true;
                CreditInformationSheet.Cells[curRow, i + 1].Style.Font.UnderLine = true;
                CreditInformationSheet.Cells[curRow, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.LightGray;
                CreditInformationSheet.Cells[curRow, i + 1].Style.Fill.BackgroundColor.SetColor(green);
            }

            curRow = curRow + 1;

            for (int i = 0; i < headers.Length; i++)
            {
                CreditInformationSheet.Cells[curRow, i + 1].Value = "";
            }

            if (dt.Rows.Count != 0)
            {

                //throw new Exception("No records found!");

                foreach (var drow in dt.AsEnumerable())
                {

                    for (int i = 0; i < headers.Length; i++)
                    {
                        if (Column[i].isNull())
                        {
                            Column[i] = "";
                        }

                        if (Column[i] != "")
                        {

                            CreditInformationSheet.Cells[curRow, i + 1].Value = drow[Column[i]].ToString();

                        }

                        else

                        {

                            CreditInformationSheet.Cells[curRow, i + 1].Value = "";

                        }
                    }

                    curRow = curRow + 1;
                }

            }

            xlPackage.Save();

            return downpath;
        }


        public string generateNPFUniformCorporateFormatReport(string downpath, string reportname)
        {

            string today = DateTime.Now.ToString("dd-MM-yyyy");

            downpath = downpath + today + "_" + reportname + ".xlsx";

            ExcelWorksheet oSheet;
            ExcelWorksheet CreditInformationSheet;
            ExcelPackage xlPackage;

            FileInfo newFile = new FileInfo(downpath);

            if (newFile.Exists)
            {
                newFile.Delete();
            }

            xlPackage = new ExcelPackage(newFile);

            oSheet = xlPackage.Workbook.Worksheets.Add("Corporate Borrower Sheet");

            CreditInformationSheet = xlPackage.Workbook.Worksheets.Add("Credit Information Sheet");

            int curRow = 1;

            DataTable PrintTable = new DataTable();



            //implement Corporate browser method

            var sql = "select * from corporate_loan_customers";
            var db = new InlaksBIContext().getDBInterface(new Settings().warehousedbtype, new Settings().warehousedb);

            var dt = db.getData(sql);

            //if (dt.Rows.Count == 0)
            //{
            //    throw new Exception("No records found!");
            //}

            var headers = new string[] { "Business Identification Number", "Business Name", "Business Corporate Type", "Business Category", "Date of Incorporation", "Customer ID", "Customer's Branch Code", "Business Office Address Line 1", "Business Office Address Line 2", "City/LGA", "State", "Country", "Email Address", "Secondary Address Line 1", "Secondary Address Line 2", "City/LGA", "State", "Country", "Tax ID", "Phone Number" };

            var Column = new string[] { "", "FirstName", "", "", "", "CustomerID", "CO_CODE", "Address", "", "", "", "", "EmailAddress", "Address", "", "", "", "", "", "DefaultPhone" };

            System.Drawing.Color green = new System.Drawing.Color();
            for (int i = 0; i < headers.Length; i++)
            {
                oSheet.Cells[curRow, i + 1].Value = headers[i];
                oSheet.Cells[curRow, i + 1].Style.Font.Bold = true;
                oSheet.Cells[curRow, i + 1].Style.Font.UnderLine = true;
                oSheet.Cells[curRow, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.LightGray;
                oSheet.Cells[curRow, i + 1].Style.Fill.BackgroundColor.SetColor(green);
            }

            curRow = curRow + 1;

            for (int i = 0; i < headers.Length; i++)
            {

                oSheet.Cells[curRow, i + 1].Value = "";
            }

            if (dt.Rows.Count != 0)
            {

                foreach (var drow in dt.AsEnumerable())
                {

                    //throw new Exception("No records found!");
                    for (int i = 0; i < headers.Length; i++)
                    {
                        if (Column[i].isNull())
                        {
                            Column[i] = "";
                        }

                        if (Column[i] != "")
                        {

                            oSheet.Cells[curRow, i + 1].Value = drow[Column[i]].ToString();

                        }

                        else

                        {

                            oSheet.Cells[curRow, i + 1].Value = "";

                        }
                    }

                    curRow = curRow + 1;

                }
            }



            //implement Credit Information method for corporate


            curRow = 1;

            sql = "select a.*,b.RATE from loansummary a, parsummary b where a.ArrangementId = b.LOANID and ltrim(rtrim(CustomerType)) = 'NI'";

            db = new InlaksBIContext().getDBInterface(new Settings().warehousedbtype, new Settings().warehousedb);

            dt = db.getData(sql);

            headers = new string[] { "CustomerID", "Account Number", "Account Status", "Account Status date", "Date of loan (facility) Disbursement/loan effective date", "Credit limit/Facility amount/Global limit", "Loan (facility) Amount / Availed Limit", "Outstanding Balance", "Instalment amount", "Interest Rate (%)", "Currency", "Days in arrears", "Overdue amount", "Loan (Facility) type", "Loan (Facility) Tenor", "Repayment frequency", "Last payment date", "Last Payment amount", "Maturity", "Loan Classification", "Legal Challenge Status", "Litigation Date", "Consent Status", "Loan Security Status", "Collateral Type", "Previous Account Number", "Previous Name", "Previous Customer ID", "Previous Branch Code" };

            Column = new string[] { "CustomerID", "AccountId", "Overdue_Status", "", "ProdEffDate", "InternalAmount", "OpenBalance1", "OutstandingBalance", "Amount", "RATE", "Currency", "Total_Days", "Average_Amt", "", "", "PaymentFrequency", "DateLastDrAuto", "AmntLastDrAuto", "MaturityDate", "", "", "", "", "", "", "Alt_AcctId", "", "", "" };


            for (int i = 0; i < headers.Length; i++)
            {
                CreditInformationSheet.Cells[curRow, i + 1].Value = headers[i];
                CreditInformationSheet.Cells[curRow, i + 1].Style.Font.Bold = true;
                CreditInformationSheet.Cells[curRow, i + 1].Style.Font.UnderLine = true;
                CreditInformationSheet.Cells[curRow, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.LightGray;
                CreditInformationSheet.Cells[curRow, i + 1].Style.Fill.BackgroundColor.SetColor(green);
            }

            curRow = curRow + 1;

            for (int i = 0; i < headers.Length; i++)
            {
                CreditInformationSheet.Cells[curRow, i + 1].Value = "";
            }

            if (dt.Rows.Count != 0)
            {

                //throw new Exception("No records found!");

                foreach (var drow in dt.AsEnumerable())
                {

                    for (int i = 0; i < headers.Length; i++)
                    {
                        if (Column[i].isNull())
                        {
                            Column[i] = "";
                        }

                        if (Column[i] != "")
                        {

                            CreditInformationSheet.Cells[curRow, i + 1].Value = drow[Column[i]].ToString();

                        }

                        else

                        {

                            CreditInformationSheet.Cells[curRow, i + 1].Value = "";

                        }
                    }

                    curRow = curRow + 1;
                }

            }

            xlPackage.Save();

            return downpath;
        }



         public string generateNPFDisbursementReport(string downpath, string reportname,string StartDate,string EndDate)
        {

            string today = DateTime.Now.ToString("dd-MM-yyyy");

            downpath = downpath + today + "_" + reportname + ".xlsx";

            ExcelWorksheet oSheet;
            //ExcelWorksheet CreditInformationSheet;
            ExcelPackage xlPackage;

            FileInfo newFile = new FileInfo(downpath);

            if (newFile.Exists)
            {
                newFile.Delete();
            }

            xlPackage = new ExcelPackage(newFile);

            oSheet = xlPackage.Workbook.Worksheets.Add("Disbursment Report Sheet");

           // CreditInformationSheet = xlPackage.Workbook.Worksheets.Add("Credit Information Sheet");

            int curRow = 1;

            DataTable PrintTable = new DataTable();

            var sd = DateTime.ParseExact(StartDate,"yyyy-MM-dd", new CultureInfo("en-Us")).ToString("yyyyMMdd");

            var ed = DateTime.ParseExact(EndDate, "yyyy-MM-dd", new CultureInfo("en-Us")).ToString("yyyyMMdd");


            //implement Corporate browser method

            var sql = "select a.*,b.* from loansummary a join DimCustomer b on (a.CustomerId=b.CustomerId)  where a.StartDate between '" + sd+"' and '"+ed+"'";

            var db = new InlaksBIContext().getDBInterface(new Settings().warehousedbtype, new Settings().warehousedb);

            var dt = db.getData(sql);

            //if (dt.Rows.Count == 0)
            //{
            //    throw new Exception("No records found!");
            //}
            //////////
            
            var headers = new string[] { "CustomerID", "Account Number", "Account Status", "Account Status date", "Date of loan (facility) Disbursement/loan effective date", "Loan (facility) Amount / Availed Limit", "Outstanding Balance", "Instalment amount", "Currency", "Days in arrears", "Overdue amount", "Loan (Facility) type", "Loan (Facility) Tenor", "Maturity", "Repayment frequency", "Previous Account Number", "Branch Code", "Surname", "First Name", "Middle Name", "Date of Birth", "Natinonal Identity Number", "Drivers License No", "BVN No", "Passport NO", "Gender", "Nationality", "Marital Status", "Mobile Number", "Primary Address Line 1", "Primary Address Line 2", "Primary city/LGA", "Primary State", "Primary Country", "Employment Status", "Occupation",  "E-mail Address",  "Title", "Place of Birth", "Work Phone", "Home Phone","Reference Number"  };

            var Column = new string[] {  "CustomerID", "AccountId",      "Overdue_Status",          "",                                   "ProdEffDate",                                     "OpenBalance1",            "OutstandingBalance",      "Amount",        "Currency",  "Total_Days",      "Average_Amt",           "",                    "Tenor",        "MaturityDate", "PaymentFrequency",           "Alt_AcctId",       "CO_CODE",       "",     "FirstName",      "",          "BirthDate",    "NationalIdentityNum",                "",         "BVN",         "",      "Gender",     "",                "",       "DefaultPhone",      "Address" ,                 "",                      "",               "",              "",               "IsEmployee",      "Occupation",   "EmailAddress", "accountTitle", "" ,       "DefaultPhone","DefaultPhone", "ReferenceNo" };

            System.Drawing.Color green = new System.Drawing.Color();

            for (int i = 0; i < headers.Length; i++)
            {
                oSheet.Cells[curRow, i + 1].Value = headers[i];
                oSheet.Cells[curRow, i + 1].Style.Font.Bold = true;
                oSheet.Cells[curRow, i + 1].Style.Font.UnderLine = true;
                oSheet.Cells[curRow, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.LightGray;
                oSheet.Cells[curRow, i + 1].Style.Fill.BackgroundColor.SetColor(green);
            }

            curRow = curRow + 1;

            for (int i = 0; i < headers.Length; i++)
            {

                oSheet.Cells[curRow, i + 1].Value = "";
            }

            if (dt.Rows.Count != 0)
            {

                foreach (var drow in dt.AsEnumerable())
                {

                    //throw new Exception("No records found!");
                    for (int i = 0; i < headers.Length; i++)
                    {
                        if (Column[i].isNull())
                        {
                            Column[i] = "";
                        }

                        if (Column[i] != "")
                        {

                            oSheet.Cells[curRow, i + 1].Value = drow[Column[i]].ToString();

                        }

                        else

                        {

                            oSheet.Cells[curRow, i + 1].Value = "";

                        }
                    }

                    curRow = curRow + 1;

                }
            }











            //implement Credit Information method


            //curRow = 1;

            //sql = "select top 100 a.*,c.AccountId,c.Alt_AcctId,b.CustomerType,c.AmntLastDrAuto,c.DateLastDrAuto,c.Overdue_Status, (Select InternalAmount from Dimlimit where LiabNo = a.CustomerID) as InternalAmount, (Select InternalAmount from Dimlimit where LiabNo = a.CustomerID) as InternalAmount, (Select top 1 PaymentFrequency from factAAPaymentSchedule where PaymentArrangementID like '%'+a.ArrangementId+'%'  order by PaymentArrangementID desc) as PaymentFrequency, (Select MaturityDate from factAAAccountDetails where ArrangementID = a.ArrangementId) as MaturityDate, (Select top 1 OpenBalance1 from factBalances where AccountId = c.AccountId and TypeSysdate1 = 'TOTCOMMITMENT' order by BusinessDate desc) as OpenBalance1, (select (COALESCE(CAST((Select top 1 OpenBalance4 from factBalances where AccountId = c.AccountId and TypeSysdate4 = 'ACCPRINCIPALINT' order by BusinessDate desc) AS DECIMAL(20,4)),0) +  (COALESCE(CAST((Select top 1 OpenBalance3 from factBalances where AccountId = c.AccountId and TypeSysdate3 = 'CURACCOUNT'      order by BusinessDate desc) AS decimal(20,4)),0))))as OutstandingBalance, (Select top 1 OpenBalance4 from factBalances where AccountId = c.AccountId and TypeSysdate4 = 'ACCPRINCIPALINT' order by BusinessDate desc) AS OpenBalance4, (Select top 1 OpenBalance3 from factBalances where AccountId = c.AccountId and TypeSysdate3 = 'CURACCOUNT' order by BusinessDate desc)AS OpenBalance3, (Select top 1 Amount from factAAPaymentSchedule where PaymentArrangementID like '%'+a.ArrangementId+'%') as Amount, (Select Average_Amt from overdueLoans where ArrangementId like '%'+a.ArrangementId+'%') as Average_Amt,(Select Total_Days from overdueLoans where ArrangementId like '%'+a.ArrangementId+'%') as Total_Days,a.Currency from factArrangement a JOIN DimCustomer b on (a.CustomerID = b.CustomerId) JOIN DimAccount C on (a.LinkedApplId = c.AccountId) where a.ProductLine = 'Lending' and a.OrigContractDate IS null and ltrim(rtrim(b.CustomerType)) = 'NI'";

            //db = new InlaksBIContext().getDBInterface(new Settings().warehousedbtype, new Settings().warehousedb);

            //dt = db.getData(sql);

            //headers = new string[] { "Customer ID", "Account Number", "Account Status", "Account Status date", "Date of loan (facility) Disbursement/loan effective date", "Credit limit/Facility amount/Global limit", "Loan (facility) Amount / Availed Limit", "Outstanding Balance", "Instalment amount", "Currency", "Days in arrears", "Overdue amount", "Loan (Facility) type", "Loan (Facility) Tenor", "Repayment frequency", "Last payment date", "Last Payment amount", "Maturity", "Loan Classification", "Legal Challenge Status", "Litigation Date", "Consent Status", "Loan Security Status", "Collateral Type", "Previous Account Number", "Previous Name", "Previous Customer ID", "Previous Branch Code" };

            //Column = new string[] {   "CustomerID", "AccountId",      "Overdue_Status",           "",                         "ProdEffDate",                                          "InternalAmount",                           "OpenBalance1",                 "OutstandingBalance",        "Amount",      "Currency",   "Total_Days",       "Average_Amt",            "",                   "Tenor",         "PaymentFrequency",    "DateLastDrAuto",     "AmntLastDrAuto",   "MaturityDate",        "",                    "",                  "",                 "",                    "",                "",                  "Alt_AcctId",            "",               "",                      ""             };

            //for (int i = 0; i < headers.Length; i++)
            //{
            //    CreditInformationSheet.Cells[curRow, i + 1].Value = headers[i];
            //    CreditInformationSheet.Cells[curRow, i + 1].Style.Font.Bold = true;
            //    CreditInformationSheet.Cells[curRow, i + 1].Style.Font.UnderLine = true;
            //    CreditInformationSheet.Cells[curRow, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.LightGray;
            //    CreditInformationSheet.Cells[curRow, i + 1].Style.Fill.BackgroundColor.SetColor(green);
            //}

            //curRow = curRow + 1;

            //for (int i = 0; i < headers.Length; i++)
            //{
            //    CreditInformationSheet.Cells[curRow, i + 1].Value = "";
            //}

            //if (dt.Rows.Count != 0)
            //{

            //    //throw new Exception("No records found!");

            //    foreach (var drow in dt.AsEnumerable())
            //    {

            //        for (int i = 0; i < headers.Length; i++)
            //        {
            //            if (Column[i].isNull())
            //            {
            //                Column[i] = "";
            //            }

            //            if (Column[i] != "")
            //            {

            //                CreditInformationSheet.Cells[curRow, i + 1].Value = drow[Column[i]].ToString();

            //            }

            //            else

            //            {

            //                CreditInformationSheet.Cells[curRow, i + 1].Value = "";

            //            }
            //        }

            //        curRow = curRow + 1;
            //    }

            //}

            xlPackage.Save();

            return downpath;
        }

        public string generateNPFPortfolio_at_Risk(string downpath, string reportname, string classification,string subclass)
        {

            string today = DateTime.Now.ToString("dd-MM-yyyy");

            downpath = downpath + today + "_" + reportname + " by " +classification+ ".xlsx";

            ExcelWorksheet oSheet;
            ExcelPackage xlPackage;

            FileInfo newFile = new FileInfo(downpath);

            if (newFile.Exists)
            {
                newFile.Delete();
            }

            xlPackage = new ExcelPackage(newFile);


            oSheet = xlPackage.Workbook.Worksheets.Add("PAR");

            int curRow = 1;

            DataTable PrintTable = new DataTable();



            //implement Corporate browser method

            //   var sql = "select a.*,b.FirstName,c.AccountTitle,b.BVN,b.BirthDate,b.NationalIdentityNum,b.Gender,b.MaritalStatus,b.DefaultPhone,c.ClosedDate,b.Address,b.IsEmployee,b.Occupation,b.MiddleName,b.LastName,b.CustomerType,b.EmailAddress from factArrangement a JOIN DimCustomer b on (a.CustomerID = b.CustomerId) JOIN DimAccount C on (a.LinkedApplId = c.AccountId) where ProductLine = 'Lending'  and b.CustomerType = 'NI'";

            var filter = subclass.CleanUp().Equals("null") ? "" : " where " + classification + "='"+ subclass + "'";

            var sql = "Select *,'All Branches' as consolidated from parsummary"+filter;

            var db = new InlaksBIContext().getDBInterface(new Settings().warehousedbtype, new Settings().warehousedb);

            var dt = db.getData(sql);

            //if (dt.Rows.Count == 0)
            //{
            //    throw new Exception("No records found!");
            //}

            var headers = new string[] { "LOANID", "BRANCH_CODE", "BranchName", "product_name", "AC_NO", "loan_CYCLE", "NAME", "DISBURSMENT_DATE","MATURITY_DATE", "TENOR", "LAST_REPAYMENT_DATE", "PRINC_OUT", "LOAN_AMT", "TOT_INT", "CLASS_DATE", "PRINC_PAID", "INT_PAID", "INT_OUT", "PRINC_ARR", "INT_ARR", "PAR_PRINC", "PAR_INT", "AGE", "SECTOR", "classification", "consolidated" };


            dt.AddTableColumns(new string[] { "TOT_INT", "INT_PAID", "INT_OUT", "PRINC_ARR", "INT_ARR", "PAR_PRINC", "PAR_INT", "AGE", "classification" });



            dt = dt.AsEnumerable().Select(row =>
            {
                var loanamt = row["LOAN_AMT"].toDecimal();

                var rate = row["RATE"].toDecimal();

                var interest = (rate / 100) * loanamt;

                var accruedInterest = row["AccruedInterest"].toDecimal();

                var interestpaid = interest - accruedInterest;

                row["INT_PAID"] = interestpaid.ToString();

                row["TOT_INT"] = interest.ToString();

                row["INT_OUT"] = accruedInterest;

                


                if (row["CLASS_DATE"] == DBNull.Value)
                {

                    row["PRINC_ARR"] = 0;
                    row["INT_ARR"] = 0;
                    row["PAR_PRINC"] = 0;
                    row["PAR_INT"] = 0;
                    row["classification"] = clasifyoverdues(row["overdueStatus"].ToString());
                }
                else
                {
                    var overdueamt = row["OverdueAmount"].toDecimal();
                    var interestarr = (rate / 100) * overdueamt;
                    var prinarr = overdueamt - interestarr;
                    var overduedate = row["CLASS_DATE"].toDateTime();
                    var overduestatus = row["overdueStatus"].ToString();
                    row["classification"] = clasifyoverdues(overduestatus);
                    row["AGE"] = Utils.getAge(overduedate).ToString();
                    row["PRINC_ARR"] = prinarr;
                    row["INT_ARR"] = interestarr;
                    row["PAR_PRINC"] = row["PRINC_OUT"];
                    row["PAR_INT"] = row["INT_OUT"];

                }


                return row;
            }).CopyToDataTable();


            dt = new DataView(dt).ToTable(false, headers);


          //  bool isconsolidated = classification.CleanUp().Equals("consolidated");


            var classgrp = dt.AsEnumerable().GroupBy(r => r[classification]).Select(grp => grp.ToList());




            curRow = 3;

            foreach (var grp in classgrp)
            {
                var totaloutstandingprincpal = grp.Sum(r => r["PRINC_OUT"].toDecimal());
                var totaloutstandingintrst = grp.Sum(r => r["INT_OUT"].toDecimal());

                var totoutstanding = totaloutstandingprincpal + totaloutstandingintrst;

                var totparinterest = grp.Sum(r => r["PAR_INT"].toDecimal());
                var totparprincipal = grp.Sum(r => r["PAR_PRINC"].toDecimal());

                var totpar = totparprincipal + totparinterest;

                var par = (totpar / totoutstanding) * 100;



                //case "GRC":
                //    return "PASS&WATCH";


                //case "NAB":

                //    return "DOUBTFUL";

                //case "DEL":
                //    return "SUB STANDARD";

                //case "LOS":
                //    return "LOST";

                //default:
                //    return "PERFORMING";

                var totpassnwatch = sumoverdueclasses("PASS&WATCH", grp);

                var totdoubtful = sumoverdueclasses("DOUBTFUL", grp);

                var totsubstandard = sumoverdueclasses("SUB STANDARD", grp);

                var totloss = sumoverdueclasses("LOST", grp);


                oSheet.Cells[curRow, 2].Value = "NPF MICROFINANCE BANK PLC";
                oSheet.Cells[curRow, 2].Style.Font.Bold = true;
                oSheet.Cells[curRow, 2].Style.Font.UnderLine = false;
                //oSheet.Cells[3, 1, 3, 13].Merge = true;

                curRow = curRow + 1;

                oSheet.Cells[curRow, 2].Value = reportname;
                oSheet.Cells[curRow, 2].Style.Font.Bold = true;
                oSheet.Cells[curRow, 2].Style.Font.UnderLine = false;


                curRow = curRow + 1;

                oSheet.Cells[curRow, 2].Value = grp[0][classification].ToString();
                oSheet.Cells[curRow, 2].Style.Font.Bold = true;
                oSheet.Cells[curRow, 2].Style.Font.UnderLine = false;

                curRow = curRow + 1;

                oSheet.Cells[curRow, 2].Value = "TOTAL OUTSTANDING BALANCE: " + totoutstanding.ToString("#,##0.00");
                oSheet.Cells[curRow, 2].Style.Font.Bold = true;
                oSheet.Cells[curRow, 2].Style.Font.UnderLine = false;

                oSheet.Cells[curRow, 4].Value = "PASS&WATCH: " + totpassnwatch.ToString("#,##0.00");
                oSheet.Cells[curRow, 4].Style.Font.Bold = true;
                oSheet.Cells[curRow, 4].Style.Font.UnderLine = false;



                curRow = curRow + 1;

                oSheet.Cells[curRow, 2].Value = "Total Portfolio at Risk: " + totpar.ToString("#,##0.00");
                oSheet.Cells[curRow, 2].Style.Font.Bold = true;
                oSheet.Cells[curRow, 2].Style.Font.UnderLine = false;

                oSheet.Cells[curRow, 4].Value = "DOUBTFUL: " + totdoubtful.ToString("#,##0.00");
                oSheet.Cells[curRow, 4].Style.Font.Bold = true;
                oSheet.Cells[curRow, 4].Style.Font.UnderLine = false;

                curRow = curRow + 1;

                oSheet.Cells[curRow, 2].Value = "Total Principal at Risk: " + totparprincipal.ToString("#,##0.00");
                oSheet.Cells[curRow, 2].Style.Font.Bold = true;
                oSheet.Cells[curRow, 2].Style.Font.UnderLine = false;


             



                oSheet.Cells[curRow, 4].Value = "SUB STANDARD: " + totsubstandard.ToString("#,##0.00");
                oSheet.Cells[curRow, 4].Style.Font.Bold = true;
                oSheet.Cells[curRow, 4].Style.Font.UnderLine = false;


                curRow = curRow + 1;



                oSheet.Cells[curRow, 2].Value = "Total Interest at Risk: " + totparinterest.ToString("#,##0.00");
                oSheet.Cells[curRow, 2].Style.Font.Bold = true;
                oSheet.Cells[curRow, 2].Style.Font.UnderLine = false;

                oSheet.Cells[curRow, 4].Value = "LOSS: " + totloss.ToString("#,##0.00");
                oSheet.Cells[curRow, 4].Style.Font.Bold = true;
                oSheet.Cells[curRow, 4].Style.Font.UnderLine = false;



                curRow = curRow + 1;

                oSheet.Cells[curRow, 2].Value = "PAR Ratio: " + par.ToString("0.00");
                oSheet.Cells[curRow, 2].Style.Font.Bold = true;
                oSheet.Cells[curRow, 2].Style.Font.UnderLine = false;



                curRow = curRow + 2;




                for (int i = 0; i < (headers.Length-1); i++)
                {
                    
                    oSheet.Cells[curRow, 2 + i].Value = headers[i];
                    oSheet.Cells[curRow, 2 + i].Style.Font.Bold = true;
                    oSheet.Cells[curRow, 2 + i].Style.Font.UnderLine = false;

                }

                curRow = curRow + 1;

                foreach (DataRow row in grp)
                {
                    

                    for (int i = 0; i < (dt.Columns.Count-1); i++)
                    {

                        if (dt.Columns[i].ColumnName.Contains("PRINC")|| dt.Columns[i].ColumnName.Contains("INT") || dt.Columns[i].ColumnName.Contains("LOAN_AMT") )
                        {
                            oSheet.Cells[curRow, i + 2].Value = row[i].toDecimal();
                            oSheet.Cells[curRow, i + 2].Style.Numberformat.Format = "#,##0.00";
                        }
                        else
                        {

                            oSheet.Cells[curRow, i + 2].Value = row[i].ToString();
                        }

                    }

                    curRow = curRow + 1;

                }
                curRow = curRow + 4;


            }
       


         

            xlPackage.Save();

            return downpath;


        }



        public string generateLoanRepaymentsReport(string downpath, string reportname, string classification, string StartDate, string EndDate)
        {

         


            var sd = DateTime.ParseExact(StartDate, "yyyy-MM-dd", new CultureInfo("en-Us")).ToString("dd-MMM-yyyy");

            var ed = DateTime.ParseExact(EndDate, "yyyy-MM-dd", new CultureInfo("en-Us")).ToString("dd-MMM-yyyy");

            ExcelWorksheet oSheet;
            //ExcelWorksheet CreditInformationSheet;
            ExcelPackage xlPackage;

            downpath = downpath + sd + "-" + ed + "_" + reportname + ".xlsx";

            FileInfo newFile = new FileInfo(downpath);

            if (newFile.Exists)
            {
                newFile.Delete();
            }

            xlPackage = new ExcelPackage(newFile);

            oSheet = xlPackage.Workbook.Worksheets.Add("Repayments Report Sheet");

            // CreditInformationSheet = xlPackage.Workbook.Worksheets.Add("Credit Information Sheet");

            int curRow = 1;

        


       


            var sql = "select * from loanrepayments where SettledDate between '" + sd+"' and '"+ed+"'";

            var db = new InlaksBIContext().getDBInterface(new Settings().warehousedbtype, new Settings().warehousedb);

            var dt = db.getData(sql);

            //if (dt.Rows.Count == 0)
            //{
            //    throw new Exception("No records found!");
            //}



            //  bool isconsolidated = classification.CleanUp().Equals("consolidated");


            var classgrp = dt.AsEnumerable().GroupBy(r => r[classification]).Select(grp => grp.ToList());




            curRow = 3;

            foreach (var grp in classgrp)
            {
                var totalrepayments = grp.Sum(r => r["PaymentAmount"].toDecimal());
                var totprinpaid = grp.Sum(r => r["Or_Pr_Amount"].toDecimal());

                var totintpaid = grp.Sum(r => r["InterestPaid"].toDecimal());



                oSheet.Cells[curRow, 2].Value = "NPF MICROFINANCE BANK PLC";
                oSheet.Cells[curRow, 2].Style.Font.Bold = true;
                oSheet.Cells[curRow, 2].Style.Font.UnderLine = false;
                //oSheet.Cells[3, 1, 3, 13].Merge = true;

                curRow = curRow + 1;

                oSheet.Cells[curRow, 2].Value = reportname;
                oSheet.Cells[curRow, 2].Style.Font.Bold = true;
                oSheet.Cells[curRow, 2].Style.Font.UnderLine = false;


                curRow = curRow + 1;

                oSheet.Cells[curRow, 2].Value = grp[0][classification].ToString();
                oSheet.Cells[curRow, 2].Style.Font.Bold = true;
                oSheet.Cells[curRow, 2].Style.Font.UnderLine = false;

                curRow = curRow + 1;

                oSheet.Cells[curRow, 2].Value = "TOTAL REPAYMENTS: " + totalrepayments.ToString("#,##0.00");
                oSheet.Cells[curRow, 2].Style.Font.Bold = true;
                oSheet.Cells[curRow, 2].Style.Font.UnderLine = false;

                curRow = curRow + 1;

                oSheet.Cells[curRow, 2].Value = "TOTAL PRINCIPAL REPAID: " + totprinpaid.ToString("#,##0.00");
                oSheet.Cells[curRow, 2].Style.Font.Bold = true;
                oSheet.Cells[curRow, 2].Style.Font.UnderLine = false;

                curRow = curRow + 1;


                oSheet.Cells[curRow, 2].Value = "TOTAL INTEREST REPAID: " + totintpaid.ToString("#,##0.00");
                oSheet.Cells[curRow, 2].Style.Font.Bold = true;
                oSheet.Cells[curRow, 2].Style.Font.UnderLine = false;


                curRow = curRow + 2;


                for (int i = 0; i < dt.Columns.Count; i++)
                {

                    oSheet.Cells[curRow, 2 + i].Value = dt.Columns[i].ColumnName;
                    oSheet.Cells[curRow, 2 + i].Style.Font.Bold = true;
                    oSheet.Cells[curRow, 2 + i].Style.Font.UnderLine = false;

                }

                curRow = curRow + 1;

                foreach (DataRow row in grp)
                {


                    for (int i = 0; i < dt.Columns.Count; i++)
                    {

                        if (dt.Columns[i].ColumnName.Contains("InterestPaid") || dt.Columns[i].ColumnName.Contains("Amount"))
                        {
                            oSheet.Cells[curRow, i + 2].Value = row[i].toDecimal();
                            oSheet.Cells[curRow, i + 2].Style.Numberformat.Format = "#,##0.00";
                        }
                        else
                        {

                            oSheet.Cells[curRow, i + 2].Value = row[i].ToString();
                        }

                    }

                    curRow = curRow + 1;

                }
                curRow = curRow + 4;


            }





            xlPackage.Save();

            return downpath;


        }



        public string generateAuditTrailReport(string startdate, string enddate, string reportname, string downpath, string branch, string userid)
        {

            //DateTime start = DateTime.ParseExact(startdate, "yyyy-MM-dd", new System.Globalization.CultureInfo("en-US"));
            // DateTime end = DateTime.ParseExact(enddate, "yyyy-MM-dd", new System.Globalization.CultureInfo("en-US"));
            var sql = "select * from auditlogs where Date between '" + startdate + "'  and '" + enddate + "'";

            if (!userid.CleanUp().Equals("null"))
            {
                sql = sql + " and UserID='" + userid + "'";
            }

            if (!branch.CleanUp().Equals("null"))
            {
                sql = sql + " and BranchID='" + branch + "'";
            }

            var db = new InlaksBIContext().getDBInterface(new Settings().warehousedbtype, new Settings().warehousedb);

            var dt = db.getData(sql);

            if (dt.Rows.Count == 0)
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


            //var headers = new string[] { "EntryID", "AccountID", "OldAccountNo", "BranchId", "Amount", "CustomerId",  "CustomerName", "Telephone", "CustomerAddress", "ValueDate", "Currency", "OurReference", "TransactionReference", "PostDate", "Inputer", "Authoriser", "CrDr", "Time", "BranchName", "BranchAddress", "Narrative", "Birthdate" };


            //dt = new DataView(dt).ToTable(false, headers);


            var branchgroups = dt.AsEnumerable().GroupBy(r => r["BranchID"].ToString()).Select(grp => grp.ToList());


            foreach (var branchdata in branchgroups)
            {


                //oSheet.Cells[curRow, 2].Value = "NPF MICROFINANCE BANK PLC";
                //oSheet.Cells[curRow, 2].Style.Font.Bold = true;
                //oSheet.Cells[curRow, 2].Style.Font.UnderLine = false;
                ////oSheet.Cells[3, 1, 3, 13].Merge = true;

                //curRow = curRow + 1;

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


                //}     IDENTIFICATION	REGISTRATION	DATE OF	PLACE OF	ISSUING	CUSTOMER 	FIRST ADDRESS	SECOND ADDRESS
                //NUMBER CERTIFICATE NO.ISSUE ISSUE   AUTHORITY ADDRESS  TYPE LINE    LINE
  

                var headers = new string[] { "S/N", "BranchID", "ProtocolID", "UserID", "Date", "Time", "Classification", "Application", "Level_Function", "RecordID", "Remark", "TerminalID", "BranchName" };
                var Column = new string[] { "SN", "BranchId", "ProtocolID", "UserID", "Date", "Time", "Classification", "Application", "Level_Function", "RecordID", "Remark", "TerminalID", "BranchName" };


        System.Drawing.Color green = new System.Drawing.Color();
                for (int i = 0; i < headers.Length; i++)
                {
                    oSheet.Cells[curRow, i + 1].Value = headers[i];
                    oSheet.Cells[curRow, i + 1].Style.Font.Bold = true;
                    oSheet.Cells[curRow, i + 1].Style.Font.UnderLine = true;
                    oSheet.Cells[curRow, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.LightGray;
                    oSheet.Cells[curRow, i + 1].Style.Fill.BackgroundColor.SetColor(green);
                }

                curRow = curRow + 1;

                for (int i = 0; i < headers.Length; i++)
                {

                    oSheet.Cells[curRow, i + 1].Value = "";
                }

                if (dt.Rows.Count != 0)
                {
                    int count = 0;

                    foreach (var drow in branchdata.AsEnumerable())
                    {
                        count = count + 1;
                        //throw new Exception("No records found!");
                        for (int i = 0; i < headers.Length; i++)
                        {
                            if (Column[i].isNull())
                            {
                                Column[i] = "";
                            }

                            if (Column[i] != "")
                            {
                                if (Column[i].Trim() == "SN")
                                {
                                    oSheet.Cells[curRow, i + 1].Value = count;
                                }
                                else
                                {


                                    oSheet.Cells[curRow, i + 1].Value = drow[Column[i]].ToString();

                                    if (Column[i].Trim() == "Amount")
                                    {
                                        oSheet.Cells[curRow, i + 1].Value = drow[Column[i]].toDecimal();
                                        oSheet.Cells[curRow, i + 1].Style.Numberformat.Format = "#,##0.00";
                                    }

                                }




                            }

                            else

                            {

                                oSheet.Cells[curRow, i + 1].Value = "";

                            }
                        }

                        curRow = curRow + 1;

                    }
                }


                curRow = curRow + 4;

            }

            xlPackage.Save();

            return downpath;
        }




        private string clasifyoverdues(string value)
        {
            switch (value.Trim())
            {
                case "GRC":
                    return "PASS&WATCH";
                

                case "NAB":

                    return "DOUBTFUL";

                case "DEL":
                    return "SUB STANDARD";

                case "LOS":
                    return "LOST";

                default:
                    return "PERFORMING";
            }

        }


        private decimal sumoverdueclasses(string value, List<DataRow> data)
        {
            decimal sum = decimal.Zero;
            switch (value.Trim())
            {
                case "performing":
                    var perf = data.Where(r => r["CLASS_DATE"] == DBNull.Value).ToList();
                    sum = perf.Sum(r => r["PRINC_OUT"].toDecimal());
                    break;

                default:
                    var valid = data.Where(r => r["classification"].ToString().Trim() == value).ToList();

                    sum = valid.Sum(r => r["PRINC_OUT"].toDecimal());
                    break;
            }

            return sum;
    }

    }






}