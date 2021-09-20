﻿using System.ComponentModel.DataAnnotations;

namespace CodeskWeb.Areas.Users.Models
{
    public class ForgotPasswordViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter your email address")]
        [EmailAddress(ErrorMessage = "Email Address is invalid")]
        public string EmailAddress { get; set; }
    }
}