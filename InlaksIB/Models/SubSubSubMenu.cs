using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InlaksIB
{
    public class SubSubSubMenu
    {
        public string SubSubSubMenuName { get; set; }

        [Key]
        public string SubSubSubMenuID { get; set; }

        public string Url { get; set; }


        public virtual SubSubMenu SubSubMenu { get; set; }

        public string id { get; set; }

        public string value { get; set; }


    }
}