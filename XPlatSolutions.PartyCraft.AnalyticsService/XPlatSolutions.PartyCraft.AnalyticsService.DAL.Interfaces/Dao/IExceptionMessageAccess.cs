using XPlatSolutions.PartyCraft.AnalyticsService.Domain.Core.Models;

namespace XPlatSolutions.PartyCraft.AnalyticsService.DAL.Interfaces.Dao;

public interface IExceptionMessageAccess
{
    Task<List<ExceptionMessage>> GetExceptionMessages(string serviceId);
    Task<List<ExceptionMessage>> GetExceptionMessages();
    Task<List<ExceptionMessage>> GetExceptionMessages(DateTime from, DateTime to);
    Task<List<ExceptionMessage>> GetExceptionMessages(string serviceId, DateTime from, DateTime to);
    Task AddExceptionMessage(ExceptionMessage message);
    Task RemoveAllActivationCodes(string serviceId);
}