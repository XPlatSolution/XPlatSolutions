using Microsoft.Extensions.Options;
using MongoDB.Driver;
using XPlatSolutions.PartyCraft.AnalyticsService.DAL.Interfaces.Dao;
using XPlatSolutions.PartyCraft.AnalyticsService.Domain.Core.Classes;
using XPlatSolutions.PartyCraft.AnalyticsService.Domain.Core.Models;

namespace XPlatSolutions.PartyCraft.AnalyticsService.DAL.Dao;

public class ServiceAccess : IServiceAccess
{
    private readonly IMongoCollection<Service> _servicesCollection;

    public ServiceAccess(IDatabaseResolver databaseResolver, IOptions<AppOptions> appOptions)
    {
        _servicesCollection = databaseResolver.GetDatabase().GetCollection<Service>(
            appOptions.Value.ServicesCollectionName);
    }
    public Task<List<Service>> GetServices()
    {
        return _servicesCollection.Find(x => true).ToListAsync();
    }

    public Task<Service> GetService(string guid)
    {
        return _servicesCollection.Find(x => x.Guid == guid).FirstOrDefaultAsync();
    }

    public Task AddService(Service service)
    {
        return _servicesCollection.InsertOneAsync(service);
    }

    public Task RemoveService(string serviceId)
    {
        return _servicesCollection.DeleteManyAsync(x => x.Id == serviceId);
    }
}