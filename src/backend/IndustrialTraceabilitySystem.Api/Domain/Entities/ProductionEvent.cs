using IndustrialTraceabilitySystem.Api.Domain.Enums;

namespace IndustrialTraceabilitySystem.Api.Domain.Entities;

public class ProductionEvent
{
    public long Id { get; set; }
    public int UnitId { get; set; }
    public int StationId { get; set; }
    public int? UserId { get; set; }
    public ProductionEventType EventType { get; set; }
    public ProductionEventResult? Result { get; set; }
    public string? Notes { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Unit Unit { get; set; } = null!;
    public Station Station { get; set; } = null!;
    public User? User { get; set; }
}