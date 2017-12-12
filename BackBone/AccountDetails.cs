using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace BackBone
{
   public  class AccountDetails
    {
        public DateTime LastLedgerDate { get; set; }
        public DateTime LastStmtDate { get; set; }
        public DateTime CurrentDate { get; set; }

        public string AccountCode { get; set; }


        public string AccountName { get; set; }
        public string DefinitionID { get; set; }
        public List<string> CBAccount { get; set; }
        public List<string> StmtCBAccount { get; set; }
        public decimal Balance { get; set; }

        public decimal stmtBalmce { get; set; }

        public string LedgerLastID { get; set; }

        public string StmtLastID { get; set; }

        public string Endate { get; set; }

        public string DomainName { get; set; }

        public string CurrentUser { get; set; }

        public string AffiliateName { get; set; }

        public string Acct_Active { get; set; }

        public bool isSuspense   { get; set; }

        public string CA_ACCTNO { get; set; }

        public string FOREIGN_NO { get; set; }

        public string SHORTNAME { get; set; }

        public string currency { get; set; }

        public string CbaBranchCode { get; set; }

        public string InternalRecord { get; set; }

        public string ExternalRecod { get; set; }

        public decimal Ledger_Creation_Balance { get; set; }

        public decimal Statement_Creation_Balance { get; set; }

    }
}
