using CodeskWeb.HubModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeskWeb.Hubs
{
    public interface ISessionClient
    {
        Task ReceiveNewSessionInfo(ConnectedUser user, string sessionKey);

        Task ReceiveJoinSessionInfo(List<ConnectedUser> users);

        Task AddUser(ConnectedUser user);

        Task RemoveUser(ConnectedUser user);

        Task NotifyUser(string notification);
    }
}