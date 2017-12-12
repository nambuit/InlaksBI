using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace InlaksIB
{
    public class Resource
    {
        [Key]
        public int ResourceID { get; set; }
        [Required]
        public string ResourceName { get; set; }


        public virtual Module Module { get; set; }

        [NotMapped]
        public int ModuleID { get; set; }
        public virtual List<SubMenu> SubMenus { get; set; }
       
        public string Url { get; set; }

        [Required]
        public string value { get; set; }


    }
}