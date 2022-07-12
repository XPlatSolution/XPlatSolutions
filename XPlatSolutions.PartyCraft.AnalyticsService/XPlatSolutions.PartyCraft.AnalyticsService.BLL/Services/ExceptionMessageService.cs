using XPlatSolutions.PartyCraft.AnalyticsService.BLL.Interfaces.Services;
using XPlatSolutions.PartyCraft.AnalyticsService.DAL.Interfaces.Dao;
using XPlatSolutions.PartyCraft.AnalyticsService.Domain.Core.Models;
using XPlatSolutions.PartyCraft.AnalyticsService.Domain.Core.Requests;
using XPlatSolutions.PartyCraft.AnalyticsService.Domain.Core.Responses;

namespace XPlatSolutions.PartyCraft.AnalyticsService.BLL.Services;

public class ExceptionMessageService : IExceptionMessageService
{
    private readonly IExceptionMessageAccess _exceptionMessageAccess;

    public ExceptionMessageService(IExceptionMessageAccess exceptionMessageAccess)
    {
        _exceptionMessageAccess = exceptionMessageAccess;
    }

    public async Task<ExceptionsResponse> GetExceptionMessagesList(ExceptionsRequest request)
    {
        if (request == null)
            return new ExceptionsResponse
            {
                Exceptions = await _exceptionMessageAccess.GetExceptionMessages()
            };

        if (string.IsNullOrWhiteSpace(request.ServiceId))
        {
            if (request.From == null && request.To == null)
                return new ExceptionsResponse
                {
                    Exceptions = await _exceptionMessageAccess.GetExceptionMessages()
                };

            if (request.From == null && request.To != null)
                return new ExceptionsResponse
                {
                    Exceptions = await _exceptionMessageAccess.GetExceptionMessages(DateTime.MinValue, request.To.Value)
                };

            if (request.From != null && request.To == null)
                return new ExceptionsResponse
                {
                    Exceptions = await _exceptionMessageAccess.GetExceptionMessages(request.From.Value, DateTime.MaxValue)
                };

            if (request.From != null && request.To != null)
                return new ExceptionsResponse
                {
                    Exceptions = await _exceptionMessageAccess.GetExceptionMessages(request.From.Value, request.To.Value)
                };

        }
        else
        {
            if (request.From == null && request.To == null)
                return new ExceptionsResponse
                {
                    Exceptions = await _exceptionMessageAccess.GetExceptionMessages(request.ServiceId)
                };

            if (request.From == null && request.To != null)
                return new ExceptionsResponse
                {
                    Exceptions = await _exceptionMessageAccess.GetExceptionMessages(request.ServiceId, DateTime.MinValue, request.To.Value)
                };

            if (request.From != null && request.To == null)
                return new ExceptionsResponse
                {
                    Exceptions =
                        await _exceptionMessageAccess.GetExceptionMessages(request.ServiceId, request.From.Value, DateTime.MaxValue)
                };

            if (request.From != null && request.To != null)
                return new ExceptionsResponse
                {
                    Exceptions =
                        await _exceptionMessageAccess.GetExceptionMessages(request.ServiceId, request.From.Value, request.To.Value)
                };
        }

        return new ExceptionsResponse
        {
            Exceptions = await _exceptionMessageAccess.GetExceptionMessages()
        };
    }

    public async Task<List<ExceptionMessage>> GetExceptionMessagesList()
    {
        return await _exceptionMessageAccess.GetExceptionMessages();
    }

    public async Task<List<ExceptionMessage>> GetExceptionMessagesList(string id)
    {
        return await _exceptionMessageAccess.GetExceptionMessages(id);
    }
}