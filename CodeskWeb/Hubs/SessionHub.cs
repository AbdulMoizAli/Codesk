using CodeskLibrary.DataAccess;
using CodeskWeb.HubModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CodeskWeb.Hubs
{
    public class SessionHub : Hub<ISessionClient>
    {
        private readonly IConfiguration _configuration;

        public SessionHub(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Authorize]
        public async Task CreateSession()
        {
            var sessionKey = Guid.NewGuid().ToString();
            var userName = Context.User.Identity.Name;

            var user = new ConnectedUser { UserId = Context.ConnectionId, UserName = userName, HasWriteAccess = true };

            SessionInformation.SessionInfo.Add(sessionKey, (_configuration["CodeExecution:Default"], new StringBuilder(), Context.ConnectionId, new List<ConnectedUser> { user }));

            await Groups.AddToGroupAsync(Context.ConnectionId, sessionKey)
                .ConfigureAwait(false);

            await Clients.Caller.ReceiveNewSessionInfo(user, sessionKey)
                .ConfigureAwait(false);

            await Clients.Caller.NotifyUser(NotificationMessage.GetWelcomeMessage(userName))
                .ConfigureAwait(false);
        }

        [Authorize]
        public async Task EndSessionForAll(string endDateTime, string sessionKey)
        {
            if (!SessionHelper.IsValidHostId(Context.ConnectionId, sessionKey))
                return;

            await Clients.OthersInGroup(sessionKey).EndSession()
                .ConfigureAwait(false);

            var email = Context.User.GetEmailAddress();

            await SessionManager.EndSession(email, endDateTime, sessionKey).ConfigureAwait(false);

            SessionInformation.SessionInfo.Remove(sessionKey);
        }

        [Authorize]
        public async Task ToggleWriteAccess(string sessionKey, string userId)
        {
            if (!SessionHelper.IsValidHostId(Context.ConnectionId, sessionKey))
                return;

            var user = SessionInformation.SessionInfo[sessionKey].connectedUsers.Find(u => u.UserId == userId);
            user.HasWriteAccess = !user.HasWriteAccess;

            await Clients.Client(userId).ToggleEditorReadOnly(!user.HasWriteAccess).ConfigureAwait(false);
        }

        [Authorize]
        public async Task SetCodingLanguage(string sessionKey, string language)
        {
            if (!SessionHelper.IsValidHostId(Context.ConnectionId, sessionKey))
                return;

            var excludedParticipants = SessionHelper.GetPrivateModeParticipants(sessionKey);
            excludedParticipants.Add(Context.ConnectionId);

            await Clients.GroupExcept(sessionKey, excludedParticipants).SetEditorLanguage(language)
                .ConfigureAwait(false);
        }

        public async Task JoinSession(string userName, string sessionKey)
        {
            var user = new ConnectedUser { UserId = Context.ConnectionId, UserName = userName };

            SessionInformation.SessionInfo[sessionKey].connectedUsers.Insert(0, user);

            var users = SessionInformation.SessionInfo[sessionKey].connectedUsers;

            await Groups.AddToGroupAsync(Context.ConnectionId, sessionKey)
                .ConfigureAwait(false);

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
            string userName = SessionInformation.SessionInfo[sessionKey].connectedUsers.Find(user => user.UserId == Context.ConnectionId).UserName;

            await Clients.OthersInGroup(sessionKey).ReceiveMessage(message, userName)
                .ConfigureAwait(false);

            await Clients.OthersInGroup(sessionKey).NotifyUser(NotificationMessage.GetChatMessageNotification(userName))
                .ConfigureAwait(false);
        }

        public async Task StartedMessageTyping(string sessionKey, string userName)
        {
            await Clients.OthersInGroup(sessionKey).StartMessageTypingIndication(userName)
                .ConfigureAwait(false);
        }

        public async Task StoppedMessageTyping(string sessionKey)
        {
            await Clients.OthersInGroup(sessionKey).StopMessageTypingIndication()
                .ConfigureAwait(false);
        }

        public async Task SendEditorContent(string editorContent, string sessionKey)
        {
            if (!SessionInformation.SessionInfo[sessionKey].connectedUsers.Find(u => u.UserId == Context.ConnectionId).HasWriteAccess)
                return;

            var excludedParticipants = SessionHelper.GetPrivateModeParticipants(sessionKey);
            excludedParticipants.Add(Context.ConnectionId);

            await Clients.GroupExcept(sessionKey, excludedParticipants).ReceiveEditorContent(editorContent)
                .ConfigureAwait(false);
        }

        public async Task StartedTyping(string sessionKey)
        {
            var excludedParticipants = SessionHelper.GetPrivateModeParticipants(sessionKey);
            excludedParticipants.Add(Context.ConnectionId);

            await Clients.GroupExcept(sessionKey, excludedParticipants).StartTypingIndication(Context.ConnectionId)
                .ConfigureAwait(false);
        }

        public async Task StoppedTyping(string sessionKey)
        {
            var excludedParticipants = SessionHelper.GetPrivateModeParticipants(sessionKey);
            excludedParticipants.Add(Context.ConnectionId);

            await Clients.GroupExcept(sessionKey, excludedParticipants).StopTypingIndication(Context.ConnectionId)
                .ConfigureAwait(false);
        }

        public async Task SendPeerId(string peerId, string sessionKey)
        {
            await Clients.OthersInGroup(sessionKey).ReceivePeerId(peerId, Context.ConnectionId)
                .ConfigureAwait(false);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            foreach (var item in SessionInformation.SessionInfo)
            {
                int index = item.Value.connectedUsers.FindIndex(x => x.UserId == Context.ConnectionId);

                if (index != -1)
                {
                    var user = item.Value.connectedUsers[index];

                    item.Value.connectedUsers.RemoveAt(index);

                    if (item.Value.connectedUsers.Count == 0)
                    {
                        SessionInformation.SessionInfo.Remove(item.Key);
                    }
                    else
                    {
                        await Groups.RemoveFromGroupAsync(Context.ConnectionId, item.Key)
                        .ConfigureAwait(false);

                        await Clients.OthersInGroup(item.Key).RemoveUser(user)
                            .ConfigureAwait(false);

                        await Clients.OthersInGroup(item.Key).CloseVideoCall(Context.ConnectionId)
                            .ConfigureAwait(false);

                        await Clients.OthersInGroup(item.Key).NotifyUser(NotificationMessage.GetUserLeaveMessage(user.UserName))
                            .ConfigureAwait(false);
                    }

                    break;
                }
            }

            await base.OnDisconnectedAsync(exception)
                .ConfigureAwait(false);
        }
    }
}