using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Data;

namespace WebApplication1.Models
{
    [Table("Employees")]
    public class Employee
    {
        [Key]
        public int Employee_Id { get; set; }

        [Required(ErrorMessage ="Employee's name is required")]
        [StringLength(35, MinimumLength = 3, ErrorMessage = "Employee's name must be between 3 and 35 characters")]
        [DisplayName("Name")]
        public string Employee_Name { get; set; }

        [Range(290,10000, ErrorMessage = "Salary must be between 290 and 10000")]
        [Required]
        [Precision(7,2)]
        [DisplayName("Salary")]
        public double salary { get; set; }


        public DateTime hireDate { get; set; }

        public int ?DepartmentId { get; set; }

        [ForeignKey("DepartmentId")]
        [ValidateNever]
        public Department department { get; set; }

        [DefaultValue(true)]
        [Required]
        public bool Is_Active { get; set; }

        public string? UserID { get; set; }

        [ForeignKey(nameof(UserID))]
        public AppIdentityUser? User { get; set; }


    }
}
