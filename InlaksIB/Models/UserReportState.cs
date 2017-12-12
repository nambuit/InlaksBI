using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InlaksIB
{
    public class UserReportState
    {
        public virtual User User { get; set; }

        public int ReportID { get; set; }

        public string ConfigData { get; set; }

        [Key]
        public int ID { get; set; }

        public string InstanceID { get; set; }
    }
}