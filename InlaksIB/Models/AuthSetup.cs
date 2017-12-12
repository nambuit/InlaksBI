using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InlaksIB
{
    public class AuthSetup
    {
        [Display(Name = "Active Directory Server")]

        public string Server { get; set; }


        [Display(Name = "Select Authentication Mode")]

        public string AuthType { get; set; }

        public int id { get; set; }
    }
}