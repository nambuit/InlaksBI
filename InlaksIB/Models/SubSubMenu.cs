using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InlaksIB
{
    public class SubSubMenu
    {
        public string SubSubMenuName { get; set; }

        [Key]
        public string SubSubMenuID { get; set; }


        public virtual SubMenu SubMenu { get; set; }

        public string Url { get; set; }

        public virtual List<SubSubSubMenu> SubSubSubMenus { get; set; }

        public string value { get; set; }
    }
}