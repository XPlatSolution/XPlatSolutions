namespace XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Classes;

public class AppOptions
{
    public bool IsCacheEnabled { get; set; }
    public string RedisHost { get; set; }
    public int RedisPort { get; set; }
    public string RedisPassword { get; set; }
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
    public string ActivationCodeCollectionName { get; set; }
    public string PasswordChangeRequestCollectionName { get; set; }
    public string UsersCollectionName { get; set; }
    public string TokensCollectionName { get; set; }
    public  int ResetPasswordInHoursTTL { get; set; }
    public int CookieTTL { get; set; }
    public int RefreshTokenTTL { get; set; }
    public int ActivationCodeMinutesTTL { get; set; }
    public string Secret { get; set; }
    public string IdSecret { get; set; }
    public bool RemoveOldAndRevokedTokens { get; set; }
    
    public string HostName { get; set; }
    public string UserName { get; set; }
    public string PasswordRmq { get; set; }

    public string AnalyticsHostName { get; set; }
    public string AnalyticsUserName { get; set; }
    public string AnalyticsPasswordRmq { get; set; }
}