using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Department
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Department name is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Department name must be between 3 and 50 characters")]
        public string Name { get; set; }

        [ValidateNever]
        List<Employee> Employees { get; set; } = new List<Employee>();

    }
}
