using AI.TaskManagement.Domain.Enums;

namespace AI.TaskManagement.Domain.Entities;

public class Notification : BaseEntity
{
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public Guid UserId { get; set; }
    public Guid? TaskId { get; set; }
    public bool IsRead { get; set; } = false;

    // Navigation properties
    public User? User { get; set; }
    public TaskItem? Task { get; set; }
}
