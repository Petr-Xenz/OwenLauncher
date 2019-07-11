using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace OwenLauncher.Applications
{
    public class ApplicationModel: INotifyPropertyChanged
    {
        private bool _isInstalled;
        private readonly IInstallApplicationService _installService;

        public event PropertyChangedEventHandler PropertyChanged;

        public ApplicationModel(string name, InstallData installData, Bitmap icon, string installUrl, string historyUrl, IInstallApplicationService installService)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            InstallData = installData ?? throw new ArgumentNullException(nameof(installData));
            Icon = icon ?? throw new ArgumentNullException(nameof(icon));
            InstallUrl = installUrl ?? throw new ArgumentNullException(nameof(installUrl));
            HistoryUrl = historyUrl ?? throw new ArgumentNullException(nameof(historyUrl));
            _installService = installService;
        }

        public string Name { get; }
        public InstallData InstallData { get; }
        public string Version { get; private set; }

        public string ExecutablePath { get; private set; }
        public string UninstallCommand { get; private set; }
        public bool IsInstalled
        {
            get => _isInstalled;
            private set
            {
                if (_isInstalled == value)
                    return;
                _isInstalled = value;
                RaiseNotifyPropertyChanged(nameof(IsInstalled));

            }
        }

        public bool CanInstallApp => _installService != null;

        private void RaiseNotifyPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public Bitmap Icon { get; }
        public string InstallUrl { get; }
        public string HistoryUrl { get; }

        public void UpdateInstallInfo(string executablePath, string uninstallCommand, string version)
        {
            IsInstalled = !string.IsNullOrEmpty(executablePath);
            ExecutablePath = executablePath;
            UninstallCommand = uninstallCommand;
            Version = version;
            RaiseNotifyPropertyChanged(nameof(Version));
        }

        public async Task InstallApplication()
        {
            if (_installService is null)
                throw new ArgumentException(nameof(_installService));

            var isInstalled = await _installService.InstallApplicationAsync(InstallUrl, InstallData.SilentInstall);

            if (isInstalled) //TODO
            {
                var serv = new RegistryInstalledApplicationsService();
                serv.UpdateInstallStatus(this);
            }
            else
            {
                MessageBox.Show("install error");
            }
        }

        public void StartApp()
        {
            try
            {
                var info = new ProcessStartInfo(ExecutablePath);
                using (var process = Process.Start(info))
                {

                }
            }
            catch (Exception)
            {
                //TODO
            }
        }

        public void DeleteApp()
        {
            var command = UninstallCommand;
            var args = "";
            if (!command.StartsWith("\""))
            {
                command = command.Split(' ').First();
                args = UninstallCommand.Split(' ').Skip(1).Aggregate((p, c) => $"{p} {c}");
            }
            var info = new ProcessStartInfo(command, args);
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