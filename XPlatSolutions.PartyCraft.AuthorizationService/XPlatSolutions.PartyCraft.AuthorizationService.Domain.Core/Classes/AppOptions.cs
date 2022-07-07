namespace XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Classes;

public class AppOptions
{
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
}