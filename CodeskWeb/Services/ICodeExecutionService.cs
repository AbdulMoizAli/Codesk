using CodeskWeb.ServiceModels;
using System.Threading.Tasks;

namespace CodeskWeb.Services
{
    public interface ICodeExecutionService
    {
        Task<CodeExecutionResponse> ExecuteCode(CodeExecutionRequest request);
    }
}