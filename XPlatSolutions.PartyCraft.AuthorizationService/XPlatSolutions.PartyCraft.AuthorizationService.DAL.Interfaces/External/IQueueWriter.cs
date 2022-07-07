namespace XPlatSolutions.PartyCraft.AuthorizationService.DAL.Interfaces.External;

public interface IQueueWriter
{
    Task WriteEmailMessageTask(string email, string message);
}