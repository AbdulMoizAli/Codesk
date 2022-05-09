namespace CodeskLibrary.Models
{
    public class TaskSubmission
    {
        public string ParticipantName { get; set; }

        public string FilePath { get; set; }

        public bool HasTurnedIn => FilePath is not null;
    }
}