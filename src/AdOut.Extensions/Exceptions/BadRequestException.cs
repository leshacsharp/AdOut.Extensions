using System;

namespace AdOut.Extensions.Exceptions
{
    public class BadRequestException : Exception, IHttpException
    {
        public BadRequestException(string message) : base(message)
        {
        }

        public int HttpStatusCode => (int)System.Net.HttpStatusCode.BadRequest;
    }
}
