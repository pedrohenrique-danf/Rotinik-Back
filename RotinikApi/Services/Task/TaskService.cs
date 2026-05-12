#nullable enable
using Microsoft.EntityFrameworkCore;
using RotinikApi.Data;
using RotinikApi.DTOs.Requests.Task;
using RotinikApi.DTOs.Responses.Task;
using RotinikApi.Models;

namespace RotinikApi.Services.Tasks
{
    public class TaskService : ITaskService
    {
        private readonly RotinikContext _context;

        public TaskService(RotinikContext context)
        {
            _context = context;
        }

        // ── FMRT_4 ─────────────────────────────────────────────────────────
        public async System.Threading.Tasks.Task<TaskResponse> CreateAsync(TaskCreateRequest dto, int userId)
        {
            var task = new UserTask(userId, dto.Title, dto.Description, dto.EstimatedMinutes, dto.Frequency, dto.Type);
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return MapToResponse(task);
        }

        // ── FMRT_5 ─────────────────────────────────────────────────────────
        public async System.Threading.Tasks.Task<TaskResponse> UpdateAsync(int id, TaskUpdateRequest dto, int userId)
        {
            var task = await GetOwnedTaskAsync(id, userId);

            if (dto.Title            != null) task.SetTitle(dto.Title);
            if (dto.Description      != null) task.SetDescription(dto.Description);
            if (dto.EstimatedMinutes != null) task.SetEstimatedMinutes(dto.EstimatedMinutes.Value);
            if (dto.Frequency        != null) task.SetFrequency(dto.Frequency.Value);
            if (dto.Type             != null) task.SetType(dto.Type.Value);

            await _context.SaveChangesAsync();
            return MapToResponse(task);
        }

        // ── FMRT_6 ─────────────────────────────────────────────────────────
        public async System.Threading.Tasks.Task DeleteAsync(int id, int userId)
        {
            var task = await GetOwnedTaskAsync(id, userId);
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }

        // ── List / Get ──────────────────────────────────────────────────────
        public async System.Threading.Tasks.Task<IEnumerable<TaskResponse>> GetAllAsync(int userId)
        {
            return await _context.Tasks
                .Where(t => t.UserId == userId)
                .Select(t => new TaskResponse
                {
                    Id               = t.Id,
                    UserId           = t.UserId,
                    Title            = t.Title,
                    Description      = t.Description,
                    EstimatedMinutes = t.EstimatedMinutes,
                    Frequency        = t.Frequency,
                    Type             = t.Type,
                    CreatedAt        = t.CreatedAt
                })
                .ToListAsync();
        }

        public async System.Threading.Tasks.Task<TaskResponse?> GetByIdAsync(int id, int userId)
        {
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            return task is null ? null : MapToResponse(task);
        }

        // ── Helpers ─────────────────────────────────────────────────────────
        private async System.Threading.Tasks.Task<UserTask> GetOwnedTaskAsync(int id, int userId)
        {
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (task is null)
                throw new KeyNotFoundException("Task not found or access denied.");

            return task;
        }

        private static TaskResponse MapToResponse(UserTask t) => new()
        {
            Id               = t.Id,
            UserId           = t.UserId,
            Title            = t.Title,
            Description      = t.Description,
            EstimatedMinutes = t.EstimatedMinutes,
            Frequency        = t.Frequency,
            Type             = t.Type,
            CreatedAt        = t.CreatedAt
        };
    }
}
