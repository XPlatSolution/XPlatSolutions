using System.Globalization;

namespace XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Exceptions;

public class AuthenticateException : XPlatSolutionsException
{
    public AuthenticateException(string message) : base(message) { }

    public AuthenticateException(string message, params object[] args)
        : base(string.Format(CultureInfo.InvariantCulture, message, args))
    {
    }
}