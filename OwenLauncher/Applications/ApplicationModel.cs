using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace OwenLauncher.Applications
{
    public class ApplicationModel: INotifyPropertyChanged
    {
        private bool _isInstalled;
        private readonly IInstallApplicationService _installService;
        private readonly IVersionCheckerService _versionChecker;

        public event PropertyChangedEventHandler PropertyChanged;

        public ApplicationModel(string name, string installId, Bitmap icon, string historyUrl, IInstallApplicationService installService, IVersionCheckerService versionChecker)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            InstallId = installId ?? throw new ArgumentNullException(nameof(installId));
            Icon = icon ?? throw new ArgumentNullException(nameof(icon));
            HistoryUrl = historyUrl ?? throw new ArgumentNullException(nameof(historyUrl));
            _installService = installService;
            _versionChecker = versionChecker;
        }

        public string Name { get; }
        public string InstallId { get; }
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

            var isInstalled = await _installService.InstallApplication("/VERYSILENT /SUPPRESSMSGBOXES");

            if (isInstalled) //TODO
            {
                var serv = new RegistryInstalledApplicationsService();
                serv.UpdateInstallStatus(this);
            }
        }

        public async Task<(bool isLast, string lastVersion)> IsLastVersion()
        {
            if (_versionChecker is null)
                return (true, Version);

            return await _versionChecker.IsLastVersion(System.Version.Parse(Version));
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