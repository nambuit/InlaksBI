using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InlaksIB
{
    public class DataSetFilter
    {
        public virtual ResourceReport Report { get; set; }

        public string ColumnName { get; set; }

        public string Operator { get; set; }

        public string ColumnValue { get; set; }
        
        public bool IsIncluded { get; set; }

        [Key]
        public int ID { get; set; }

    }

    public class DataSetFilterModel
    {
        public int  ReportID { get; set; }

        public string ColumnName { get; set; }

        public string ColumnValue { get; set; }

        public bool IsIncluded { get; set; }

        public string Operator { get; set; }

        public int ID { get; set; }

    }
}