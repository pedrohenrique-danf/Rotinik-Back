#nullable enable
using RotinikApi.DTOs.Requests.Routine;
using RotinikApi.DTOs.Responses.Routine;

namespace RotinikApi.Services.Routines
{
    public interface IRoutineService
    {
        System.Threading.Tasks.Task<IEnumerable<RoutineResponse>> GetAllAsync(int userId);
        System.Threading.Tasks.Task<RoutineDetailResponse?> GetByIdAsync(int id, int userId);
        System.Threading.Tasks.Task<RoutineResponse> CreateAsync(RoutineCreateRequest dto, int userId);
        System.Threading.Tasks.Task<RoutineResponse> UpdateAsync(int id, RoutineUpdateRequest dto, int userId);
        System.Threading.Tasks.Task DeleteAsync(int id, int userId);

        // FMRT_13
        System.Threading.Tasks.Task<IEnumerable<RoutineResponse>> GetTemplatesAsync();
        System.Threading.Tasks.Task<RoutineDetailResponse> CloneTemplateAsync(int templateId, int userId);

        // FMRT_14
        System.Threading.Tasks.Task<RoutineExecutionResponse> StartExecutionAsync(int routineId, int userId);
        System.Threading.Tasks.Task<RoutineExecutionResponse> FinishExecutionAsync(int executionId, int userId);
    }
}
