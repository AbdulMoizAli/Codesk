using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeskWeb.Hubs
{
    public interface ISessionClient
    {
        Task ReceiveNewSessionInfo(string userName, string sessionKey);

        Task ReceiveJoinSessionInfo(List<string> userNames);

        Task AddNewUserName(string userName);

        Task NotifyUser(string notification);
    }
}