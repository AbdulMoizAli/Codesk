namespace CodeskLibrary.Models
{
    public class SessionTask
    {
        public int TaskId { get; set; }

        public int SessionId { get; set; }

        public string TaskName { get; set; }

        public string TaskDescription { get; set; }
    }
}