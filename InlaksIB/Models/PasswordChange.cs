using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InlaksIB
{
    public class PasswordChange
    {
        [Required]
        [Display(Name = "Current Password")]

        public string Password { get; set; }

        [Required]
        [Unlike(nameof(Password), ErrorMessage = "New and Old Password cannot be thesame")]
        [Display(Name = "New Password")]

        public string NewPassword { get; set; }

        [Required]
        [Compare(nameof(NewPassword), ErrorMessage = "New Passwords do not match.")]
        [Display(Name = "Re-Enter New Password")]
        public string RePassword { get; set; }
    }
}