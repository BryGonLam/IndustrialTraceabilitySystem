using IndustrialTraceabilitySystem.Api.Domain.Enums;

namespace IndustrialTraceabilitySystem.Api.Application.Dtos.ProductionEvents;

public class CreateProductionEventRequest
{
    public int UnitId { get; set; }
    public int StationId { get; set; }
    public int? UserId { get; set; }
    public ProductionEventType EventType { get; set; }
    public ProductionEventResult? Result { get; set; }
    public string? Notes { get; set; }
    public DateTime? OccurredAt { get; set; }
}