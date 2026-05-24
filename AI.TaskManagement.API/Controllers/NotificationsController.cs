using AI.TaskManagement.Application.DTOs.Notification;
using AI.TaskManagement.Application.Features.Notifications.Queries;
using AI.TaskManagement.Shared.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AI.TaskManagement.API.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> GetUserNotifications([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
        var query = new GetUserNotificationsQuery(userId) { PageNumber = pageNumber, PageSize = pageSize };
        var result = await _mediator.Send(query);
        return Ok(ApiResponse<object>.SuccessResponse(result, "Notifications retrieved successfully"));
    }
}
