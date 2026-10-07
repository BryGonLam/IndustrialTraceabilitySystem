using IndustrialTraceabilitySystem.Api.Domain.Enums;

namespace IndustrialTraceabilitySystem.Api.Application.Dtos.Units;

public class UnitHistoryResponse
{
    public int UnitId { get; set; }
    public string SerialNumber { get; set; } = string.Empty;
    public UnitStatus CurrentStatus { get; set; }
    public List<UnitHistoryEventResponse> Events { get; set; } = new();
}

public class UnitHistoryEventResponse
{
    public long Id { get; set; }
    public int StationId { get; set; }
    public string StationCode { get; set; } = string.Empty;
    public string StationName { get; set; } = string.Empty;
    public ProductionEventType EventType { get; set; }
    public ProductionEventResult? Result { get; set; }
    public string? Notes { get; set; }
    public DateTime OccurredAt { get; set; }
}