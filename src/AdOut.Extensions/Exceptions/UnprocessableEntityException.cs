using System;

namespace AdOut.Extensions.Exceptions
{
    public class UnprocessableEntityException : Exception, IHttpException
    {
        public UnprocessableEntityException(string message = null) : base(message)
        {
        }

        public int HttpStatusCode => (int)System.Net.HttpStatusCode.UnprocessableEntity;
    }
}
