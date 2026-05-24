using MediatR;
using AI.TaskManagement.Application.DTOs.Notification;
using AI.TaskManagement.Shared.DTOs;

namespace AI.TaskManagement.Application.Features.Notifications.Queries;

public class GetUserNotificationsQuery : IRequest<PaginatedResult<NotificationDto>>
{
    public Guid UserId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public GetUserNotificationsQuery(Guid userId)
    {
        UserId = userId;
    }
}
