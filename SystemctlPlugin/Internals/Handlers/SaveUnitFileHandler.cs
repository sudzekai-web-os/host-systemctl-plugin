using Sudzekai.LinCtrl.Abi.Managed;
using Sudzekai.LinCtrl.Core;
using SystemctlPlugin.Application.Dispatchering.Commands;
using SystemctlPlugin.Application.Dispatchering.Dispatchers;
using SystemctlPlugin.Application.Dto;

namespace SystemctlPlugin
{
    internal sealed class SaveUnitFileHandler(IDispatcher dispatcher) : SystemctlRequestHandler
    {
        public override string Pattern => "systemctl/services/{name}/file";

        public override HttpRequestMethod HttpMethod => HttpRequestMethod.Put;

        public override HandlerMetadata Metadata { get; } = new(
            string.Empty, "Редактирование файла службы", ["systemctl"], "Редактирование файла службы")
        {
            Parameters = [new("name", "path", SystemctlJsonContext.Default.String, "Имя службы", required: true)],
            RequestBody = new(SystemctlJsonContext.Default.UnitFileRequest),
            Responses = [new(200, LinCtrlContext.GetTypeInfo<ServiceDetails>()), new(400), new(401), new(403), new(404), new(409), new(413), new(415), new(503), new(504)]
        };

        protected override object Execute(HttpRequestDto request)
            => dispatcher.Execute<SaveUnitFileCommand, ServiceDetails>(new(Name(request), ReadBody(request, SystemctlJsonContext.Default.UnitFileRequest)));
    }
}
