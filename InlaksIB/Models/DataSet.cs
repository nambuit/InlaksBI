using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InlaksIB
{
    public class DataSetDetail
    {
        public virtual Module Module { get; set; }

        public string DataSetName { get; set; }

        public string Script { get; set; }
        [Key]
        public int ID { get; set; }

    }


    public struct DatasetObject
    {
        public string DataSetName;

        public string TableName;

        public string[] Columns;

        public string PreTable;

        public string NxtTable;

        
    }


}