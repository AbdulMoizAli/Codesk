using CodeskWeb.ServiceModels;
using System.Net.Http;
using System.Text.Json;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CodeskWeb.Services
{
    public class CodeExecutionService : ICodeExecutionService
    {
        private readonly HttpClient _httpClient;

        public CodeExecutionService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CodeExecutionResponse> ExecuteCode(CodeExecutionRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("/execute", request).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                return null;

            var data = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<CodeExecutionResponse>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}