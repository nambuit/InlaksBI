using InlaksIB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace InlaksIB
{
    public class ResourceReport
    {

        public virtual Resource Resource { get; set; }

        public string pivotConfig { get; set; }


        public string dataSet { get; set; }

        [Display(Name ="DataSet Filters")]
        public virtual List<DataSetFilter> Filters {get;set;}
        
        [Key]
        public int ReportID { get; set; }

        [NotMapped]
        [Display(Name ="ModuleName")]
        public string ModuleID { get; set; }

     
        public string InstanceID { get; set; }

        [Display(Name = "Report Type")]
        public string report_type { get; set; }

    }

    public class ReportModel
    {

        public int ResourcID  { get; set; }

        public string pivotConfig { get; set; }

        public string UserConfig { get; set; }

        public string dataSet { get; set; }

        public string report_type { get; set; }

        public  List<DataSetFilterModel> Filters { get; set; }
        public int ReportID { get; set; }

        public string ModuleID { get; set; }

        public string InstanceID { get; set; }

    }

}