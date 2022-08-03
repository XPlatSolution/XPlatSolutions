using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Enums;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Models;

namespace XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Interfaces;

public interface IOperationResultFactory
{
    OperationResult<T> CreateOperationResult<T>(T? result, StatusCode status = StatusCode.Success,
        string? message = null);
    OperationResult CreateOperationResult(StatusCode status = StatusCode.Success,
        string? message = null);
}