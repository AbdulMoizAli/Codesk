using System.Collections.Generic;

namespace CodeskWeb.HubModels
{
    public static class SessionInformation
    {
        public static Dictionary<string, List<string>> SessionInfo { get; set; } = new Dictionary<string, List<string>>();
    }
}