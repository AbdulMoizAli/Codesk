namespace CodeskWeb.HubModels
{
    public static class NotificationMessage
    {
        public static string GetWelcomeMessage(string userName)
        {
            return $"<i class=\"material-icons left\">code</i><span>Welcome to CODESK <span class=\"yellow-text\"> {userName.ToUpper()}</span></span>";
        }

        public static string GetUserJoinMessage(string userName)
        {
            return $"<i class=\"material-icons left\">person_add</i><span><span class=\"yellow-text\"> {userName.ToUpper()}</span> just joined the session</span>";
        }

        public static string GetUserLeaveMessage(string userName)
        {
            return $"<i class=\"material-icons left\">error</i><span><span class=\"yellow-text\"> {userName.ToUpper()}</span> has left the session</span>";
        }

        public static string GetChatMessageNotification(string userName)
        {
            return $"<i class=\"material-icons left\">chat_bubble</i><span>New message from <span class=\"yellow-text\"> {userName.ToUpper()}</span></span>";
        }

        public static string GetNewTaskMessage()
        {
            return $"<i class=\"material-icons left\">assignment</i><span>You have a new task</span>";
        }
    }
}