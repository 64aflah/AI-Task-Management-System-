using AutoMapper;
using AI.TaskManagement.Application.DTOs.Comment;
using AI.TaskManagement.Application.DTOs.Notification;
using AI.TaskManagement.Application.DTOs.Task;
using AI.TaskManagement.Application.DTOs.User;
using AI.TaskManagement.Domain.Entities;

namespace AI.TaskManagement.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Task mappings
        CreateMap<TaskItem, TaskDto>()
            .ForMember(dest => dest.AssignedUserName, 
                opt => opt.MapFrom(src => src.AssignedUser != null ? $"{src.AssignedUser.FirstName} {src.AssignedUser.LastName}" : null))
            .ForMember(dest => dest.CreatedByUserName,
                opt => opt.MapFrom(src => src.CreatedByUser != null ? $"{src.CreatedByUser.FirstName} {src.CreatedByUser.LastName}" : null));

        CreateMap<TaskDto, TaskItem>();

        // User mappings
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role != null ? src.Role.Name : null));

        CreateMap<User, GetProfileResponse>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role != null ? src.Role.Name : null));

        // Comment mappings
        CreateMap<Comment, CommentDto>()
            .ForMember(dest => dest.UserName,
                opt => opt.MapFrom(src => src.User != null ? $"{src.User.FirstName} {src.User.LastName}" : null));

        CreateMap<CommentDto, Comment>();

        // Notification mappings
        CreateMap<Notification, NotificationDto>();
        CreateMap<NotificationDto, Notification>();
    }
}
