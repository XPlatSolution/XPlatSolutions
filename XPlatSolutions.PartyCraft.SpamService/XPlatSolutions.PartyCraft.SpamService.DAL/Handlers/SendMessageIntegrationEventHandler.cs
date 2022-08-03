using XPlatSolutions.PartyCraft.EventBus.Interfaces;
using XPlatSolutions.PartyCraft.SpamService.DAL.Interfaces.Mail;
using XPlatSolutions.PartyCraft.SpamService.Domain.Core.Models;

namespace XPlatSolutions.PartyCraft.SpamService.DAL.Handlers;

public class SendMessageIntegrationEventHandler : IIntegrationEventHandler<MessageEvent>
{
    private readonly ISmtpSenderService _smtpSenderService;

    public SendMessageIntegrationEventHandler(ISmtpSenderService smtpSenderService)
    {
        _smtpSenderService = smtpSenderService;
    }

    public async Task Handle(MessageEvent ev)
    {
        //TODO: добавить идемпотентность
        await _smtpSenderService.SendMessage(ev);
    }
}