using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EmployeeDetailMsys.Models
{
    public class EmployeeM
    {
        [Key]
       public int ID { get; set; }

        [Required]
        public string FullName { get; set; }
        [Required]
        public string DateOfBirth { get; set; }
        [Required]
        public string Gender { get; set; }
   
        [Required]
        public decimal Salary { get; set; }

        [Required]
        public string Designation { get; set; }
        public string importdate { get; set; }

        [Required (ErrorMessage ="Upload Profile Image")]
        public string Uimg { get; set; }
    }
}