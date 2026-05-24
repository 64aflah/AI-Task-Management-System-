using MediatR;
using AI.TaskManagement.Application.DTOs.Comment;
using AI.TaskManagement.Shared.DTOs;

namespace AI.TaskManagement.Application.Features.Comments.Queries;

public class GetCommentsByTaskQuery : IRequest<PaginatedResult<CommentDto>>
{
    public Guid TaskId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public GetCommentsByTaskQuery(Guid taskId)
    {
        TaskId = taskId;
    }
}

public class GetCommentByIdQuery : IRequest<CommentDto>
{
    public Guid Id { get; set; }

    public GetCommentByIdQuery(Guid id)
    {
        Id = id;
    }
}
