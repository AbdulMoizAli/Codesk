using CodeskWeb.HubModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeskWeb.Hubs
{
    public class SessionHub : Hub<ISessionClient>
    {
        [Authorize]
        public async Task CreateSession()
        {
            var sessionKey = Guid.NewGuid().ToString();
            var userName = Context.User.Identity.Name;

            var user = new ConnectedUser { UserId = Context.ConnectionId, UserName = userName };

            SessionInformation.SessionInfo.Add(sessionKey, new List<ConnectedUser> { user });

            await Groups.AddToGroupAsync(Context.ConnectionId, sessionKey)
                .ConfigureAwait(false);

            await Clients.Caller.ReceiveNewSessionInfo(user, sessionKey)
                .ConfigureAwait(false);

            await Clients.Caller.NotifyUser(NotificationMessage.GetWelcomeMessage(userName))
                .ConfigureAwait(false);
        }

        public async Task JoinSession(string userName, string sessionKey)
        {
            var user = new ConnectedUser { UserId = Context.ConnectionId, UserName = userName };

            SessionInformation.SessionInfo[sessionKey].Add(user);

            await Groups.AddToGroupAsync(Context.ConnectionId, sessionKey)
                .ConfigureAwait(false);

            var users = SessionInformation.SessionInfo[sessionKey];

            await Clients.Caller.ReceiveJoinSessionInfo(users)
                .ConfigureAwait(false);

            await Clients.OthersInGroup(sessionKey).AddUser(user)
                .ConfigureAwait(false);

            await Clients.Caller.NotifyUser(NotificationMessage.GetWelcomeMessage(userName))
                .ConfigureAwait(false);

            await Clients.OthersInGroup(sessionKey).NotifyUser(NotificationMessage.GetUserJoinMessage(userName))
                .ConfigureAwait(false);
        }

        public async Task SendMessage(string message, string sessionKey)
        {
            string userName = SessionInformation.SessionInfo[sessionKey].Find(user => user.UserId == Context.ConnectionId).UserName;

            await Clients.OthersInGroup(sessionKey).ReceiveMessage(message, userName)
                .ConfigureAwait(false);

            await Clients.OthersInGroup(sessionKey).NotifyUser(NotificationMessage.GetChatMessageNotification(userName))
                .ConfigureAwait(false);
        }

        public async Task SendEditorContent(string editorContent, string sessionKey)
        {
            await Clients.OthersInGroup(sessionKey).ReceiveEditorContent(editorContent)
                .ConfigureAwait(false);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            foreach (var item in SessionInformation.SessionInfo)
            {
                int index = item.Value.FindIndex(x => x.UserId == Context.ConnectionId);

                if (index != -1)
                {
                    var user = item.Value[index];

                    item.Value.RemoveAt(index);

                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, item.Key)
                        .ConfigureAwait(false);

                    await Clients.OthersInGroup(item.Key).RemoveUser(user)
                        .ConfigureAwait(false);

                    await Clients.OthersInGroup(item.Key).NotifyUser(NotificationMessage.GetUserLeaveMessage(user.UserName))
                        .ConfigureAwait(false);

                    break;
                }
            }

            await base.OnDisconnectedAsync(exception)
                .ConfigureAwait(false);
        }
    }
}