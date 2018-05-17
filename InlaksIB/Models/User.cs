using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace InlaksIB
{
    public class User
    {
        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string UserID { get; set; }

        [Required]
        [Display(Name = "Password")]

        public string Password { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        public DateTime LastLogin { get; set; }

        public bool DefaultPassword { get; set; }



        [Required]
        [NotMapped]
        [Compare(nameof(Password), ErrorMessage = "Passwords don't match.")]
        [Display(Name = "Re-Enter Password")]
        public string RePassword { get; set; }


        [Required]
        [NotMapped]
        [Display(Name = "Select User Role")]
        public int RoleID { get; set; }



        [NotMapped]
        public string Message { get; set; }

        [NotMapped]
        public string errorclass { get; set; }


        public virtual Role UserRole { get; set; }

        [Display(Name = "Select Company")]
        [Required]
        public string LeadCompany { get; set; }

        [Display(Name = "Select Branch")]
        [Required]
        public string Branch { get; set; }


    }
}