
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmpDirectory.Models
{
    public class EmployeeModel
    {
        public string Id { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "Employee's first name is required.")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Employee's last name is required.")]
        public string LastName { get; set; }

        [Display(Name = "Employee ID")]
        [Required(ErrorMessage = "Employee must be assigned an ID")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "An Employee ID must be 8 numbers.")] //validate that ID is 8 characters long
        [RegularExpression(@"[0-9]*$", ErrorMessage = "Employee ID can only contain numbers.")]
        public string EmployeeID { get; set; }

        [Required(ErrorMessage = "Employee's department must be specified.")]
        public string Department { get; set; }

        [Required(ErrorMessage = "Everyone has a birthday come on...")]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy - MM - dd}", ApplyFormatInEditMode = true)]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "If you're unsure of  your gender, just pick one. This field is required")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "An option must be selected")]
        public string Race { get; set; }
        public List<SelectListItem> RaceOptions { get; set; }

        public bool FullTime { get; set; }

    }
}