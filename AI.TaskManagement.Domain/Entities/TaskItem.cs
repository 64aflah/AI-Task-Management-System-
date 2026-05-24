using AI.TaskManagement.Domain.Enums;

namespace AI.TaskManagement.Domain.Entities;

public class TaskItem : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public TaskStatus Status { get; set; } = TaskStatus.NotStarted;
    public DateTime? DueDate { get; set; }
    public Guid? AssignedUserId { get; set; }
    public Guid CreatedByUserId { get; set; }

    // Navigation properties
    public User? CreatedByUser { get; set; }
    public User? AssignedUser { get; set; }
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
