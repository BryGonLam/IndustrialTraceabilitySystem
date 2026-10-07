using IndustrialTraceabilitySystem.Api.Domain.Enums;

namespace IndustrialTraceabilitySystem.Api.Application.Dtos.Units;

public class UnitDetailResponse
{
    public int Id { get; set; }
    public string SerialNumber { get; set; } = string.Empty;
    public int ProductionOrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public UnitStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}