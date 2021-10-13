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

            SessionInformation.SessionInfo.Add(sessionKey, new List<string> { userName });

            await Groups.AddToGroupAsync(Context.ConnectionId, sessionKey)
                .ConfigureAwait(false);

            await Clients.Caller.ReceiveNewSessionInfo(userName, sessionKey)
                .ConfigureAwait(false);

            await Clients.Caller.NotifyUser(NotificationMessages.GetWelcomeMessage(userName))
                .ConfigureAwait(false);
        }

        public async Task JoinSession(string userName, string sessionKey)
        {
            SessionInformation.SessionInfo[sessionKey].Add(userName);

            await Groups.AddToGroupAsync(Context.ConnectionId, sessionKey)
                .ConfigureAwait(false);

            var userNames = SessionInformation.SessionInfo[sessionKey];

            await Clients.Caller.ReceiveJoinSessionInfo(userNames)
                .ConfigureAwait(false);

            await Clients.OthersInGroup(sessionKey).AddNewUserName(userName)
                .ConfigureAwait(false);

            await Clients.Caller.NotifyUser(NotificationMessages.GetWelcomeMessage(userName))
                .ConfigureAwait(false);

            await Clients.OthersInGroup(sessionKey).NotifyUser(NotificationMessages.GetUserJoinMessage(userName))
                .ConfigureAwait(false);
        }
    }
}