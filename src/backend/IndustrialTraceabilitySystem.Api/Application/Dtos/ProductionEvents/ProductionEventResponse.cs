using IndustrialTraceabilitySystem.Api.Domain.Enums;

namespace IndustrialTraceabilitySystem.Api.Application.Dtos.ProductionEvents;

public class ProductionEventResponse
{
    public long Id { get; set; }
    public int UnitId { get; set; }
    public string SerialNumber { get; set; } = string.Empty;
    public int StationId { get; set; }
    public string StationCode { get; set; } = string.Empty;
    public string StationName { get; set; } = string.Empty;
    public int? UserId { get; set; }
    public string? Username { get; set; }
    public ProductionEventType EventType { get; set; }
    public ProductionEventResult? Result { get; set; }
    public string? Notes { get; set; }
    public DateTime OccurredAt { get; set; }
    public DateTime CreatedAt { get; set; }
}