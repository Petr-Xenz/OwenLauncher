using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace OwenLauncher.Applications
{
    public class ApplicationModel
    {
        public ApplicationModel(string name, string installId, Bitmap icon, string updateUrl, string historyUrl)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            InstallId = installId ?? throw new ArgumentNullException(nameof(installId));
            Icon = icon ?? throw new ArgumentNullException(nameof(icon));
            UpdateUrl = updateUrl ?? throw new ArgumentNullException(nameof(updateUrl));
            HistoryUrl = historyUrl ?? throw new ArgumentNullException(nameof(historyUrl));
        }

        public string Name { get; }
        public string InstallId { get; }
        public string Version { get; private set; }

        public string ExecutablePath { get; private set; }
        public string UninstallCommand { get; private set; }
        public bool IsInstalled { get; private set; }

        public Bitmap Icon { get; }
        public string UpdateUrl { get; }
        public string HistoryUrl { get; }

        public void UpdateInstallInfo(string executablePath, string uninstallCommand, string version)
        {
            IsInstalled = !string.IsNullOrEmpty(executablePath);
            ExecutablePath = executablePath;
            UninstallCommand = uninstallCommand;
            Version = version;
        }

        public void StartApp()
        {
            var info = new ProcessStartInfo(ExecutablePath);
            using (var process = Process.Start(info))
            {

            }
        }

        public void DeleteApp()
        {
            var info = new ProcessStartInfo(UninstallCommand);
            var process = new Process();
            process.StartInfo = info;
            process.EnableRaisingEvents = true;
            process.Start();
            process.Exited += (s, e) =>
            {
                if (process.ExitCode == 0)
                {
                    UpdateInstallInfo("", "", "");
                }
                process.Dispose();
            };
        }
    }
}