using System.ComponentModel.DataAnnotations;


namespace WebApplication1.Models.ViewModels
{
    public class LoginView
    {

        [Required(ErrorMessage = "Please enter your email address")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string? email { get; set; }

        [Required(ErrorMessage = "Please enter your password")]
        [DataType(DataType.Password, ErrorMessage = "Please enter a valid password")]
        public string? password { get; set; }

    }
}
