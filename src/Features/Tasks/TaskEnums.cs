namespace Rotinik.Features.Tasks;

public enum TaskFrequency
{
    None = 0,
    Daily = 1,
    Monthly = 2,
    Yearly = 3
}

public enum TaskPriority
{
    Low = 1,
    Moderate = 2,
    Important = 3,
    Urgent = 4
}

public enum TaskHistoryStatus
{
    Completed = 1,
    Failed = 2
}