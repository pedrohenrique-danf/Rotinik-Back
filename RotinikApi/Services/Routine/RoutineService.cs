#nullable enable
using Microsoft.EntityFrameworkCore;
using RotinikApi.Data;
using RotinikApi.DTOs.Requests.Routine;
using RotinikApi.DTOs.Responses.Routine;
using RotinikApi.DTOs.Responses.RoutineTask;
using RotinikApi.Models;

namespace RotinikApi.Services.Routines
{
    public class RoutineService : IRoutineService
    {
        private readonly RotinikContext _context;

        public RoutineService(RotinikContext context)
        {
            _context = context;
        }

        // ── FMRT_1 ─────────────────────────────────────────────────────────
        public async System.Threading.Tasks.Task<RoutineResponse> CreateAsync(RoutineCreateRequest dto, int userId)
        {
            var routine = new Routine(userId, dto.Name, dto.Description, dto.Theme);
            _context.Routines.Add(routine);
            await _context.SaveChangesAsync();
            return MapToResponse(routine, 0);
        }

        // ── FMRT_2 ─────────────────────────────────────────────────────────
        public async System.Threading.Tasks.Task<RoutineResponse> UpdateAsync(int id, RoutineUpdateRequest dto, int userId)
        {
            var routine = await GetOwnedRoutineAsync(id, userId);

            if (dto.Name        != null) routine.SetName(dto.Name);
            if (dto.Description != null) routine.SetDescription(dto.Description);
            if (dto.Theme       != null) routine.SetTheme(dto.Theme);

            await _context.SaveChangesAsync();

            var taskCount = await _context.RoutineTasks.CountAsync(rt => rt.RoutineId == id);
            return MapToResponse(routine, taskCount);
        }

        // ── FMRT_3 ─────────────────────────────────────────────────────────
        public async System.Threading.Tasks.Task DeleteAsync(int id, int userId)
        {
            var routine = await GetOwnedRoutineAsync(id, userId);
            _context.Routines.Remove(routine);
            await _context.SaveChangesAsync();
        }

        // ── List / Get ──────────────────────────────────────────────────────
        public async System.Threading.Tasks.Task<IEnumerable<RoutineResponse>> GetAllAsync(int userId)
        {
            return await _context.Routines
              //  .Where(r => r.UserId == userId)
                .Select(r => new RoutineResponse
                {
                    Id          = r.Id,
                    UserId      = r.UserId,
                    Name        = r.Name,
                    Description = r.Description,
                    Theme       = r.Theme,
                    IsTemplate  = r.IsTemplate,
                    TaskCount   = r.RoutineTasks.Count,
                    CreatedAt   = r.CreatedAt
                })
                .ToListAsync();
        }

        public async System.Threading.Tasks.Task<RoutineDetailResponse?> GetByIdAsync(int id, int userId)
        {
            var routine = await _context.Routines
                .Include(r => r.RoutineTasks)
                    .ThenInclude(rt => rt.Task)
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (routine is null) return null;
            return MapToDetailResponse(routine);
        }

        // ── FMRT_13 — Templates ─────────────────────────────────────────────
        public async System.Threading.Tasks.Task<IEnumerable<RoutineResponse>> GetTemplatesAsync()
        {
            return await _context.Routines
                .Where(r => r.IsTemplate)
                .Select(r => new RoutineResponse
                {
                    Id          = r.Id,
                    UserId      = r.UserId,
                    Name        = r.Name,
                    Description = r.Description,
                    Theme       = r.Theme,
                    IsTemplate  = r.IsTemplate,
                    TaskCount   = r.RoutineTasks.Count,
                    CreatedAt   = r.CreatedAt
                })
                .ToListAsync();
        }

        public async System.Threading.Tasks.Task<RoutineDetailResponse> CloneTemplateAsync(int templateId, int userId)
        {
            var template = await _context.Routines
                .Include(r => r.RoutineTasks)
                    .ThenInclude(rt => rt.Task)
                .FirstOrDefaultAsync(r => r.Id == templateId && r.IsTemplate);

            if (template is null)
                throw new KeyNotFoundException("Template routine not found.");

            var cloned = new Routine(userId, template.Name, template.Description, template.Theme, isTemplate: false);
            _context.Routines.Add(cloned);
            await _context.SaveChangesAsync();

            foreach (var rt in template.RoutineTasks.OrderBy(x => x.Order))
                _context.RoutineTasks.Add(new RoutineTask(cloned.Id, rt.TaskId, rt.Order));

            await _context.SaveChangesAsync();

            var result = await _context.Routines
                .Include(r => r.RoutineTasks)
                    .ThenInclude(rt => rt.Task)
                .FirstAsync(r => r.Id == cloned.Id);

            return MapToDetailResponse(result);
        }

        // ── FMRT_14 — Execution tracking ────────────────────────────────────
        public async System.Threading.Tasks.Task<RoutineExecutionResponse> StartExecutionAsync(int routineId, int userId)
        {
            await GetOwnedRoutineAsync(routineId, userId);

            var execution = new RoutineExecution(routineId, userId);
            _context.RoutineExecutions.Add(execution);
            await _context.SaveChangesAsync();

            return MapExecution(execution);
        }

        public async System.Threading.Tasks.Task<RoutineExecutionResponse> FinishExecutionAsync(int executionId, int userId)
        {
            var execution = await _context.RoutineExecutions
                .FirstOrDefaultAsync(e => e.Id == executionId && e.UserId == userId);

            if (execution is null)
                throw new KeyNotFoundException("Routine execution not found.");

            execution.Finish();
            await _context.SaveChangesAsync();

            return MapExecution(execution);
        }

        // ── Helpers ─────────────────────────────────────────────────────────
        private async System.Threading.Tasks.Task<Routine> GetOwnedRoutineAsync(int id, int userId)
        {
            var routine = await _context.Routines
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (routine is null)
                throw new KeyNotFoundException("Routine not found or access denied.");

            return routine;
        }

        private static RoutineResponse MapToResponse(Routine r, int taskCount) => new()
        {
            Id          = r.Id,
            UserId      = r.UserId,
            Name        = r.Name,
            Description = r.Description,
            Theme       = r.Theme,
            IsTemplate  = r.IsTemplate,
            TaskCount   = taskCount,
            CreatedAt   = r.CreatedAt
        };

        private static RoutineDetailResponse MapToDetailResponse(Routine r) => new()
        {
            Id          = r.Id,
            UserId      = r.UserId,
            Name        = r.Name,
            Description = r.Description,
            Theme       = r.Theme,
            IsTemplate  = r.IsTemplate,
            CreatedAt   = r.CreatedAt,
            Tasks       = r.RoutineTasks
                           .OrderBy(rt => rt.Order)
                           .Select(MapRoutineTask)
                           .ToList()
        };

        private static RoutineTaskResponse MapRoutineTask(RoutineTask rt) => new()
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

        private static RoutineExecutionResponse MapExecution(RoutineExecution e) => new()
        {
            Id             = e.Id,
            RoutineId      = e.RoutineId,
            UserId         = e.UserId,
            StartedAt      = e.StartedAt,
            FinishedAt     = e.FinishedAt,
            ElapsedMinutes = e.ElapsedMinutes()
        };
    }
}
