using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace CodeskWeb.HubModels
{
    public static class SessionHelper
    {
        public static bool IsValidConnectionId(string connectionId, string sessionKey)
        {
            return string.Equals(connectionId, SessionInformation.SessionInfo[sessionKey].hostId);
        }

        public static List<string> GetPrivateModeParticipants(string sessionKey)
        {
            return SessionInformation.SessionInfo[sessionKey]
                .connectedUsers.Where(u => u.IsPrivateMode).Select(u => u.UserId).ToList();
        }

        public static string GetEmailAddress(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirst(x => x.Type == ClaimTypes.Email).Value;
        }
    }
}