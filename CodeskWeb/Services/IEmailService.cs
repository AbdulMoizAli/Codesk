using CodeskWeb.ServiceModels;
using System.Threading.Tasks;

namespace CodeskWeb.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmail(EmailRequest request);
    }
}