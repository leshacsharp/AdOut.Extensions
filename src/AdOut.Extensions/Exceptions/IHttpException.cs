namespace AdOut.Extensions.Exceptions
{
    public interface IHttpException
    {
        int HttpStatusCode { get; }
    }
}
