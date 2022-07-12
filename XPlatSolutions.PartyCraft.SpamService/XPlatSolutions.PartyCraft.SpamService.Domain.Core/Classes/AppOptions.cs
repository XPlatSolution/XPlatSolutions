namespace XPlatSolutions.PartyCraft.SpamService.Domain.Core.Classes;

public class AppOptions
{
    public string EmailFrom { get; set; }
    public string Password { get; set; }
    public string SmtpHost { get; set; }
    public int SmtpPort { get; set; }

    public string HostName { get; set; }
    public string UserName { get; set; }
    public string PasswordRmq { get; set; }
    
    public string AnalyticsHostName { get; set; }
    public string AnalyticsUserName { get; set; }
    public string AnalyticsPasswordRmq { get; set; }
}