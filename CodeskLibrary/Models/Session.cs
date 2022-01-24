namespace CodeskLibrary.Models
{
    public class Session
    {
        public int SessionId { get; set; }

        public string Title { get; set; }

        public string StartedAt { get; set; }

        public string EndedAt { get; set; }

        public int ParticipantCount { get; set; }

        public int FileCount { get; set; }

        public string CoverImage { get; set; }

        public string FileIds { get; set; }

        public string FileTitles { get; set; }

        public string Participants { get; set; }
    }
}