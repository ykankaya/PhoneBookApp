namespace ReportService.Application.Configuration;

public class KafkaOptions
{
    public string BootstrapServers { get; set; } = string.Empty;
    public string ReportRequestTopic { get; set; } = string.Empty; 
}