using AutoMapper;
using MediatR;
using AI.TaskManagement.Application.DTOs.Comment;
using AI.TaskManagement.Application.Features.Comments.Commands;
using AI.TaskManagement.Domain.Entities;
using AI.TaskManagement.Domain.Interfaces;
using AI.TaskManagement.Shared.Exceptions;

namespace AI.TaskManagement.Application.Features.Comments.Handlers;

public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, CommentDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateCommentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CommentDto> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = new Comment
        {
            Content = request.Content,
            TaskId = request.TaskId,
            UserId = request.UserId
        };

        var commentRepo = _unitOfWork.Repository<Comment>();
        await commentRepo.AddAsync(comment);

        var userRepo = _unitOfWork.Repository<User>();
        comment.User = await userRepo.GetByIdAsync(request.UserId);

        return _mapper.Map<CommentDto>(comment);
    }
}

public class UpdateCommentCommandHandler : IRequestHandler<UpdateCommentCommand, CommentDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateCommentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CommentDto> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
    {
        var commentRepo = _unitOfWork.Repository<Comment>();
        var comment = await commentRepo.GetByIdAsync(request.Id) ?? throw new NotFoundException("Comment not found");

        comment.Content = request.Content;
        comment.UpdatedAt = DateTime.UtcNow;
        await commentRepo.UpdateAsync(comment);

        var userRepo = _unitOfWork.Repository<User>();
        comment.User = await userRepo.GetByIdAsync(comment.UserId);

        return _mapper.Map<CommentDto>(comment);
    }
}

public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCommentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        var commentRepo = _unitOfWork.Repository<Comment>();
        var comment = await commentRepo.GetByIdAsync(request.Id) ?? throw new NotFoundException("Comment not found");

        comment.IsDeleted = true;
        comment.DeletedAt = DateTime.UtcNow;
        await commentRepo.UpdateAsync(comment);

        return true;
    }
}
