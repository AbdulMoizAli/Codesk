namespace CodeskWeb.ServiceModels
{
    public class CodeExecutionResponse
    {
        public string Output { get; set; }

        public int StatusCode { get; set; }

        public string Memory { get; set; }

        public string CpuTime { get; set; }
    }
}