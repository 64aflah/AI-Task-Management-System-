import { Task } from '../store/taskSlice';

interface TaskListProps {
  tasks: Task[];
}

const TaskList: React.FC<TaskListProps> = ({ tasks }) => {
  const getPriorityColor = (priority: string) => {
    switch (priority) {
      case 'Critical':
        return 'bg-red-100 text-red-800';
      case 'High':
        return 'bg-orange-100 text-orange-800';
      case 'Medium':
        return 'bg-yellow-100 text-yellow-800';
      default:
        return 'bg-green-100 text-green-800';
    }
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Completed':
        return 'bg-green-100 text-green-800';
      case 'InProgress':
        return 'bg-blue-100 text-blue-800';
      case 'OnHold':
        return 'bg-gray-100 text-gray-800';
      case 'Cancelled':
        return 'bg-red-100 text-red-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  };

  if (tasks.length === 0) {
    return <div className="text-center py-8 text-gray-500">No tasks yet. Create one to get started!</div>;
  }

  return (
    <div className="space-y-4">
      {tasks.map((task) => (
        <div
          key={task.id}
          className="p-4 border border-gray-200 rounded-lg hover:shadow-md transition"
        >
          <div className="flex justify-between items-start mb-2">
            <h3 className="text-lg font-semibold text-gray-900">{task.title}</h3>
            <div className="flex gap-2">
              <span className={`px-2 py-1 rounded text-xs font-medium ${getPriorityColor(task.priority)}`}>
                {task.priority}
              </span>
              <span className={`px-2 py-1 rounded text-xs font-medium ${getStatusColor(task.status)}`}>
                {task.status}
              </span>
            </div>
          </div>
          <p className="text-gray-700 text-sm mb-2">{task.description}</p>
          <div className="flex justify-between text-xs text-gray-500">
            <span>Created by: {task.createdByUserName}</span>
            {task.assignedUserName && <span>Assigned to: {task.assignedUserName}</span>}
            {task.dueDate && <span>Due: {new Date(task.dueDate).toLocaleDateString()}</span>}
          </div>
        </div>
      ))}
    </div>
  );
};

export default TaskList;
