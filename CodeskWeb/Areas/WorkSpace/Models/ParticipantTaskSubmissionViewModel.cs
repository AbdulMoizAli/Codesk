using System.ComponentModel.DataAnnotations;

namespace CodeskWeb.Areas.WorkSpace.Models
{
    public class ParticipantTaskSubmissionViewModel
    {
        [Range(1, int.MaxValue)]
        public int TaskId { get; set; }

        [Range(1, int.MaxValue)]
        public int ParticipantId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string SubmissionText { get; set; }
    }
}