using Microsoft.Extensions.Options;
using MongoDB.Driver;
using XPlatSolutions.PartyCraft.AnalyticsService.DAL.Interfaces.Dao;
using XPlatSolutions.PartyCraft.AnalyticsService.Domain.Core.Classes;
using XPlatSolutions.PartyCraft.AnalyticsService.Domain.Core.Models;

namespace XPlatSolutions.PartyCraft.AnalyticsService.DAL.Dao;

public class ExceptionMessageAccess : IExceptionMessageAccess
{
    private readonly IMongoCollection<ExceptionMessage> _exceptionMessagesCollection;

    public ExceptionMessageAccess(IDatabaseResolver databaseResolver, IOptions<AppOptions> appOptions)
    {
        _exceptionMessagesCollection = databaseResolver.GetDatabase().GetCollection<ExceptionMessage>(
            appOptions.Value.ExceptionMessagesCollectionName);
    }
    public Task<List<ExceptionMessage>> GetExceptionMessages(string serviceId)
    {
        return _exceptionMessagesCollection.Find(x => x.ServiceId == serviceId).ToListAsync();
    }

    public Task<List<ExceptionMessage>> GetExceptionMessages()
    {
        return _exceptionMessagesCollection.Find(x => true).ToListAsync();
    }

    public Task<List<ExceptionMessage>> GetExceptionMessages(DateTime from, DateTime to)
    {
        return _exceptionMessagesCollection.Find(x => x.DateTime >= from && x.DateTime <= to).ToListAsync();
    }

    public Task<List<ExceptionMessage>> GetExceptionMessages(string serviceId, DateTime from, DateTime to)
    {
        return _exceptionMessagesCollection.Find(x => x.DateTime >= from && x.DateTime <= to && x.ServiceId == serviceId).ToListAsync();
    }

    public Task AddExceptionMessage(ExceptionMessage message)
    {
        return _exceptionMessagesCollection.InsertOneAsync(message);
    }

    public Task RemoveAllActivationCodes(string serviceId)
    {
        return _exceptionMessagesCollection.DeleteManyAsync(x => x.ServiceId == serviceId);
    }
}