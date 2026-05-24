using AI.TaskManagement.Application.DTOs.Task;
using AI.TaskManagement.Application.Features.Tasks.Commands;
using AI.TaskManagement.Application.Features.Tasks.Queries;
using AI.TaskManagement.Shared.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AI.TaskManagement.API.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class TasksController : ControllerBase
{
    private readonly IMediator _mediator;

    public TasksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetAllTasksQuery { PageNumber = pageNumber, PageSize = pageSize };
        var result = await _mediator.Send(query);
        return Ok(ApiResponse<object>.SuccessResponse(result, "Tasks retrieved successfully"));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<TaskDto>>> GetById(Guid id)
    {
        var query = new GetTaskByIdQuery(id);
        var result = await _mediator.Send(query);
        return Ok(ApiResponse<TaskDto>.SuccessResponse(result, "Task retrieved successfully"));
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<ApiResponse<object>>> GetUserTasks(Guid userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetUserTasksQuery(userId) { PageNumber = pageNumber, PageSize = pageSize };
        var result = await _mediator.Send(query);
        return Ok(ApiResponse<object>.SuccessResponse(result, "User tasks retrieved successfully"));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<TaskDto>>> Create([FromBody] CreateTaskRequest request)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
        var command = new CreateTaskCommand(request.Title, request.Description, request.Priority, 
            request.DueDate, request.AssignedUserId, userId);
        var result = await _mediator.Send(command);
        return Created(nameof(GetById), ApiResponse<TaskDto>.SuccessResponse(result, "Task created successfully"));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<TaskDto>>> Update(Guid id, [FromBody] UpdateTaskRequest request)
    {
        var command = new UpdateTaskCommand(id, request.Title, request.Description, request.Priority,
            request.Status, request.DueDate, request.AssignedUserId);
        var result = await _mediator.Send(command);
        return Ok(ApiResponse<TaskDto>.SuccessResponse(result, "Task updated successfully"));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> Delete(Guid id)
    {
        var command = new DeleteTaskCommand(id);
        await _mediator.Send(command);
        return Ok(ApiResponse.SuccessResponse("Task deleted successfully"));
    }

    [HttpPost("{id}/assign")]
    public async Task<ActionResult<ApiResponse<TaskDto>>> AssignTask(Guid id, [FromBody] AssignTaskRequest request)
    {
        var command = new AssignTaskCommand(id, request.AssignedUserId);
        var result = await _mediator.Send(command);
        return Ok(ApiResponse<TaskDto>.SuccessResponse(result, "Task assigned successfully"));
    }
}
