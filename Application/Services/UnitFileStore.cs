using System.Net;
using System.Text;

namespace SystemctlPlugin.Application.Services
{
    internal static class UnitFileStore
    {
        public const string DirectoryPath = "/etc/systemd/system";
        public const int MaximumSize = 256 * 1024;

        public static bool IsRegular(string path)
        {
            var file = new FileInfo(path);
            return file.Exists && (file.Attributes & (FileAttributes.ReparsePoint | FileAttributes.Directory | FileAttributes.Device)) == 0;
        }

        public static string Read(string path)
        {
            if (!IsRegular(path))
                throw new SystemctlException(HttpStatusCode.Forbidden, "Редактировать можно только локальный файл службы.");

            using var stream = File.OpenRead(path);
            var data = new byte[MaximumSize + 1];
            var count = stream.ReadAtLeast(data, data.Length, false);
            if (count > MaximumSize)
                throw new SystemctlException(HttpStatusCode.RequestEntityTooLarge, "Файл службы превышает 256 КиБ.");

            return Encoding.UTF8.GetString(data, 0, count);
        }

        public static void Write(string path, string content, bool overwrite)
        {
            var temporary = Path.Combine(Path.GetDirectoryName(path)!, $".linctrl-{Guid.NewGuid():N}.tmp");
            try
            {
                using (var stream = new FileStream(temporary, FileMode.CreateNew, FileAccess.Write))
                using (var writer = new StreamWriter(stream, new UTF8Encoding(false)))
                    writer.Write(content);

                if (OperatingSystem.IsLinux())
                    File.SetUnixFileMode(temporary, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.GroupRead | UnixFileMode.OtherRead);

                File.Move(temporary, path, overwrite);
            }
            finally
            {
                File.Delete(temporary);
            }
        }
    }
}
