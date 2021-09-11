using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CodeskWeb.Areas.Users.Models
{
    public class SignUpViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter your first name")]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "First name can only have letters")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Minimum 2 letters, Maximum 50 letters")]
        public string FirstName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter your last name")]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Last name can only have letters")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Minimum 2 letters, Maximum 50 letters")]
        public string LastName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a user name")]
        [RegularExpression("^[a-zA-Z0-9_.-]+$", ErrorMessage = "User name can only have letters, digits, '_', '.' and '-'")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Minimum 5 characters, Maximum 50 characters")]
        [Remote("ValidateUserName", "Account", "Users", ErrorMessage = "Username is already taken")]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter an email address")]
        [EmailAddress(ErrorMessage = "Email address is not valid")]
        [MaxLength(256, ErrorMessage = "Email address is too long, minimum 256 characters")]
        [Remote("ValidateEmailAddress", "Account", "Users", ErrorMessage = "Email address is already taken")]
        public string EmailAddress { get; set; }

        [Compare("EmailAddress", ErrorMessage = "Confirm email address does not match")]
        [DataType(DataType.EmailAddress)]
        public string ConfirmEmailAddress { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a password")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Minimum 8 characters, Maximum 100 characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Confirm password does not match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}