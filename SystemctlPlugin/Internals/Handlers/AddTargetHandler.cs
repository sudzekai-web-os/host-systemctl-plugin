using Sudzekai.LinCtrl.Abi.Managed;
using Sudzekai.LinCtrl.Core;
using SystemctlPlugin.Application.Dispatchering.Commands;
using SystemctlPlugin.Application.Dispatchering.Dispatchers;
using SystemctlPlugin.Application.Dto;

namespace SystemctlPlugin
{
    internal sealed class AddTargetHandler(IDispatcher dispatcher) : SystemctlRequestHandler
    {
        public override string Pattern => "systemctl/services/{name}/targets";

        public override HttpRequestMethod HttpMethod => HttpRequestMethod.Post;

        public override HandlerMetadata Metadata { get; } = new(
            string.Empty, "Добавление службы в цель systemd", ["systemctl"], "Добавление службы в цель systemd")
        {
            Parameters = [new("name", "path", SystemctlJsonContext.Default.String, "Имя службы", required: true)],
            RequestBody = new(SystemctlJsonContext.Default.TargetRequest),
            Responses = [new(200, LinCtrlContext.GetTypeInfo<ServiceDetails>()), new(400), new(401), new(403), new(404), new(409), new(413), new(415), new(503), new(504)]
        };

        protected override object Execute(HttpRequestDto request)
            => dispatcher.Execute<AddTargetCommand, ServiceDetails>(new(Name(request), ReadBody(request, SystemctlJsonContext.Default.TargetRequest)));
    }
}
