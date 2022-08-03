namespace XPlatSolutions.PartyCraft.AuthorizationService.DAL.Interfaces.External;

public interface IQueueWriter
{
    void WriteEmailMessageTask(string email, string message, string subject);
}