using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InlaksIB
{

    public class Role
    {
        [Key]
        public int RoleID { get; set; }

        public string RoleName { get; set; }

        public virtual List<RoleResource> Resources { get; set; }
    }

}