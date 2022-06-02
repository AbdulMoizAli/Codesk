using System.ComponentModel.DataAnnotations;

namespace CodeskWeb.Areas.WorkSpace.Models
{
    public class JoinSessionViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter your name")]
        [RegularExpression("^[^,]*$", ErrorMessage = "Using a comma in your name is not valid")]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter session key")]
        public string SessionKey { get; set; }
    }
}