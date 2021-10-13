namespace CodeskWeb.HubModels
{
    public static class NotificationMessages
    {
        public static string GetWelcomeMessage(string userName)
        {
            return $"<span>Welcome to CODESK <span class=\"yellow-text\"> {userName}</span></span>";
        }

        public static string GetUserJoinMessage(string userName)
        {
            return $"<span><span class=\"yellow-text\"> {userName}</span> just joined the session</span>";
        }
    }
}