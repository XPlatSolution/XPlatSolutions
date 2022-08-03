namespace XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Enums;

public enum StatusCode : byte
{
    Success = 0,
    HandledError,
    CriticalError
}