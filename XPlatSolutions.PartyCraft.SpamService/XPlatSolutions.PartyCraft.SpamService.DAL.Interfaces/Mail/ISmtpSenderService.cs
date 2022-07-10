using XPlatSolutions.PartyCraft.SpamService.Domain.Core.Models;

namespace XPlatSolutions.PartyCraft.SpamService.DAL.Interfaces.Mail;

public interface ISmtpSenderService
{
    Task SendMessage(MessageEvent messageEvent);
}