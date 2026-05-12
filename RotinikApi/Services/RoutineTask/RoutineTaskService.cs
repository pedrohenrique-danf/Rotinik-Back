#nullable enable
using Microsoft.EntityFrameworkCore;
using RotinikApi.Data;
using RotinikApi.DTOs.Requests.RoutineTask;
using RotinikApi.DTOs.Responses.RoutineTask;
using RotinikApi.Models;

namespace RotinikApi.Services.RoutineTasks
{
    public class RoutineTaskService : IRoutineTaskService
    {
        private readonly RotinikContext _context;

        public RoutineTaskService(RotinikContext context)
        {
            _context = context;
        }

        // ── FMRT_7 — Add task to routine ────────────────────────────────────
        public async System.Threading.Tasks.Task<RoutineTaskResponse> AddTaskAsync(
            int routineId, AddTaskToRoutineRequest dto, int userId)
        {
            await RequireOwnedRoutineAsync(routineId, userId);

            var taskExists = await _context.Tasks
                .AnyAsync(t => t.Id == dto.TaskId && (t.UserId == userId || t.UserId == null));

            if (!taskExists)
                throw new KeyNotFoundException("Task not found or access denied.");

            var duplicate = await _context.RoutineTasks
                .AnyAsync(rt => rt.RoutineId == routineId && rt.TaskId == dto.TaskId);

            if (duplicate)
                throw new InvalidOperationException("This task is already in the routine.");

            var routineTask = new RoutineTask(routineId, dto.TaskId, dto.Order);
            _context.RoutineTasks.Add(routineTask);
            await _context.SaveChangesAsync();

            return await LoadRoutineTaskResponseAsync(routineTask.Id);
        }

        // ── FMRT_8 — Edit task entry in routine ─────────────────────────────
        public async System.Threading.Tasks.Task<RoutineTaskResponse> UpdateAsync(
            int routineId, int routineTaskId, UpdateRoutineTaskRequest dto, int userId)
        {
            var rt = await GetOwnedRoutineTaskAsync(routineId, routineTaskId, userId);

            if (dto.Order.HasValue) rt.SetOrder(dto.Order.Value);

            await _context.SaveChangesAsync();
            return await LoadRoutineTaskResponseAsync(rt.Id);
        }

        // ── FMRT_9 — Remove task from routine ───────────────────────────────
        public async System.Threading.Tasks.Task RemoveAsync(int routineId, int routineTaskId, int userId)
        {
            var rt = await GetOwnedRoutineTaskAsync(routineId, routineTaskId, userId);
            _context.RoutineTasks.Remove(rt);
            await _context.SaveChangesAsync();
        }

        // ── FMRT_12 + FMRT_14 — Complete task ───────────────────────────────
        public async System.Threading.Tasks.Task<RoutineTaskResponse> CompleteAsync(
            int routineId, int routineTaskId, CompleteRoutineTaskRequest dto, int userId)
        {
            var rt = await GetOwnedRoutineTaskAsync(routineId, routineTaskId, userId);

            // FMRT_14: find the running execution and validate elapsed time
            var execution = await _context.RoutineExecutions
                .FirstOrDefaultAsync(e => e.Id == dto.RoutineExecutionId
                                       && e.RoutineId  == routineId
                                       && e.UserId     == userId
                                       && e.FinishedAt == null);

            if (execution is null)
                throw new KeyNotFoundException(
                    "Active routine execution not found. Start the routine before completing tasks.");

            var userTask = await _context.Tasks.FindAsync(rt.TaskId);

            if (userTask is null)
                throw new InvalidOperationException("Associated task record not found.");

            var elapsed = execution.ElapsedMinutes();
            if (elapsed < userTask.EstimatedMinutes)
            {
                throw new InvalidOperationException(
                    $"Cannot complete this task yet. Minimum required time: {userTask.EstimatedMinutes} min. " +
                    $"Elapsed: {elapsed:F1} min.");
            }

            rt.Complete();
            await _context.SaveChangesAsync();

            return await LoadRoutineTaskResponseAsync(rt.Id);
        }

        // ── Undo completion ──────────────────────────────────────────────────
        public async System.Threading.Tasks.Task<RoutineTaskResponse> UncompleteAsync(
            int routineId, int routineTaskId, int userId)
        {
            var rt = await GetOwnedRoutineTaskAsync(routineId, routineTaskId, userId);
            rt.Uncomplete();
            await _context.SaveChangesAsync();
            return await LoadRoutineTaskResponseAsync(rt.Id);
        }

        // ── Helpers ─────────────────────────────────────────────────────────
        private async System.Threading.Tasks.Task RequireOwnedRoutineAsync(int routineId, int userId)
        {
            var exists = await _context.Routines
                .AnyAsync(r => r.Id == routineId && r.UserId == userId);

            if (!exists)
                throw new KeyNotFoundException("Routine not found or access denied.");
        }

        private async System.Threading.Tasks.Task<RoutineTask> GetOwnedRoutineTaskAsync(
            int routineId, int routineTaskId, int userId)
        {
            var rt = await _context.RoutineTasks
                .Include(x => x.Routine)
                .FirstOrDefaultAsync(x => x.Id == routineTaskId
                                       && x.RoutineId == routineId
                                       && x.Routine.UserId == userId);

            if (rt is null)
                throw new KeyNotFoundException("Routine task entry not found or access denied.");

            return rt;
        }

        private async System.Threading.Tasks.Task<RoutineTaskResponse> LoadRoutineTaskResponseAsync(int routineTaskId)
        {
            var rt = await _context.RoutineTasks
                .Include(x => x.Task)
                .FirstAsync(x => x.Id == routineTaskId);

            return new RoutineTaskResponse
            {
                Id               = rt.Id,
                RoutineId        = rt.RoutineId,
                TaskId           = rt.TaskId,
                TaskTitle        = rt.Task.Title,
                TaskDescription  = rt.Task.Description,
                EstimatedMinutes = rt.Task.EstimatedMinutes,
                Frequency        = rt.Task.Frequency,
                Type             = rt.Task.Type,
                Order            = rt.Order,
                IsCompleted      = rt.IsCompleted,
                CompletedAt      = rt.CompletedAt
            };
        }
    }
}
