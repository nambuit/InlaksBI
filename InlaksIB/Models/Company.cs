using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InlaksIB
{
    public class Company
    {


        [Key]
        public string CompanyCode { get; set; }


        public string branchcode { get; set; }

        public string leadcompcode { get; set; }

        public string CompanyName { get; set; }

        

    }

}