namespace CodeskLibrary.Models
{
    public class ParticipantTaskSubmission
    {
        public int SubmissionId { get; set; }

        public int TaskId { get; set; }

        public int ParticipantId { get; set; }

        public string FilePath { get; set; }
    }
}