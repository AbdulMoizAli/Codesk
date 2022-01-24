namespace CodeskLibrary.Models
{
    public class SessionFile
    {
        public int FileId { get; set; }

        public int SessionId { get; set; }

        public string FilePath { get; set; }

        public int FileTypeId { get; set; }

        public string FileTitle { get; set; }

        public FileType SessionFileType { get; set; }
    }
}