using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InlaksIB
{
    public class RoleResource
    {

        [Key]
        public int id { get; set; }
        public virtual Role Role { get; set; }

        public virtual Resource Resource { get; set; }
    }

}