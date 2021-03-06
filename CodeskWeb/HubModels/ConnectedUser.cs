namespace CodeskWeb.HubModels
{
    public class ConnectedUser
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public bool HasWriteAccess { get; set; }

        public bool IsPrivateMode { get; set; }
    }
}