using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackBone
{
    public static class Scripts
    {
        public static string CreateExtractionTemp
        {
            get {
                return "CREATE TABLE [dbo].[ExtractionTemp](\n" +
"	[SN] [int] NOT NULL,\n" +
"	[Postdate] [datetime] NULL,\n" +
"	[ValDate] [datetime] NULL,\n" +
"	[details] [char](1500) NULL,\n" +
"	[Debits] [money] NULL,\n" +
"	[Credits] [money] NULL,\n" +
"	[Amount] [money] NULL,\n" +
"	[CrDr] [float] NULL,\n" +
"	[ud1] [varchar](300) NULL,\n" +
"	[ud2] [varchar](300) NULL,\n" +
"	[ud3] [varchar](300) NULL,\n" +
"	[ud4] [varchar](300) NULL,\n" +
"	[ud5] [varchar](300) NULL,\n" +
"	[Id] [varchar](100) NULL,\n" +
"	[username] [varchar](100) NULL\n" +
") ON [PRIMARY]";
                }
        }

        public static string CreateAlldetailsView
        {
            get
            {
                return "CREATE  view [dbo].[alldetails] as \n" +
"select distinct a.BANKCODE, a.bankname as AccountName, a.SHORTNAME,A.LOCAL_NO, a.branchcode, a.CurrentUser, a.CUR_DATE, a.LASTDATE, a.LASTSTMT, a.CA_ACCTNO, a.FOREIGN_NO,\n" +
"a.currency_code as currency,a.INTERNALRECORD as ledger, a.EXTERNALRECORD as statement, a.Account_Active, a.ACCTYPE, b.branchname as Domain, b.affiliate_id,\n" +
" c.affiliate_name as Affiliate, d.UserName as id, d.can_access, d.can_match from NLBANKS a join BranchList b on (a.BranchCode=b.BranchCode) \n" +
"		join affiliates c on(b.affiliate_id=c.affiliate_id)  , User_Accounts d";
            }
        }

   public static string CreateDefinitionTable
        {
            get
            {
                return "CREATE TABLE [dbo].[InterfaceDefinition](\n" +
"	[ID] [varchar](100) NOT NULL,\n" +
"	[PostDate_Col] [varchar](400) NOT NULL,\n" +
"	[ValDate_Col] [varchar](400) NOT NULL,\n" +
"	[Naration] [varchar](1000) NOT NULL,\n" +
"	[Amount_Col] [varchar](400) NOT NULL,\n" +
"	[direction_Col] [varchar](400) NOT NULL,\n" +
"	[ud1_Col] [varchar](400) NULL,\n" +
"	[ud2_Col] [varchar](400) NULL,\n" +
"	[ud3_Col] [varchar](400) NULL,\n" +
"	[ud4_Col] [varchar](400) NULL,\n" +
"	[ud5_Col] [varchar](400) NULL,\n" +
"	[script] [varchar](max) NOT NULL,\n" +
"	[bal_script] [varchar](max) NOT NULL,\n" +
"	[bal_Col] [varchar](400) NOT NULL,\n" +
"	[Naration_Type] [varchar](400) DEFAULT ('normal'),\n" +
"PRIMARY KEY CLUSTERED \n" +
"(\n" +
"	[ID] ASC\n" +
")WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]\n" +
") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";
            }
        }



        public static string RestorePoint
        {
            get
            {
                return "insert into Account_Restore_Point (Account_ID,LedgerBal,stmtBal,LedgerLastID,stmtLastID,Action_Time,UserID,Description,Action_Type,LastDate,Side)\n" +
                        "values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}')";
            }
        }

        public static string Balances
        {
            get
            {
                return "select t1.closebal as ledgerbal, t2.closebal as stmtbal from ledgerbal t1, stmtbal t2 where  \n" +
                        "t1.Period=t2.Period and t1.Account_ID=t2.Account_ID\n" +
                        "and t1.Account_ID ='{0}' and t2.Period='{1}' ";
            }
        }

        public static string updateMonthlyTable
        {
            get
            {
                return "insert into {0}  (Postdate, Valdate, Details, Amount, CrDr,UD1, UD2, UD3, UD4, UD5,Account_ID) \n" +
                "select Postdate,ValDate, details, Amount, CrDr, ud1, ud2, ud3, ud4, ud5, '{1}' from ExtractionTemp where ID ='{2}'";
            }
        }



        public static string CreateAccounts
        {
            get
            {
                return "Insert into NLbanks (BANKCODE,CA_ACCTNO,BANKNAME,FOREIGN_NO,ACCTNO,SHORTNAME,TITLE,ACCTYPE,START_DATE,CUR_DATE,LASTDATE,LASTSTMT,INTERNALRECORD,EXTERNALRECORD,Account_Active,Dr_Divisor,Cr_Divisor,currency_code,BranchCode,Ledger_Creation_Balance,Statement_Creation_Balance,Ledger_ASAT_Date,Stmt_ASAT_Date,Creation_Date)\n" +
                       "values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}')"; 

    }
        }

public static string CreateMonthlyTable
        {
            get
            {
                return "CREATE TABLE [dbo].[monthlyTable](\n" +
   "	[Acctno] [varchar](25) NULL,\n" +
   "	[Details] [varchar](1000) NULL,\n" +
   "	[Matches] [varchar](1200) NULL,\n" +
   "	[MatchType] [varchar](4) NULL,\n" +
   "	[Remarks] [varchar](60) NULL,\n" +
   "	[Serial_no] [varchar](20) NULL,\n" +
   "	[Postdate] [datetime] NULL,\n" +
   "	[Valdate] [datetime] NULL,\n" +
   "	[CrDr] [float] NULL,\n" +
   "	[Amount] [money] NULL,\n" +
   "	[Analcode] [float] NULL,\n" +
   "	[Period] [float] NULL,\n" +
   "	[idfields] [float] NULL,\n" +
   "	[Tracer_Note] [varchar](30) NULL,\n" +
   "	[MatchID] [varchar](20) NULL,\n" +
   "	[UD1] [varchar](200) NULL,\n" +
   "	[UD2] [varchar](200) NULL,\n" +
   "	[UD3] [varchar](200) NULL,\n" +
   "	[UD4] [varchar](200) NULL,\n" +
   "	[UD5] [varchar](200) NULL,\n" +
   "	[Account_ID] [varchar](25) NULL,\n" +
   "	[userid] [varchar](200) NULL,\n" +
   "	[comment] [varchar](200) NULL,\n" +
   "	[MatchRef] [varchar](200) NULL,\n" +
   "	[MatchKeyWord] [varchar](50) NULL,\n" +
   "	[Tran_ID] [int] IDENTITY(1,1) NOT NULL\n" +
   ")";
            }
        }

    }

   
}
