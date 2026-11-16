using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using SystemctlPlugin.Application.Dto;

namespace SystemctlPlugin.Application.Services
{
    internal sealed partial class SystemctlClient(IConfiguration configuration)
    {
        private readonly HashSet<string> _allowed = configuration.GetSection("Systemctl:AllowedUnits").GetChildren()
            .Select(section => section.Value).OfType<string>().ToHashSet(StringComparer.Ordinal);

        public ServiceInfo[] List()
        {
            var active = Run("list-units", "--all", "--type=service", "--no-legend", "--plain", "--no-pager")
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Trim().Split(' ', 5, StringSplitOptions.RemoveEmptyEntries))
                .Where(fields => fields.Length >= 4 && UnitPattern().IsMatch(fields[0]))
                .ToDictionary(fields => fields[0], fields => new ServiceInfo(fields[0], fields[1], fields[2], fields[3], fields.Length > 4 ? fields[4] : ""), StringComparer.Ordinal);
            foreach (var line in Run("list-unit-files", "--type=service", "--no-legend", "--no-pager").Split('\n', StringSplitOptions.RemoveEmptyEntries))
            {
                var name = line.Trim().Split(' ', 2)[0];
                if (UnitPattern().IsMatch(name))
                    active.TryAdd(name, new(name, "", "", "", ""));
            }
            return [.. active.Values.OrderBy(unit => unit.Name, StringComparer.Ordinal)];
        }

        public ServiceDetails Inspect(string name)
        {
            Validate(name);
            var values = Run("show", "--property=Id,Description,LoadState,ActiveState,SubState,UnitFileState,FragmentPath,Restart,WantedBy,RequiredBy", "--", name)
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Split('=', 2))
                .Where(pair => pair.Length == 2)
                .ToDictionary(pair => pair[0], pair => pair[1], StringComparer.Ordinal);
            var get = (string key) => values.GetValueOrDefault(key, "");
            if (get("Id").Length == 0 || get("LoadState") == "not-found")
                throw new SystemctlException(HttpStatusCode.NotFound, "Служба не найдена.");
            var canManage = _allowed.Contains(name) && name == get("Id");
            var path = Path.Combine(UnitFileStore.DirectoryPath, name);
            var targets = (get("WantedBy") + " " + get("RequiredBy")).Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Where(target => TargetPattern().IsMatch(target)).Distinct(StringComparer.Ordinal).Order(StringComparer.Ordinal);
            return new(get("Id"), get("Description"), get("LoadState"), get("ActiveState"), get("SubState"),
                get("UnitFileState"), canManage, canManage && get("FragmentPath") == path && UnitFileStore.IsRegular(path),
                get("Restart"), [.. targets]);
        }

        public ServiceDetails Change(ServiceOperation operation)
        {
            if (operation.Verb is not ("start" or "stop" or "restart" or "enable" or "disable"))
                throw new SystemctlException(HttpStatusCode.BadRequest, "Неизвестная операция со службой.");

            lock (_configurationLock)
            {
                EnsureManageable(operation.Name);
                Run(operation.Verb, "--", operation.Name);
                return Inspect(operation.Name);
            }
        }

        private static string Run(params string[] arguments)
            => RunAsync(arguments).GetAwaiter().GetResult();

        private static async Task<string> RunAsync(string[] arguments)
        {
            if (!OperatingSystem.IsLinux() || !File.Exists("/usr/bin/systemctl"))
                throw new SystemctlException(HttpStatusCode.ServiceUnavailable, "systemctl недоступен.");

            var start = new ProcessStartInfo("/usr/bin/systemctl")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };
            start.Environment["LC_ALL"] = "C";
            start.Environment["SYSTEMD_COLORS"] = "0";
            start.ArgumentList.Add("--no-ask-password");
            foreach (var argument in arguments)
                start.ArgumentList.Add(argument);

            using var process = Process.Start(start)
                ?? throw new SystemctlException(HttpStatusCode.ServiceUnavailable, "Не удалось запустить systemctl.");
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(12));
            var output = process.StandardOutput.ReadToEndAsync();
            var errors = process.StandardError.ReadToEndAsync();
            try
            {
                await process.WaitForExitAsync(timeout.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                if (!process.HasExited)
                {
                    try { process.Kill(); }
                    catch (InvalidOperationException) { }
                }

                await process.WaitForExitAsync().ConfigureAwait(false);
                await Task.WhenAll(output, errors).ConfigureAwait(false);
                throw new SystemctlException(HttpStatusCode.GatewayTimeout, "systemctl не ответил вовремя.");
            }

            var result = await output.ConfigureAwait(false);
            var error = await errors.ConfigureAwait(false);
            if (process.ExitCode == 0)
                return result;

            if (error.Contains("could not be found", StringComparison.OrdinalIgnoreCase) || error.Contains("not found", StringComparison.OrdinalIgnoreCase))
                throw new SystemctlException(HttpStatusCode.NotFound, "Служба не найдена.");

            throw new SystemctlException(HttpStatusCode.Conflict, "systemctl отклонил операцию.");
        }

        private static void Validate(string name)
        {
            if (string.IsNullOrEmpty(name) || !UnitPattern().IsMatch(name))
                throw new SystemctlException(HttpStatusCode.BadRequest, "Укажите имя службы с суффиксом .service.");
        }

        [GeneratedRegex("^[A-Za-z0-9][A-Za-z0-9_.@:-]{0,246}\\.service$")]
        private static partial Regex UnitPattern();
    }
}
