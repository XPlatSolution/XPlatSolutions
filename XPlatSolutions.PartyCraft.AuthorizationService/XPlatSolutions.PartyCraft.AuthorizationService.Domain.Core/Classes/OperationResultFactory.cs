using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Enums;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Interfaces;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Models;

namespace XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Classes;

public class OperationResultFactory : IOperationResultFactory
{
    public OperationResult<T> CreateOperationResult<T>(T? result, StatusCode status = StatusCode.Success, string? message = null)
    {
        return new OperationResult<T>
        {
            Message = message,
            Status = status,
            Result = result
        };
    }

    public OperationResult CreateOperationResult(StatusCode status = StatusCode.Success, string? message = null)
    {
        return new OperationResult
        {
            Message = message,
            Status = status
        };
    }
}