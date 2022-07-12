using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using XPlatSolutions.PartyCraft.EventBus.Interfaces;
using XPlatSolutions.PartyCraft.SpamService.DAL.Interfaces.Mail;
using XPlatSolutions.PartyCraft.SpamService.Domain.Core.Classes;
using XPlatSolutions.PartyCraft.SpamService.Domain.Core.Enums;
using XPlatSolutions.PartyCraft.SpamService.Domain.Core.Interfaces;
using XPlatSolutions.PartyCraft.SpamService.Domain.Core.Models;

namespace XPlatSolutions.PartyCraft.SpamService.DAL.Mail;

public class SmtpSenderService : ISmtpSenderService
{
    private readonly IOptions<AppOptions> _appOptions;
    private IEventBusResolver<EventBusTypes> _eventBusResolver;
    private readonly IServiceInfoResolver _serviceInfoResolver;

    public SmtpSenderService(IOptions<AppOptions> appOptions, IEventBusResolver<EventBusTypes> eventBusResolver, 
        IServiceInfoResolver serviceInfoResolver)
    {
        _appOptions = appOptions;
        _eventBusResolver = eventBusResolver;
        _serviceInfoResolver = serviceInfoResolver;
    }

    public async Task SendMessage(MessageEvent messageEvent)
    {
        try
        {
            var smtp = new SmtpClient
            {
                Port = _appOptions.Value.SmtpPort,
                Host = _appOptions.Value.SmtpHost,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_appOptions.Value.EmailFrom, _appOptions.Value.Password),
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            var message = new MailMessage
            {
                From = new MailAddress(_appOptions.Value.EmailFrom),
                Subject = messageEvent.Subject,
                IsBodyHtml = true,
                Body = messageEvent.Text
            };

            message.To.Add(new MailAddress(messageEvent.Email));
            await smtp.SendMailAsync(message);
        }
        catch (Exception ex)
        {
            var message = new ExceptionMessageEvent
            {
                DateTime = DateTime.UtcNow,
                Guid = _serviceInfoResolver.GetServiceGuid(),
                Service = _serviceInfoResolver.GetServiceName(),
                IsCritical = true,
                Stacktrace = ex.StackTrace ?? string.Empty,
                Text = ex.Message
            };
            _eventBusResolver.Resolve(EventBusTypes.AnalyticsBus)?.Publish(message);
        }
    }
}