using System;

namespace AdOut.Extensions.Exceptions
{
    public class ForbiddenException : Exception, IHttpException
    {
        public ForbiddenException(string message = null) : base(message)
        {
        }

        public int HttpStatusCode => (int)System.Net.HttpStatusCode.Forbidden;
    }
}
