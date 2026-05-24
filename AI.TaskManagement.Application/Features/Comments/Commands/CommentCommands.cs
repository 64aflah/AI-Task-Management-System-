using MediatR;
using AI.TaskManagement.Application.DTOs.Comment;

namespace AI.TaskManagement.Application.Features.Comments.Commands;

public class CreateCommentCommand : IRequest<CommentDto>
{
    public string Content { get; set; }
    public Guid TaskId { get; set; }
    public Guid UserId { get; set; }

    public CreateCommentCommand(string content, Guid taskId, Guid userId)
    {
        Content = content;
        TaskId = taskId;
        UserId = userId;
    }
}

public class UpdateCommentCommand : IRequest<CommentDto>
{
    public Guid Id { get; set; }
    public string Content { get; set; }

    public UpdateCommentCommand(Guid id, string content)
    {
        Id = id;
        Content = content;
    }
}

public class DeleteCommentCommand : IRequest<bool>
{
    public Guid Id { get; set; }

    public DeleteCommentCommand(Guid id)
    {
        Id = id;
    }
}
