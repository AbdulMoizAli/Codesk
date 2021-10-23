using System.Collections.Generic;
using System.Text;

namespace CodeskWeb.HubModels
{
    public static class SessionInformation
    {
        public static Dictionary<string, (StringBuilder code, List<ConnectedUser> connectedUsers)> SessionInfo { get; set; } = new();
    }
}