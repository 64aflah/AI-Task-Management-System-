namespace AI.TaskManagement.Domain.Entities;

public class Comment : BaseEntity
{
    public string Content { get; set; } = string.Empty;
    public Guid TaskId { get; set; }
    public Guid UserId { get; set; }

    // Navigation properties
    public TaskItem? Task { get; set; }
    public User? User { get; set; }
}
