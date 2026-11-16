using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using SystemctlPlugin.Application.Dto;

namespace SystemctlPlugin.Application.Services
{
    internal sealed partial class SystemctlClient
    {
        private readonly object _configurationLock = new();

        public string[] ListTargets()
            => [.. Run("list-unit-files", "--type=target", "--no-legend", "--no-pager")
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Trim().Split(' ', 2)[0])
                .Where(name => TargetPattern().IsMatch(name))
                .Distinct(StringComparer.Ordinal)
                .Order(StringComparer.Ordinal)];

        public ServiceInfo[] ListTargetServices(string target)
        {
            ValidateTarget(target);

            var dependencies = Run("show", "--property=Wants,Requires", "--value", "--", target)
                .Split([' ', '\n', '\r'], StringSplitOptions.RemoveEmptyEntries)
                .ToHashSet(StringComparer.Ordinal);
            return [.. List().Where(unit => dependencies.Contains(unit.Name))];
        }

        public string ReadUnitFile(string name)
        {
            lock (_configurationLock)
                return UnitFileStore.Read(LocalPath(name));
        }

        public ServiceDetails SaveUnitFile(string name, string content, bool create)
        {
            ValidateAllowed(name);
            if (string.IsNullOrWhiteSpace(content) || content.Contains('\0'))
                throw new SystemctlException(HttpStatusCode.BadRequest, "Введите содержимое файла службы.");

            if (Encoding.UTF8.GetByteCount(content) > UnitFileStore.MaximumSize)
                throw new SystemctlException(HttpStatusCode.RequestEntityTooLarge, "Файл службы превышает 256 КиБ.");

            lock (_configurationLock)
            {
                var path = Path.Combine(UnitFileStore.DirectoryPath, name);
                string? previous = null;
                if (create)
                {
                    if (List().Any(unit => unit.Name == name) || new FileInfo(path).LinkTarget is not null || Path.Exists(path))
                        throw new SystemctlException(HttpStatusCode.Conflict, "Служба с таким именем уже существует.");
                }
                else
                    previous = UnitFileStore.Read(LocalPath(name));

                UnitFileStore.Write(path, content, overwrite: !create);
                try
                {
                    Run("daemon-reload");
                    var service = Inspect(name);
                    if (service.LoadState != "loaded")
                        throw new SystemctlException(HttpStatusCode.BadRequest, "systemd не удалось загрузить файл службы.");

                    return service;
                }
                catch (SystemctlException)
                {
                    if (previous is null)
                        File.Delete(path);
                    else
                        UnitFileStore.Write(path, previous, overwrite: true);

                    Run("daemon-reload");
                    throw;
                }
            }
        }

        public bool DeleteUnitFile(string name)
        {
            lock (_configurationLock)
            {
                var path = LocalPath(name);
                Run("stop", "--", name);
                Run("disable", "--", name);
                var restartFile = RestartPath(name);
                if (UnitFileStore.IsRegular(restartFile))
                    File.Delete(restartFile);

                File.Delete(path);
                Run("daemon-reload");
                return true;
            }
        }

        public ServiceDetails SetRestartPolicy(string name, bool enabled)
        {
            lock (_configurationLock)
            {
                EnsureManageable(name);

                var path = RestartPath(name);
                var directory = Path.GetDirectoryName(path)!;
                if (new DirectoryInfo(directory).LinkTarget is not null || new FileInfo(path).LinkTarget is not null)
                    throw new SystemctlException(HttpStatusCode.Forbidden, "Путь настройки авторестарта содержит символическую ссылку.");

                Directory.CreateDirectory(directory);
                var previous = UnitFileStore.IsRegular(path) ? UnitFileStore.Read(path) : null;
                UnitFileStore.Write(path, enabled ? "[Service]\nRestart=on-failure\n" : "[Service]\nRestart=no\n", overwrite: previous is not null);
                try
                {
                    Run("daemon-reload");
                    return Inspect(name);
                }
                catch (SystemctlException)
                {
                    if (previous is null)
                        File.Delete(path);
                    else
                        UnitFileStore.Write(path, previous, overwrite: true);

                    Run("daemon-reload");
                    throw;
                }
            }
        }

        public ServiceDetails AddTarget(string name, string target)
        {
            lock (_configurationLock)
            {
                EnsureManageable(name);
                ValidateTarget(target);

                Run("add-wants", "--", target, name);
                return Inspect(name);
            }
        }

        private void ValidateTarget(string target)
        {
            if (string.IsNullOrEmpty(target) || !TargetPattern().IsMatch(target) || !ListTargets().Contains(target, StringComparer.Ordinal))
                throw new SystemctlException(HttpStatusCode.BadRequest, "Выберите существующий target.");
        }

        private string LocalPath(string name)
        {
            var service = EnsureManageable(name);
            if (!service.CanEdit)
                throw new SystemctlException(HttpStatusCode.Forbidden, "Изменять и удалять можно только локальные файлы служб.");

            return Path.Combine(UnitFileStore.DirectoryPath, name);
        }

        private ServiceDetails EnsureManageable(string name)
        {
            ValidateAllowed(name);
            var service = Inspect(name);
            if (!service.CanManage || service.Name != name)
                throw new SystemctlException(HttpStatusCode.Forbidden, "Укажите каноническое имя разрешённой службы.");

            return service;
        }

        private void ValidateAllowed(string name)
        {
            Validate(name);
            if (!_allowed.Contains(name))
                throw new SystemctlException(HttpStatusCode.Forbidden, "Служба отсутствует в списке разрешённых.");
        }

        private static string RestartPath(string name)
            => Path.Combine(UnitFileStore.DirectoryPath, name + ".d", "90-linctrl-restart.conf");

        [GeneratedRegex("^[A-Za-z0-9][A-Za-z0-9_.@:-]{0,247}\\.target$")]
        private static partial Regex TargetPattern();
    }
}
