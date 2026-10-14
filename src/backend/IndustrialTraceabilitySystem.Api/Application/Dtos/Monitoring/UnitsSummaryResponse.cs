namespace IndustrialTraceabilitySystem.Api.Application.Dtos.Monitoring;

public class UnitsSummaryResponse
{
    public int Total { get; set; }
    public int Created { get; set; }
    public int InProgress { get; set; }
    public int OnHold { get; set; }
    public int InRework { get; set; }
    public int Passed { get; set; }
    public int Scrapped { get; set; }
    public List<UnitCountByEventType> EventsByType { get; set; } = new();
    public List<UnitCountByStation> EventsByStation { get; set; } = new();
}

public class UnitCountByEventType
{
    public string EventType { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class UnitCountByStation
{
    public int StationId { get; set; }
    public string StationCode { get; set; } = string.Empty;
    public string StationName { get; set; } = string.Empty;
    public int Count { get; set; }
}