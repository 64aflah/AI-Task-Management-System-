using MediatR;
using AI.TaskManagement.Application.DTOs.Task;
using AI.TaskManagement.Domain.Enums;

namespace AI.TaskManagement.Application.Features.Tasks.Commands;

public class CreateTaskCommand : IRequest<TaskDto>
{
    public string Title { get; set; }
    public string Description { get; set; }
    public TaskPriority Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public Guid? AssignedUserId { get; set; }
    public Guid CreatedByUserId { get; set; }

    public CreateTaskCommand(string title, string description, TaskPriority priority, 
        DateTime? dueDate, Guid? assignedUserId, Guid createdByUserId)
    {
        Title = title;
        Description = description;
        Priority = priority;
        DueDate = dueDate;
        AssignedUserId = assignedUserId;
        CreatedByUserId = createdByUserId;
    }
}

public class UpdateTaskCommand : IRequest<TaskDto>
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public TaskPriority Priority { get; set; }
    public TaskStatus Status { get; set; }
    public DateTime? DueDate { get; set; }
    public Guid? AssignedUserId { get; set; }

    public UpdateTaskCommand(Guid id, string title, string description, TaskPriority priority,
        TaskStatus status, DateTime? dueDate, Guid? assignedUserId)
    {
        Id = id;
        Title = title;
        Description = description;
        Priority = priority;
        Status = status;
        DueDate = dueDate;
        AssignedUserId = assignedUserId;
    }
}

public class DeleteTaskCommand : IRequest<bool>
{
    public Guid Id { get; set; }

    public DeleteTaskCommand(Guid id)
    {
        Id = id;
    }
}

public class AssignTaskCommand : IRequest<TaskDto>
{
    public Guid TaskId { get; set; }
    public Guid AssignedUserId { get; set; }

    public AssignTaskCommand(Guid taskId, Guid assignedUserId)
    {
        TaskId = taskId;
        AssignedUserId = assignedUserId;
    }
}
