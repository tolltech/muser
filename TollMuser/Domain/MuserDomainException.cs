using System;

namespace Tolltech.Muser.Domain
{
    public class MuserDomainException : Exception
    {
        public MuserDomainException(string msg) : base(msg)
        {
            
        }
    }
}