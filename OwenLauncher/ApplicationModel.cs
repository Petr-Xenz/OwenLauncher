using System;
using System.Diagnostics;

namespace OwenLauncher
{
    public class ApplicationModel
    {
        private readonly string _uninstallCommand;

        public ApplicationModel(string name, string version, string executablePath, string uninstallCommand)
        {
            _uninstallCommand = uninstallCommand ?? throw new ArgumentNullException(nameof(uninstallCommand));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Version = version ?? throw new ArgumentNullException(nameof(version));
            ExecutablePath = executablePath ?? throw new ArgumentNullException(nameof(executablePath));
        }

        public string Name { get; }

        public string Version { get; }

        public string ExecutablePath { get; }

        public void StartApp()
        {
            var info = new ProcessStartInfo(ExecutablePath);
            using (var process = Process.Start(info))
            {
                
            }
        }

        public void DeleteApp()
        {

        }
    }
}