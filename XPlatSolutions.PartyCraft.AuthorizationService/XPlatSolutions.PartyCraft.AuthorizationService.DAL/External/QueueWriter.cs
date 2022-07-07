using XPlatSolutions.PartyCraft.AuthorizationService.DAL.Interfaces.External;

namespace XPlatSolutions.PartyCraft.AuthorizationService.DAL.External;

public class QueueWriter : IQueueWriter
{
    public Task WriteEmailMessageTask(string email, string message)
    {
        return Task.FromResult(() => { });
    }
}