using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Enums;

namespace XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Models;

public class OperationResult
{
    public StatusCode Status { get; init; }
    public string? Message { get; init; }
}

public class OperationResult<T> : OperationResult
{
    public T? Result { get; init; }
}