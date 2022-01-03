using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeskWeb.HubModels
{
    public interface ISessionClient
    {
        Task ReceiveNewSessionInfo(ConnectedUser user, string sessionKey);

        Task ReceiveJoinSessionInfo(List<ConnectedUser> users);

        Task AddUser(ConnectedUser user);

        Task RemoveUser(ConnectedUser user);

        Task NotifyUser(string notification);

        Task ReceiveMessage(string message, string userName);

        Task StartMessageTypingIndication(string userName);

        Task StopMessageTypingIndication();

        Task ReceiveEditorContent(string editorContent);

        Task StartTypingIndication(string userId);

        Task StopTypingIndication(string userId);

        Task ReceivePeerId(string peerId, string userId);

        Task CloseVideoCall(string userId);

        Task EndSession();

        Task ToggleEditorReadOnly(bool isReadOnly);
    }
}