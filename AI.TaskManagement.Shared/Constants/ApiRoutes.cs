namespace AI.TaskManagement.Shared.Constants;

public static class ApiRoutes
{
    public const string BaseRoute = "api/v1";

    public static class Auth
    {
        public const string Base = $"{BaseRoute}/auth";
        public const string Register = $"{Base}/register";
        public const string Login = $"{Base}/login";
        public const string RefreshToken = $"{Base}/refresh-token";
    }

    public static class Tasks
    {
        public const string Base = $"{BaseRoute}/tasks";
        public const string GetAll = Base;
        public const string GetById = $"{Base}/{{id}}";
        public const string Create = Base;
        public const string Update = $"{Base}/{{id}}";
        public const string Delete = $"{Base}/{{id}}";
        public const string Assign = $"{Base}/{{id}}/assign";
    }

    public static class Comments
    {
        public const string Base = $"{BaseRoute}/comments";
        public const string GetByTask = $"{Base}/task/{{taskId}}";
        public const string Create = Base;
        public const string Delete = $"{Base}/{{id}}";
    }

    public static class Notifications
    {
        public const string Base = $"{BaseRoute}/notifications";
        public const string GetAll = Base;
        public const string MarkAsRead = $"{Base}/{{id}}/mark-as-read";
    }

    public static class Users
    {
        public const string Base = $"{BaseRoute}/users";
        public const string GetAll = Base;
        public const string GetById = $"{Base}/{{id}}";
        public const string GetProfile = $"{Base}/profile";
    }
}
