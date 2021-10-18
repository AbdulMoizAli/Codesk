using System.Collections.Generic;

namespace CodeskWeb.HubModels
{
    public static class SessionInformation
    {
        public static Dictionary<string, List<ConnectedUser>> SessionInfo { get; set; } = new Dictionary<string, List<ConnectedUser>>();
    }
}