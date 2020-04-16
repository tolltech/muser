using System;

namespace Tolltech.Musync.Domain
{
    public class VkAuthorizeException : AuthorizeException
    {
        
    }

    public class YaAuthorizeException : AuthorizeException
    {

    }

    public class MuserAuthorizeException : AuthorizeException
    {

    }

    public abstract class AuthorizeException : Exception
    {
        
    }
}