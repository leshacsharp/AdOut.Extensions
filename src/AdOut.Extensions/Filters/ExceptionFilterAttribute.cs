using AdOut.Extensions.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Net.Mime;

namespace AdOut.Extensions.Filters
{
    public class ExceptionFilterAttribute : Attribute, IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is IHttpException)
            {
                var exception = context.Exception;
                var httpStatusCode = ((IHttpException)exception).HttpStatusCode;

                context.Result = new ContentResult()
                {
                    StatusCode = httpStatusCode,
                    ContentType = MediaTypeNames.Text.Plain,
                    Content = exception.Message
                };
            }
        }
    }
}
