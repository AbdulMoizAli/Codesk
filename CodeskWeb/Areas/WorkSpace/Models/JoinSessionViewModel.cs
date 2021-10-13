﻿using System.ComponentModel.DataAnnotations;

namespace CodeskWeb.Areas.WorkSpace.Models
{
    public class JoinSessionViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter your name")]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter session key")]
        public string SessionKey { get; set; }
    }
}