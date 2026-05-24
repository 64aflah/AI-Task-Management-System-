using AutoMapper;
using MediatR;
using AI.TaskManagement.Application.DTOs.Task;
using AI.TaskManagement.Application.Features.Tasks.Queries;
using AI.TaskManagement.Domain.Entities;
using AI.TaskManagement.Domain.Interfaces;
using AI.TaskManagement.Shared.DTOs;
using AI.TaskManagement.Shared.Exceptions;

namespace AI.TaskManagement.Application.Features.Tasks.Handlers;

public class GetAllTasksQueryHandler : IRequestHandler<GetAllTasksQuery, PaginatedResult<TaskDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllTasksQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<TaskDto>> Handle(GetAllTasksQuery request, CancellationToken cancellationToken)
    {
        var taskRepo = _unitOfWork.Repository<TaskItem>();
        var tasks = (await taskRepo.GetAllAsync()).AsQueryable();

        if (request.AssignedUserId.HasValue)
            tasks = tasks.Where(t => t.AssignedUserId == request.AssignedUserId.Value);

        if (request.Status.HasValue)
            tasks = tasks.Where(t => t.Status == request.Status.Value);

        if (request.Priority.HasValue)
            tasks = tasks.Where(t => t.Priority == request.Priority.Value);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            tasks = tasks.Where(t => 
                t.Title.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                t.Description.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase));

        var totalCount = tasks.Count();
        var items = tasks.Skip((request.PageNumber - 1) * request.PageSize)
                        .Take(request.PageSize)
                        .ToList();

        // Load related data
        var userRepo = _unitOfWork.Repository<User>();
        var users = await userRepo.GetAllAsync();

        var taskDtos = new List<TaskDto>();
        foreach (var task in items)
        {
            task.CreatedByUser = users.FirstOrDefault(u => u.Id == task.CreatedByUserId);
            if (task.AssignedUserId.HasValue)
                task.AssignedUser = users.FirstOrDefault(u => u.Id == task.AssignedUserId.Value);
            taskDtos.Add(_mapper.Map<TaskDto>(task));
        }

        return new PaginatedResult<TaskDto>
        {
            Items = taskDtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}

public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, TaskDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetTaskByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<TaskDto> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        var taskRepo = _unitOfWork.Repository<TaskItem>();
        var task = await taskRepo.GetByIdAsync(request.Id) ?? throw new NotFoundException("Task not found");

        // Load related data
        var userRepo = _unitOfWork.Repository<User>();
        var users = await userRepo.GetAllAsync();
        task.CreatedByUser = users.FirstOrDefault(u => u.Id == task.CreatedByUserId);
        if (task.AssignedUserId.HasValue)
            task.AssignedUser = users.FirstOrDefault(u => u.Id == task.AssignedUserId.Value);

        return _mapper.Map<TaskDto>(task);
    }
}

public class GetUserTasksQueryHandler : IRequestHandler<GetUserTasksQuery, PaginatedResult<TaskDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetUserTasksQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<TaskDto>> Handle(GetUserTasksQuery request, CancellationToken cancellationToken)
    {
        var taskRepo = _unitOfWork.Repository<TaskItem>();
        var tasks = (await taskRepo.GetAllAsync())
            .Where(t => t.AssignedUserId == request.UserId)
            .AsQueryable();

        var totalCount = tasks.Count();
        var items = tasks.Skip((request.PageNumber - 1) * request.PageSize)
                        .Take(request.PageSize)
                        .ToList();

        // Load related data
        var userRepo = _unitOfWork.Repository<User>();
        var users = await userRepo.GetAllAsync();

        var taskDtos = new List<TaskDto>();
        foreach (var task in items)
        {
            task.CreatedByUser = users.FirstOrDefault(u => u.Id == task.CreatedByUserId);
            task.AssignedUser = users.FirstOrDefault(u => u.Id == task.AssignedUserId);
            taskDtos.Add(_mapper.Map<TaskDto>(task));
        }

        return new PaginatedResult<TaskDto>
        {
            Items = taskDtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
