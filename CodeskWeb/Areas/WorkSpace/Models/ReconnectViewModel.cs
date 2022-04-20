using CodeskWeb.HubModels;

namespace CodeskWeb.Areas.WorkSpace.Models
{
    public class ReconnectViewModel
    {
        public bool IsHost { get; set; }

        public string SessionKey { get; set; }

        public string PreviousUserId { get; set; }

        public ConnectedUser User { get; set; }
    }
}