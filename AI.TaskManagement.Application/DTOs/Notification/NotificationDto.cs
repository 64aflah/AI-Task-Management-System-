namespace AI.TaskManagement.Application.DTOs.Notification;

public class NotificationDto
{
    public Guid Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public int Type { get; set; }
    public Guid UserId { get; set; }
    public Guid? TaskId { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class MarkNotificationAsReadRequest
{
    public Guid NotificationId { get; set; }
}
