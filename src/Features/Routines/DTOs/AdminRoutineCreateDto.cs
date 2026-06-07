using System.Collections.Generic;
using Rotinik.Features.Tasks.DTOs;

namespace Rotinik.Features.Routines.DTOs;

public class AdminRoutineCreateDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Frequency { get; set; } = "daily";
    public List<TaskCreateDto> Tasks { get; set; } = new();
}
