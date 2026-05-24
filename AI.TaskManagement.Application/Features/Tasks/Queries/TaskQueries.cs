using MediatR;
using AI.TaskManagement.Application.DTOs.Task;
using AI.TaskManagement.Shared.DTOs;
using AI.TaskManagement.Domain.Enums;

namespace AI.TaskManagement.Application.Features.Tasks.Queries;

public class GetAllTasksQuery : IRequest<PaginatedResult<TaskDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public Guid? AssignedUserId { get; set; }
    public TaskStatus? Status { get; set; }
    public TaskPriority? Priority { get; set; }
    public string? SearchTerm { get; set; }
}

public class GetTaskByIdQuery : IRequest<TaskDto>
{
    public Guid Id { get; set; }

    public GetTaskByIdQuery(Guid id)
    {
        Id = id;
    }
}

public class GetUserTasksQuery : IRequest<PaginatedResult<TaskDto>>
{
    public Guid UserId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public GetUserTasksQuery(Guid userId)
    {
        UserId = userId;
    }
}
