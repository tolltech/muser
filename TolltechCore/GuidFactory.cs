using System;

namespace TolltechCore
{
    public class GuidFactory : IGuidFactory
    {
        public Guid Create() => Guid.NewGuid();
    }
}