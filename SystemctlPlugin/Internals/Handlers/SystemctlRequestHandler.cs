using Sudzekai.LinCtrl.Abi.Managed;
using Sudzekai.LinCtrl.Assembly.Extensions;
using Sudzekai.LinCtrl.Core;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using SystemctlPlugin.Application.Services;

namespace SystemctlPlugin
{
    internal abstract class SystemctlRequestHandler : IRequestHandler
    {
        public abstract string Pattern { get; }

        public abstract HttpRequestMethod HttpMethod { get; }

        public abstract HandlerMetadata Metadata { get; }

        protected virtual bool RequiresAdministrator => HttpMethod != HttpRequestMethod.Get;

        public HttpResponseDto Handle(HttpRequestDto request)
        {
            if (request.Claims.GetValueOrDefault("sub") is not [var subject] || !long.TryParse(subject, out var id) || id <= 0)
                return Error(HttpStatusCode.Unauthorized, "Требуется действующий токен.");

            var roles = request.Claims.GetValueOrDefault("role") ?? [];
            if (!roles.Contains(nameof(ClientRole.Administrator))
                && (RequiresAdministrator || !roles.Any(role => role is nameof(ClientRole.Viewer) or nameof(ClientRole.User))))
                return Error(HttpStatusCode.Forbidden, "Недостаточно прав.");

            try
            {
                return HttpResponseDto.Create(HttpStatusCode.OK).AsJson(Execute(request));
            }
            catch (SystemctlException ex)
            {
                return Error(ex.StatusCode, ex.Message);
            }
            catch (JsonException)
            {
                return Error(HttpStatusCode.BadRequest, "Некорректное тело JSON.");
            }
            catch (UnauthorizedAccessException)
            {
                return Error(HttpStatusCode.Forbidden, "У процесса API нет доступа к файлам systemd.");
            }
            catch (IOException)
            {
                return Error(HttpStatusCode.Conflict, "Не удалось изменить файлы systemd.");
            }
        }

        protected static T ReadBody<T>(HttpRequestDto request, JsonTypeInfo<T> typeInfo) where T : class
        {
            if (request.ContentType != ResponseContentType.ApplicationJson)
                throw new SystemctlException(HttpStatusCode.UnsupportedMediaType, "Требуется Content-Type application/json.");

            if (request.Body.Length > 2 * 1024 * 1024)
                throw new SystemctlException(HttpStatusCode.RequestEntityTooLarge, "Тело запроса слишком велико.");

            return JsonSerializer.Deserialize(request.Body, typeInfo)
                ?? throw new SystemctlException(HttpStatusCode.BadRequest, "Требуется объект JSON.");
        }

        protected abstract object Execute(HttpRequestDto request);

        protected static string Name(HttpRequestDto request)
            => request.RouteValues.GetValueOrDefault("name") ?? "";

        private static HttpResponseDto Error(HttpStatusCode status, string message)
            => HttpResponseDto.CreateError(status, (int)status, message);
    }
}
