#nullable enable
using RotinikApi.DTOs.Requests.Task;
using RotinikApi.DTOs.Responses.Task;

namespace RotinikApi.Services.Tasks
{
    public interface ITaskService
    {
        System.Threading.Tasks.Task<IEnumerable<TaskResponse>> GetAllAsync(int userId);
        System.Threading.Tasks.Task<TaskResponse?> GetByIdAsync(int id, int userId);
        System.Threading.Tasks.Task<TaskResponse> CreateAsync(TaskCreateRequest dto, int userId);
        System.Threading.Tasks.Task<TaskResponse> UpdateAsync(int id, TaskUpdateRequest dto, int userId);
        System.Threading.Tasks.Task DeleteAsync(int id, int userId);
    }
}
