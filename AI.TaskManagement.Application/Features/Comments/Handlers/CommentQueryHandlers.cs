using AutoMapper;
using MediatR;
using AI.TaskManagement.Application.DTOs.Comment;
using AI.TaskManagement.Application.Features.Comments.Queries;
using AI.TaskManagement.Domain.Entities;
using AI.TaskManagement.Domain.Interfaces;
using AI.TaskManagement.Shared.DTOs;
using AI.TaskManagement.Shared.Exceptions;

namespace AI.TaskManagement.Application.Features.Comments.Handlers;

public class GetCommentsByTaskQueryHandler : IRequestHandler<GetCommentsByTaskQuery, PaginatedResult<CommentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCommentsByTaskQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<CommentDto>> Handle(GetCommentsByTaskQuery request, CancellationToken cancellationToken)
    {
        var commentRepo = _unitOfWork.Repository<Comment>();
        var comments = (await commentRepo.GetAllAsync())
            .Where(c => c.TaskId == request.TaskId)
            .AsQueryable();

        var totalCount = comments.Count();
        var items = comments.Skip((request.PageNumber - 1) * request.PageSize)
                            .Take(request.PageSize)
                            .ToList();

        var userRepo = _unitOfWork.Repository<User>();
        var users = await userRepo.GetAllAsync();

        var commentDtos = new List<CommentDto>();
        foreach (var comment in items)
        {
            comment.User = users.FirstOrDefault(u => u.Id == comment.UserId);
            commentDtos.Add(_mapper.Map<CommentDto>(comment));
        }

        return new PaginatedResult<CommentDto>
        {
            Items = commentDtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}

public class GetCommentByIdQueryHandler : IRequestHandler<GetCommentByIdQuery, CommentDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCommentByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CommentDto> Handle(GetCommentByIdQuery request, CancellationToken cancellationToken)
    {
        var commentRepo = _unitOfWork.Repository<Comment>();
        var comment = await commentRepo.GetByIdAsync(request.Id) ?? throw new NotFoundException("Comment not found");

        var userRepo = _unitOfWork.Repository<User>();
        comment.User = await userRepo.GetByIdAsync(comment.UserId);

        return _mapper.Map<CommentDto>(comment);
    }
}
