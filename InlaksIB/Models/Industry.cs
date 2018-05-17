using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InlaksIB
{
    public class Industry
    {


        [Key]
        public string IndustryID { get; set; }

        public string IndustryName { get; set; }

        public virtual List<Module> Modules { get; set; }

        public string AccessFlag { get; set; }

    }

}