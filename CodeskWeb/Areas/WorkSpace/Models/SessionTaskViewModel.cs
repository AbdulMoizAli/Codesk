using System.ComponentModel.DataAnnotations;

namespace CodeskWeb.Areas.WorkSpace.Models
{
    public class SessionTaskViewModel
    {
        public int TaskId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string TaskName { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string TaskDescription { get; set; }
    }
}