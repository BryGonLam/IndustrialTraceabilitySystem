using IndustrialTraceabilitySystem.Api.Domain.Enums;

namespace IndustrialTraceabilitySystem.Api.Application.Dtos.Units;

public class UnitResponse
{
    public int Id { get; set; }
    public string SerialNumber { get; set; } = string.Empty;
    public int ProductionOrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public UnitStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}