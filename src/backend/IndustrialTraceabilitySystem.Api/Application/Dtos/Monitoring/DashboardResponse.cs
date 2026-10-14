namespace IndustrialTraceabilitySystem.Api.Application.Dtos.Monitoring;

public class DashboardResponse
{
    public int TotalProducts { get; set; }
    public int TotalLines { get; set; }
    public int TotalStations { get; set; }
    public int TotalOrders { get; set; }
    public int ActiveOrders { get; set; }
    public int TotalUnits { get; set; }
    public int TotalEvents { get; set; }
    public DateTime GeneratedAt { get; set; }
}