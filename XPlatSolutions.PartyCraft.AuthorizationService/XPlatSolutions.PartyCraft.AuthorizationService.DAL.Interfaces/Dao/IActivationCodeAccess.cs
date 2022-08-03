using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Models;

namespace XPlatSolutions.PartyCraft.AuthorizationService.DAL.Interfaces.Dao;

public interface IActivationCodeAccess
{
    Task<List<ActivationCode>> GetActivationCodes(string userId);
    Task<ActivationCode> GetActivationCode(string value);
    Task<bool> ContainsValid(string value);
    Task AddActivationCode(ActivationCode code);
    Task RemoveAllActivationCodes(string userId);
}