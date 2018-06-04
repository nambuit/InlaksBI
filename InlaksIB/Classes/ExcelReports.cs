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
            var sql = "select a.*, b.BranchName,b.BranchAddress, c.Narrative,d.BirthDate,d.FirstName as CustomerName,d.DefaultPhone as Telephone,d.Address as CustomerAddress, e.Alt_AcctId as OldAccountNo from  factStmt a  join DimBranch b on (a.BranchId=b.SourceBranchId) join dimStmt c on (a.TransactionCode=TransactionId) join DimCustomer d on (a.CustomerId=d.CustomerId) join DimAccount e on (a.AccountID=e.AccountId) where a.PostDate between '" + startdate+ "'  and '" + enddate + "'";

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



        public string generateNPFUniformConsumerFormatReport(string downpath, string reportname)
        {

            string today = DateTime.Now.ToString("dd-MM-yyyy");

            downpath = downpath + today + "_" + reportname + ".xlsx";

            ExcelWorksheet IndividualBrowserSheet;
            ExcelWorksheet CreditInformationSheet;
            ExcelPackage xlPackage;

            FileInfo newFile = new FileInfo(downpath);

            if (newFile.Exists)
            {
                newFile.Delete();
            }

            xlPackage = new ExcelPackage(newFile);

            IndividualBrowserSheet = xlPackage.Workbook.Worksheets.Add("Individual Borrower sheet");

            CreditInformationSheet = xlPackage.Workbook.Worksheets.Add("Credit Information Sheet");

            int curRow = 1;

            DataTable PrintTable = new DataTable();



            //implement individual browser method

            var sql = "select a.*,b.FirstName,c.AccountTitle,b.BVN,b.BirthDate,b.NationalIdentityNum,b.Gender,b.MaritalStatus,b.DefaultPhone,c.ClosedDate,b.Address,b.IsEmployee,b.Occupation,b.MiddleName,b.LastName,b.CustomerType,b.EmailAddress from factArrangement a JOIN DimCustomer b on (a.CustomerID = b.CustomerId) JOIN DimAccount C on (a.LinkedApplId = c.AccountId) where ProductLine = 'Lending'  and b.CustomerType = 'I'";

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
                IndividualBrowserSheet.Cells[curRow, i + 1].Value = headers[i];
                IndividualBrowserSheet.Cells[curRow, i + 1].Style.Font.Bold = true;
                IndividualBrowserSheet.Cells[curRow, i + 1].Style.Font.UnderLine = true;
                IndividualBrowserSheet.Cells[curRow, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.LightGray;
                IndividualBrowserSheet.Cells[curRow, i + 1].Style.Fill.BackgroundColor.SetColor(green);
            }

            curRow = curRow + 1;

            for (int i = 0; i < headers.Length; i++)
            {

                IndividualBrowserSheet.Cells[curRow, i + 1].Value = "";
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

                            IndividualBrowserSheet.Cells[curRow, i + 1].Value = drow[Column[i]].ToString();

                        }

                        else

                        {

                            IndividualBrowserSheet.Cells[curRow, i + 1].Value = "";

                        }
                    }

                    curRow = curRow + 1;

                }
            }


            //implement Credit Information method for individual


            curRow = 1;

            sql = "select * from loansummary where ((ltrim(rtrim(CustomerType)) != 'NI' or CustomerType is null))";

            db = new InlaksBIContext().getDBInterface(new Settings().warehousedbtype, new Settings().warehousedb);

            dt = db.getData(sql);

            headers = new string[] { "CustomerID", "Account Number", "Account Status", "Account Status date", "Date of loan (facility) Disbursement/loan effective date", "Credit limit/Facility amount/Global limit", "Loan (facility) Amount / Availed Limit", "Outstanding Balance", "Instalment amount", "Currency", "Days in arrears", "Overdue amount", "Loan (Facility) type", "Loan (Facility) Tenor", "Repayment frequency", "Last payment date", "Last Payment amount", "Maturity", "Loan Classification", "Legal Challenge Status", "Litigation Date", "Consent Status", "Loan Security Status", "Collateral Type", "Previous Account Number", "Previous Name", "Previous Customer ID", "Previous Branch Code" };

            Column = new string[] { "CustomerID", "AccountId", "Overdue_Status", "", "ProdEffDate", "InternalAmount", "OpenBalance1", "OutstandingBalance", "Amount", "Currency", "Total_Days", "Average_Amt", "", "", "PaymentFrequency", "DateLastDrAuto", "AmntLastDrAuto", "MaturityDate", "", "", "", "", "", "", "Alt_AcctId", "", "", "" };

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

            ExcelWorksheet IndividualBrowserSheet;
            ExcelWorksheet CreditInformationSheet;
            ExcelPackage xlPackage;

            FileInfo newFile = new FileInfo(downpath);

            if (newFile.Exists)
            {
                newFile.Delete();
            }

            xlPackage = new ExcelPackage(newFile);

            IndividualBrowserSheet = xlPackage.Workbook.Worksheets.Add("Corporate Borrower Sheet");

            CreditInformationSheet = xlPackage.Workbook.Worksheets.Add("Credit Information Sheet");

            int curRow = 1;

            DataTable PrintTable = new DataTable();



            //implement Corporate browser method

            var sql = "select a.*,b.FirstName,c.AccountTitle,b.BVN,b.BirthDate,b.NationalIdentityNum,b.Gender,b.MaritalStatus,b.DefaultPhone,c.ClosedDate,b.Address,b.IsEmployee,b.Occupation,b.MiddleName,b.LastName,b.CustomerType,b.EmailAddress from factArrangement a JOIN DimCustomer b on (a.CustomerID = b.CustomerId) JOIN DimAccount C on (a.LinkedApplId = c.AccountId) where ProductLine = 'Lending'  and b.CustomerType = 'NI'";

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
                IndividualBrowserSheet.Cells[curRow, i + 1].Value = headers[i];
                IndividualBrowserSheet.Cells[curRow, i + 1].Style.Font.Bold = true;
                IndividualBrowserSheet.Cells[curRow, i + 1].Style.Font.UnderLine = true;
                IndividualBrowserSheet.Cells[curRow, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.LightGray;
                IndividualBrowserSheet.Cells[curRow, i + 1].Style.Fill.BackgroundColor.SetColor(green);
            }

            curRow = curRow + 1;

            for (int i = 0; i < headers.Length; i++)
            {

                IndividualBrowserSheet.Cells[curRow, i + 1].Value = "";
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

                            IndividualBrowserSheet.Cells[curRow, i + 1].Value = drow[Column[i]].ToString();

                        }

                        else

                        {

                            IndividualBrowserSheet.Cells[curRow, i + 1].Value = "";

                        }
                    }

                    curRow = curRow + 1;

                }
            }



            //implement Credit Information method for corporate


            curRow = 1;

            sql = "select * from loansummary where ltrim(rtrim(CustomerType)) = 'NI'";

            db = new InlaksBIContext().getDBInterface(new Settings().warehousedbtype, new Settings().warehousedb);

            dt = db.getData(sql);

            headers = new string[] { "Customer ID", "Account Number", "Account Status", "Account Status date", "Date of loan (facility) Disbursement/loan effective date", "Credit limit/Facility amount/Global limit", "Loan (facility) Amount / Availed Limit", "Outstanding Balance", "Instalment amount", "Currency", "Days in arrears", "Overdue amount", "Loan (Facility) type", "Loan (Facility) Tenor", "Repayment frequency", "Last payment date", "Last Payment amount", "Maturity", "Loan Classification", "Legal Challenge Status", "Litigation Date", "Consent Status", "Loan Security Status", "Collateral Type", "Previous Account Number", "Previous Name", "Previous Customer ID", "Previous Branch Code" };

            Column = new string[] { "CustomerID", "AccountId", "Overdue_Status", "", "ProdEffDate", "InternalAmount", "OpenBalance1", "OutstandingBalance", "Amount", "Currency", "Total_Days", "Average_Amt", "", "Tenor", "PaymentFrequency", "DateLastDrAuto", "AmntLastDrAuto", "MaturityDate", "", "", "", "", "", "", "Alt_AcctId", "", "", "" };

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



        public string generateNPFDisbursementReport(string downpath, string reportname, string StartDate, string EndDate)
        {

            string today = DateTime.Now.ToString("dd-MM-yyyy");

            downpath = downpath + today + "_" + reportname + ".xlsx";

            ExcelWorksheet IndividualBrowserSheet;
            //ExcelWorksheet CreditInformationSheet;
            ExcelPackage xlPackage;

            FileInfo newFile = new FileInfo(downpath);

            if (newFile.Exists)
            {
                newFile.Delete();
            }

            xlPackage = new ExcelPackage(newFile);

            IndividualBrowserSheet = xlPackage.Workbook.Worksheets.Add("Disbursment Report Sheet");

            // CreditInformationSheet = xlPackage.Workbook.Worksheets.Add("Credit Information Sheet");

            int curRow = 1;

            DataTable PrintTable = new DataTable();



            //implement Corporate browser method

            var sql = "select a.*,c.AccountId,c.Alt_AcctId,b.CustomerType,c.AmntLastDrAuto,c.DateLastDrAuto,c.Overdue_Status, (Select InternalAmount from Dimlimit where LiabNo = a.CustomerID) as InternalAmount,b.firstname,c.accountTitle,b.bvn,b.birthdate,b.NationalIdentityNum,b.Gender,b.MaritalStatus,b.DefaultPhone,c.ClosedDate,b.Address,b.IsEmployee,b.Occupation,b.MiddleName,b.LastName,b.CustomerType,b.EmailAddress,(Select top 1 PaymentFrequency from factAAPaymentSchedule where PaymentArrangementID like '%'+a.ArrangementId+'%'  order by PaymentArrangementID desc) as PaymentFrequency, (Select MaturityDate from factAAAccountDetails where ArrangementID = a.ArrangementId) as MaturityDate, (Select top 1 OpenBalance1 from factBalances where AccountId = c.AccountId and TypeSysdate1 = 'TOTCOMMITMENT' order by BusinessDate desc) as OpenBalance1, (select (COALESCE(CAST((Select top 1 OpenBalance4 from factBalances where AccountId = c.AccountId and TypeSysdate4 = 'ACCPRINCIPALINT' order by BusinessDate desc) AS DECIMAL(20,4)),0) + (COALESCE(CAST((Select top 1 OpenBalance3 from factBalances where AccountId = c.AccountId and TypeSysdate3 = 'CURACCOUNT' order by BusinessDate desc) AS decimal(20,4)),0))))as OutstandingBalance, (Select top 1 Amount from factAAPaymentSchedule where PaymentArrangementID like '%'+a.ArrangementId+'%') as Amount,(Select top 1 Term from factAATermAmount where ArrangementID = a.ArrangementId  order by TermArrangementID desc) as Tenor, (Select Average_Amt from overdueLoans where ArrangementId like '%'+a.ArrangementId+'%') as Average_Amt,(Select Total_Days from overdueLoans where ArrangementId like '%'+a.ArrangementId+'%') as Total_Days from factArrangement a JOIN DimCustomer b on (a.CustomerID = b.CustomerId) JOIN DimAccount C on (a.LinkedApplId = c.AccountId) where a.ProductLine = 'Lending' and a.OrigContractDate IS null and a.ArrStatus != 'UNAUTH' and a.ProdEffDate > '20180401'  and a.StartDate > '" + StartDate + "'   and a.StartDate < '" + EndDate + "'";

            var db = new InlaksBIContext().getDBInterface(new Settings().warehousedbtype, new Settings().warehousedb);

            var dt = db.getData(sql);

            //if (dt.Rows.Count == 0)
            //{
            //    throw new Exception("No records found!");
            //}
            //////////

            var headers = new string[] { "CustomerID", "Account Number", "Account Status", "Account Status date", "Date of loan (facility) Disbursement/loan effective date", "Loan (facility) Amount / Availed Limit", "Outstanding Balance", "Instalment amount", "Currency", "Days in arrears", "Overdue amount", "Loan (Facility) type", "Loan (Facility) Tenor", "Maturity", "Repayment frequency", "Previous Account Number", "Branch Code", "Surname", "First Name", "Middle Name", "Date of Birth", "Natinonal Identity Number", "Drivers License No", "BVN No", "Passport NO", "Gender", "Nationality", "Marital Status", "Mobile Number", "Primary Address Line 1", "Primary Address Line 2", "Primary city/LGA", "Primary State", "Primary Country", "Employment Status", "Occupation", "E-mail Address", "Title", "Place of Birth", "Work Phone", "Home Phone" };

            var Column = new string[] { "CustomerID", "AccountId", "Overdue_Status", "", "ProdEffDate", "OpenBalance1", "OutstandingBalance", "Amount", "Currency", "Total_Days", "Average_Amt", "", "Tenor", "MaturityDate", "PaymentFrequency", "Alt_AcctId", "CO_CODE", "", "FirstName", "", "BirthDate", "NationalIdentityNum", "", "BVN", "", "Gender", "", "", "DefaultPhone", "Address", "", "", "", "", "IsEmployee", "Occupation", "EmailAddress", "accountTitle", "", "DefaultPhone", "DefaultPhone" };

            System.Drawing.Color green = new System.Drawing.Color();

            for (int i = 0; i < headers.Length; i++)
            {
                IndividualBrowserSheet.Cells[curRow, i + 1].Value = headers[i];
                IndividualBrowserSheet.Cells[curRow, i + 1].Style.Font.Bold = true;
                IndividualBrowserSheet.Cells[curRow, i + 1].Style.Font.UnderLine = true;
                IndividualBrowserSheet.Cells[curRow, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.LightGray;
                IndividualBrowserSheet.Cells[curRow, i + 1].Style.Fill.BackgroundColor.SetColor(green);
            }

            curRow = curRow + 1;

            for (int i = 0; i < headers.Length; i++)
            {

                IndividualBrowserSheet.Cells[curRow, i + 1].Value = "";
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

                            IndividualBrowserSheet.Cells[curRow, i + 1].Value = drow[Column[i]].ToString();

                        }

                        else

                        {

                            IndividualBrowserSheet.Cells[curRow, i + 1].Value = "";

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


    }




}