namespace IndustrialTraceabilitySystem.Api.Application.Dtos.Monitoring;

public class LineStatusResponse
{
    public int LineId { get; set; }
    public string LineCode { get; set; } = string.Empty;
    public string LineName { get; set; } = string.Empty;
    public int TotalStations { get; set; }
    public int ActiveOrders { get; set; }
    public int TotalUnits { get; set; }
}