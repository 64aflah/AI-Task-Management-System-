using AI.TaskManagement.Application.DTOs.Comment;
using AI.TaskManagement.Application.Features.Comments.Commands;
using AI.TaskManagement.Application.Features.Comments.Queries;
using AI.TaskManagement.Shared.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AI.TaskManagement.API.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CommentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("task/{taskId}")]
    public async Task<ActionResult<ApiResponse<object>>> GetByTask(Guid taskId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetCommentsByTaskQuery(taskId) { PageNumber = pageNumber, PageSize = pageSize };
        var result = await _mediator.Send(query);
        return Ok(ApiResponse<object>.SuccessResponse(result, "Comments retrieved successfully"));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<CommentDto>>> GetById(Guid id)
    {
        var query = new GetCommentByIdQuery(id);
        var result = await _mediator.Send(query);
        return Ok(ApiResponse<CommentDto>.SuccessResponse(result, "Comment retrieved successfully"));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<CommentDto>>> Create([FromBody] CreateCommentRequest request)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
        var command = new CreateCommentCommand(request.Content, request.TaskId, userId);
        var result = await _mediator.Send(command);
        return Created(nameof(GetById), ApiResponse<CommentDto>.SuccessResponse(result, "Comment created successfully"));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<CommentDto>>> Update(Guid id, [FromBody] UpdateCommentRequest request)
    {
        var command = new UpdateCommentCommand(id, request.Content);
        var result = await _mediator.Send(command);
        return Ok(ApiResponse<CommentDto>.SuccessResponse(result, "Comment updated successfully"));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> Delete(Guid id)
    {
        var command = new DeleteCommentCommand(id);
        await _mediator.Send(command);
        return Ok(ApiResponse.SuccessResponse("Comment deleted successfully"));
    }
}
