using IndustrialTraceabilitySystem.Api.Domain.Enums;

namespace IndustrialTraceabilitySystem.Api.Application.Dtos.ProductionOrders;

public class ProductionOrderResponse
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int ProductionLineId { get; set; }
    public string ProductionLineCode { get; set; } = string.Empty;
    public string ProductionLineName { get; set; } = string.Empty;
    public int PlannedQuantity { get; set; }
    public ProductionOrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public int TotalUnits { get; set; }
}