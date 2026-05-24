using MediatR;
using AI.TaskManagement.Application.DTOs.User;
using AI.TaskManagement.Shared.DTOs;

namespace AI.TaskManagement.Application.Features.Users.Queries;

public class GetAllUsersQuery : IRequest<PaginatedResult<UserDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetUserByIdQuery : IRequest<UserDto>
{
    public Guid Id { get; set; }

    public GetUserByIdQuery(Guid id)
    {
        Id = id;
    }
}

public class GetUserProfileQuery : IRequest<GetProfileResponse>
{
    public Guid UserId { get; set; }

    public GetUserProfileQuery(Guid userId)
    {
        UserId = userId;
    }
}
