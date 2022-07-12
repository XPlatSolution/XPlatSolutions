namespace XPlatSolutions.PartyCraft.AnalyticsService.Domain.Core.Classes;

public class AppOptions
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
    public string ServicesCollectionName { get; set; }
    public string ExceptionMessagesCollectionName { get; set; }

    public string HostName { get; set; }
    public string UserName { get; set; }
    public string PasswordRmq { get; set; }
}