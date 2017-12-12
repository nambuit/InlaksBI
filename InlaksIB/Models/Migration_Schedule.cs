using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InlaksIB
{
    public class Migration_Schedule
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string Application_Name { get; set; }

        [Required]

        public string DateFilter { get; set; }

        [Display(Name = "Cutt off Date")]
        public string MaxDate { get; set; }

        public string Time { get; set; }

        public string Fields { get; set; }

        [Required]
        [Display(Name = "Frequency")]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than {0}")]
        public int Interval { get; set; }

        [Display(Name = "Frequency Details")]
        public String IntervalDescription { get; set; }

        [Display(Name = "Weekends Inclusive")]
        public bool AllowWeekends { get; set; }

        public int FetchSize { get; set; }

        public int Lastposition { get; set; }

        public string NextRunDate { get; set; }

        public bool Status { get; set; }

    }

}