namespace Refresh.Core.Types.BlueSphere;

public class BsOAuthException : Exception
{
    public BsOAuthException(string message) : base(message)
    {
        // do nothing else for now
    }
}