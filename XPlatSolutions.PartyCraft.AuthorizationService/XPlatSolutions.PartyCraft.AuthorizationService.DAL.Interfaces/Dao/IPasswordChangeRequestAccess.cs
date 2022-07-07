using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Models;

namespace XPlatSolutions.PartyCraft.AuthorizationService.DAL.Interfaces.Dao;

public interface IPasswordChangeRequestAccess
{
    Task<List<PasswordChangeRequest>> GetPasswordChangeRequests(string userId);
    Task<PasswordChangeRequest> GetPasswordChangeRequest(string id);
    Task AddPasswordChangeRequests(PasswordChangeRequest code);
    Task RemoveAllPasswordChangeRequests(string userId);
}