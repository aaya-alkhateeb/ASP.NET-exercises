using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class AppIdentityUser : IdentityUser
    {
        [Required(ErrorMessage = "Please enter your first name")]
        [StringLength(30, ErrorMessage = "First name cannot be longer than 30 characters")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Please enter your last name")]
        [StringLength(30, ErrorMessage = "Last name cannot be longer than 30 characters")]
        public string? LastName { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Please enter a valid date")]
        public DateTime? DateOfBirth { get; set; }

        public Employee? Employee { get; set; }
    }
}
