namespace CodeskWeb.ServiceModels
{
    public class CodeExecutionRequest
    {
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string Script { get; set; }

        public string Language { get; set; }

        public string VersionIndex { get; set; }
    }
}