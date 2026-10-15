using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.ViewModels
{
    public class RegisterView
    {
        [Required(ErrorMessage ="Please enter your email address")]
        [EmailAddress(ErrorMessage ="Please enter a valid email address")]
        public string? email { get; set; }

        [Required(ErrorMessage = "Please enter your password")]
        [DataType(DataType.Password, ErrorMessage = "Please enter a valid password")]
        public string? password { get; set; }

        [Required(ErrorMessage = "Please confirm your password")]
        [DataType(DataType.Password, ErrorMessage = "Please enter a valid password")]
        [Compare("password", ErrorMessage ="Password and password confirmation don't match")]
        public string? passwordConfirmation { get; set; }

        [Required(ErrorMessage = "Please enter your first name")]
        [StringLength(30, ErrorMessage = "First name cannot be longer than 30 characters")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Please enter your last name")]
        [StringLength(30, ErrorMessage = "Last name cannot be longer than 30 characters")]
        public string? LastName { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Please enter a valid date")]
        public DateTime? DateOfBirth { get; set; }

    }
}
