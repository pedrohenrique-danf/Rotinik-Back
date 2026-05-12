#nullable enable
using RotinikApi.DTOs.Requests.RoutineTask;
using RotinikApi.DTOs.Responses.RoutineTask;

namespace RotinikApi.Services.RoutineTasks
{
    public interface IRoutineTaskService
    {
        System.Threading.Tasks.Task<RoutineTaskResponse> AddTaskAsync(int routineId, AddTaskToRoutineRequest dto, int userId);
        System.Threading.Tasks.Task<RoutineTaskResponse> UpdateAsync(int routineId, int routineTaskId, UpdateRoutineTaskRequest dto, int userId);
        System.Threading.Tasks.Task RemoveAsync(int routineId, int routineTaskId, int userId);

        /// <summary>FMRT_12 + FMRT_14 — Mark a task complete, validating elapsed time first.</summary>
        System.Threading.Tasks.Task<RoutineTaskResponse> CompleteAsync(int routineId, int routineTaskId, CompleteRoutineTaskRequest dto, int userId);

        /// <summary>Undo task completion.</summary>
        System.Threading.Tasks.Task<RoutineTaskResponse> UncompleteAsync(int routineId, int routineTaskId, int userId);
    }
}
