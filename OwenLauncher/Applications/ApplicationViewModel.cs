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
        private bool _needUpdate;

        public ImageSource Image { get; }
        public string Version => _model.Version;
        public string Name => _model.Name;

        public ICommand LaunchApp { get; }
        public ICommand DeleteApp { get; }
        public ICommand InstallApp { get; }

        public ICommand UpdateApp { get; }

        public bool IsInstalled => _model.IsInstalled;

        public bool NeedUpdate
        {
            get => _needUpdate; 
            private set
            {
                _needUpdate = value;
                this.RaisePropertiesChanged();
            }
        }

        public Version ServerVersion => _model.ServerVersion;

        protected ApplicationViewModel(ApplicationModel model)
        {
            _model = model;
            _model.PropertyChanged += (s, e) => this.RaisePropertiesChanged();
            Image = model.Icon.ToImageSource();
            LaunchApp = new DelegateCommand(_model.StartApp, () => _model.IsInstalled);
            DeleteApp = new DelegateCommand(_model.DeleteApp, () => _model.IsInstalled);
            InstallApp = new AsyncCommand(_model.InstallApplication, () => _model.CanInstallApp);
            UpdateApp = new AsyncCommand(UpdateApplication, () => NeedUpdate);
        }

        internal async Task CheckForUpdate()
        {
            NeedUpdate = await _model.CheckForUpdate();
        }

        private async Task UpdateApplication()
        {
            try
            {
                await _model.UpdateApplication();
                NeedUpdate = false;
            }
            catch (Exception)
            {
                //Error handling
            }
        }

        public static ApplicationViewModel Create(ApplicationModel model)
        {
            return ViewModelSource.Create(() => new ApplicationViewModel(model));
        }

    }
}