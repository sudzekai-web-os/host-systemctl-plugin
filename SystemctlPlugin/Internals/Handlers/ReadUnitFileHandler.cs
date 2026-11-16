using Sudzekai.LinCtrl.Abi.Managed;
using Sudzekai.LinCtrl.Core;
using SystemctlPlugin.Application.Dispatchering.Commands;
using SystemctlPlugin.Application.Dispatchering.Dispatchers;
using SystemctlPlugin.Application.Dto;

namespace SystemctlPlugin
{
    internal sealed class ReadUnitFileHandler(IDispatcher dispatcher) : SystemctlRequestHandler
    {
        public override string Pattern => "systemctl/services/{name}/file";

        public override HttpRequestMethod HttpMethod => HttpRequestMethod.Get;

        protected override bool RequiresAdministrator => true;

        public override HandlerMetadata Metadata { get; } = new(
            string.Empty, "Чтение файла службы", ["systemctl"], "Чтение файла службы")
        {
            Parameters = [new("name", "path", SystemctlJsonContext.Default.String, "Имя службы", required: true)],
            Responses = [new(200, LinCtrlContext.GetTypeInfo<string>()), new(400), new(401), new(403), new(404), new(409), new(413), new(415), new(503), new(504)]
        };

        protected override object Execute(HttpRequestDto request)
            => dispatcher.Execute<ReadUnitFileCommand, string>(new(Name(request)));
    }
}
