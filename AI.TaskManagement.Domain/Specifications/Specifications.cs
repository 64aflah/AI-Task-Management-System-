using AI.TaskManagement.Domain.Enums;

namespace AI.TaskManagement.Domain.Specifications;

public interface ISpecification<T>
{
    IQueryable<T> Apply(IQueryable<T> query);
}

public class TaskSpecification : ISpecification<Entities.TaskItem>
{
    private readonly Guid? _assignedUserId;
    private readonly TaskStatus? _status;
    private readonly TaskPriority? _priority;
    private readonly string? _searchTerm;

    public TaskSpecification(Guid? assignedUserId = null, TaskStatus? status = null, 
        TaskPriority? priority = null, string? searchTerm = null)
    {
        _assignedUserId = assignedUserId;
        _status = status;
        _priority = priority;
        _searchTerm = searchTerm;
    }

    public IQueryable<Entities.TaskItem> Apply(IQueryable<Entities.TaskItem> query)
    {
        var result = query.Where(t => !t.IsDeleted);

        if (_assignedUserId.HasValue)
            result = result.Where(t => t.AssignedUserId == _assignedUserId.Value);

        if (_status.HasValue)
            result = result.Where(t => t.Status == _status.Value);

        if (_priority.HasValue)
            result = result.Where(t => t.Priority == _priority.Value);

        if (!string.IsNullOrWhiteSpace(_searchTerm))
            result = result.Where(t => 
                t.Title.Contains(_searchTerm) || 
                t.Description.Contains(_searchTerm));

        return result;
    }
}
