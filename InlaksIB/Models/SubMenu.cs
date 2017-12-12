using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InlaksIB
{
    public class SubMenu
    {
        public string SubMenuID { get; set; }
        public string SubMenuName { get; set; }
        
        public virtual Resource ResourceItem { get; set; }

        public string Url { get; set; }

        public virtual List<SubSubMenu> SubsubMenus { get; set; }

        public string value { get; set; }
    }

}