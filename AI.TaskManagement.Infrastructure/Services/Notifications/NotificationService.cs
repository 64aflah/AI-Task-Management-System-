namespace AI.TaskManagement.Infrastructure.Services.Notifications;

public interface INotificationService
{
    Task NotifyUserAsync(Guid userId, string message);
    Task NotifyMultipleUsersAsync(IEnumerable<Guid> userIds, string message);
    Task BroadcastNotificationAsync(string message);
}

public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyUserAsync(Guid userId, string message)
    {
        await _hubContext.Clients.User(userId.ToString())
            .SendAsync("ReceiveNotification", message);
    }

    public async Task NotifyMultipleUsersAsync(IEnumerable<Guid> userIds, string message)
    {
        foreach (var userId in userIds)
        {
            await NotifyUserAsync(userId, message);
        }
    }

    public async Task BroadcastNotificationAsync(string message)
    {
        await _hubContext.Clients.All
            .SendAsync("ReceiveNotification", message);
    }
}
