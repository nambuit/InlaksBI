using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackBone
{
    public class Transaction
    {
        public string Details { get; set; }
        public DateTime Postdate { get; set; }

        public DateTime Valdate { get; set; }

        public decimal Credit { get; set; }
        public decimal Debit { get; set; }
        public string ud1 { get; set; }
        public string ud2 { get; set; }
        public string ud3 { get; set; }
        public string ud4 { get; set; }
        public string ud5 { get; set; }
        public string user { get; set; }
        public string ID { get; set; }
        public string CrDr { get; set; }
        public int SN { get; set; }

       

      
    }


    public class MatchedItems
    {
        public string LDetails { get; set; }
        public string LPostdate { get; set; }

        public string LValdate { get; set; }

        public string LAmount { get; set; }

        public string LCrDr { get; set; }

        public string SCrDr { get; set; }
        public string SDetails { get; set; }
        public string  SPostdate { get; set; }

        public string SValdate { get; set; }

        public string SAmount { get; set; }

        public string userID { get; set; }



    }


    



    public class AcctSummary
    {
        public decimal Obalance { get; set; }
        public decimal Cbalance { get; set; }
        public decimal TotalCredits { get; set; }
        public decimal TotalDebits { get; set; }

    }


    public class ValuePair
    {

        public string ID { get; set; }
        public string Value { get; set; }

    }


    public class ConsolidatedSummary
    {
        public decimal TotalLedger { get; set; }

        public decimal TotalStmt { get; set; }

        public decimal TotalLedgerCredits { get; set; }

        public decimal TotalStmtCredits { get; set; }

        public decimal TotalLedgerDebits { get; set; }

        public decimal TotalStmtDebits { get; set; }
    }

    public class Transactions
    {
        public string Details { get; set; }
        public string Postdate { get; set; }

        public string Valdate { get; set; }


        public decimal Amount { get; set; }
        public string SerialNo { get; set; }
        public string Matches { get; set; }

        public string Side { get; set; }

        //public int PageCount { get; set; }

        //public int Total { get; set; }

        public string CrDr { get; set; }
        public string matchType { get; set; }
        public string colour { get; set; }

        public string noteIcon { get; set; }

        public string noteID { get; set; }

        public string TranRef { get; set; }

        public string ud1 { get; set; }

        public string ud2 { get; set; }

        public string ud3 { get; set; }

        public string ud4 { get; set; }

        public string ud5 { get; set; }

        public string userid { get; set; }

        public string MatchRef { get; set; }

        public string MatchKeyWord { get; set; }


        public string MatchID { get; set; }
        public string SN { get; set; }
    }


    public struct AccountStats
    {
        public int TotalItemsLedger;

        public int TotalItemsStmt;

        public int LMatched;

        public int LUnmatched;

        public int SMatched;

        public int SUnmatched;

        public int LTCredit;

        public int LTDebit;

        public int STCredit;

        public int STDebit;

        public decimal LTCreditA;

        public decimal LTDebitA;

        public decimal STCreditA;

        public decimal STDebitA;

        public string Currency;

    }


}
