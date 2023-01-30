using System;

namespace Carpeddit.Api.Exceptions
{
    public sealed class ServiceDownException : Exception
    {
        public ServiceDownException() 
            : base("The web service is experiencing some outages, or doesn't appear to function correctly.") { }
    }
}
