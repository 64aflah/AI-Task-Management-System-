using AutoMapper;
using MediatR;
using AI.TaskManagement.Application.DTOs.Task;
using AI.TaskManagement.Application.Features.Tasks.Commands;
using AI.TaskManagement.Domain.Entities;
using AI.TaskManagement.Domain.Interfaces;
using AI.TaskManagement.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace AI.TaskManagement.Application.Features.Tasks.Handlers;

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, TaskDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateTaskCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<TaskDto> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = new TaskItem
        {
            Title = request.Title,
            Description = request.Description,
            Priority = request.Priority,
            DueDate = request.DueDate,
            AssignedUserId = request.AssignedUserId,
            CreatedByUserId = request.CreatedByUserId
        };

        var taskRepo = _unitOfWork.Repository<TaskItem>();
        await taskRepo.AddAsync(task);

        // Load related data
        var userRepo = _unitOfWork.Repository<User>();
        var users = await userRepo.GetAllAsync();
        task.CreatedByUser = users.FirstOrDefault(u => u.Id == task.CreatedByUserId);
        if (task.AssignedUserId.HasValue)
            task.AssignedUser = users.FirstOrDefault(u => u.Id == task.AssignedUserId.Value);

        return _mapper.Map<TaskDto>(task);
    }
}

public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, TaskDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateTaskCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<TaskDto> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var taskRepo = _unitOfWork.Repository<TaskItem>();
        var task = await taskRepo.GetByIdAsync(request.Id) ?? throw new NotFoundException("Task not found");

        task.Title = request.Title;
        task.Description = request.Description;
        task.Priority = request.Priority;
        task.Status = request.Status;
        task.DueDate = request.DueDate;
        task.AssignedUserId = request.AssignedUserId;
        task.UpdatedAt = DateTime.UtcNow;

        await taskRepo.UpdateAsync(task);

        // Load related data
        var userRepo = _unitOfWork.Repository<User>();
        var users = await userRepo.GetAllAsync();
        task.CreatedByUser = users.FirstOrDefault(u => u.Id == task.CreatedByUserId);
        if (task.AssignedUserId.HasValue)
            task.AssignedUser = users.FirstOrDefault(u => u.Id == task.AssignedUserId.Value);

        return _mapper.Map<TaskDto>(task);
    }
}

public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTaskCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        var taskRepo = _unitOfWork.Repository<TaskItem>();
        var task = await taskRepo.GetByIdAsync(request.Id) ?? throw new NotFoundException("Task not found");

        task.IsDeleted = true;
        task.DeletedAt = DateTime.UtcNow;
        await taskRepo.UpdateAsync(task);

        return true;
    }
}

public class AssignTaskCommandHandler : IRequestHandler<AssignTaskCommand, TaskDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AssignTaskCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<TaskDto> Handle(AssignTaskCommand request, CancellationToken cancellationToken)
    {
        var taskRepo = _unitOfWork.Repository<TaskItem>();
        var task = await taskRepo.GetByIdAsync(request.TaskId) ?? throw new NotFoundException("Task not found");

        var userRepo = _unitOfWork.Repository<User>();
        var assignedUser = await userRepo.GetByIdAsync(request.AssignedUserId) ?? throw new NotFoundException("User not found");

        task.AssignedUserId = request.AssignedUserId;
        task.UpdatedAt = DateTime.UtcNow;
        await taskRepo.UpdateAsync(task);

        // Load related data
        var users = await userRepo.GetAllAsync();
        task.CreatedByUser = users.FirstOrDefault(u => u.Id == task.CreatedByUserId);
        task.AssignedUser = assignedUser;

        return _mapper.Map<TaskDto>(task);
    }
}
