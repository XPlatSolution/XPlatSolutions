using System.Globalization;

namespace XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Exceptions;

public class XPlatSolutionsException : Exception
{
    public XPlatSolutionsException(string message) : base(message) { }

    public XPlatSolutionsException(string message, params object[] args)
        : base(string.Format(CultureInfo.InvariantCulture, message, args))
    {
    }
}