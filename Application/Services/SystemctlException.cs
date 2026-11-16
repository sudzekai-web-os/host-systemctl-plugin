using System.Net;

namespace SystemctlPlugin.Application.Services
{
    public sealed class SystemctlException(HttpStatusCode statusCode, string message) : Exception(message)
    {
        public HttpStatusCode StatusCode { get; } = statusCode;
    }
}
