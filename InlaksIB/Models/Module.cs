using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace InlaksIB
{
    public class Module
    {
        public string ModuleName { get; set; }
        [Key]
        public int ModuleID { get; set; }

        public virtual Industry Industry { get; set; }
        public virtual List<Resource> Resources { get; set; }

        public string value { get; set; }

        public string IconClass { get; set; }

        [NotMapped]
        [Display(Name = "Select Industry")]
        public string IndustryID { get; set; }

    }
}