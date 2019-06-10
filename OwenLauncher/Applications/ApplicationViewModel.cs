using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;

namespace OwenLauncher.Applications
{
    [POCOViewModel]
    public class ApplicationViewModel
    {
        private readonly ApplicationModel _model;
        public ImageSource Image { get; }
        public string Version => _model.Version;
        public string Name => _model.Name;

        public ICommand LaunchAppCommand { get; }
        public ICommand DeleteAppCommand { get; }
        public ICommand InstallAppCommand { get; }
        public ICommand UpdateAppCommand { get; }

        public bool IsCanUpdate { get; private set; }

        public string LastVersion { get; private set; }

        public string UpdateInfoUri => _model.HistoryUrl;

        protected ApplicationViewModel(ApplicationModel model)
        {
            _model = model;
            _model.PropertyChanged += (s, e) => this.RaisePropertiesChanged();
            Image = model.Icon.ToImageSource();
            LaunchAppCommand = new DelegateCommand(_model.StartApp, () => _model.IsInstalled);
            DeleteAppCommand = new DelegateCommand(_model.DeleteApp, () => _model.IsInstalled);
            InstallAppCommand = new AsyncCommand(_model.InstallApplication, () => _model.CanInstallApp);
            UpdateAppCommand = new AsyncCommand(UpdateApp, () => _model.CanInstallApp && IsCanUpdate);
            if (_model.IsInstalled)
            {
                Task.Run(model.IsLastVersion)
                    .ContinueWith((t) =>
                    {
                        IsCanUpdate = !t.Result.isLast;
                        LastVersion = t.Result.lastVersion;
                        this.RaisePropertiesChanged();
                    }, TaskContinuationOptions.OnlyOnRanToCompletion);
            }

        }

        private async Task UpdateApp()
        {
            try
            {
                IsCanUpdate = false;
                await _model.InstallApplication();
                
            }
            catch (Exception)
            {
                IsCanUpdate = true;
            }

        }


        public static ApplicationViewModel Create(ApplicationModel model)
        {
            return ViewModelSource.Create(() => new ApplicationViewModel(model));
        }

    }
}