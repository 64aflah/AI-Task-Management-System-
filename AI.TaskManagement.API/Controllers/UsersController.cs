using AI.TaskManagement.Application.DTOs.User;
using AI.TaskManagement.Application.Features.Users.Queries;
using AI.TaskManagement.Shared.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AI.TaskManagement.API.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetAllUsersQuery { PageNumber = pageNumber, PageSize = pageSize };
        var result = await _mediator.Send(query);
        return Ok(ApiResponse<object>.SuccessResponse(result, "Users retrieved successfully"));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetById(Guid id)
    {
        var query = new GetUserByIdQuery(id);
        var result = await _mediator.Send(query);
        return Ok(ApiResponse<UserDto>.SuccessResponse(result, "User retrieved successfully"));
    }

    [HttpGet("profile/current")]
    public async Task<ActionResult<ApiResponse<GetProfileResponse>>> GetProfile()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
        var query = new GetUserProfileQuery(userId);
        var result = await _mediator.Send(query);
        return Ok(ApiResponse<GetProfileResponse>.SuccessResponse(result, "Profile retrieved successfully"));
    }
}
