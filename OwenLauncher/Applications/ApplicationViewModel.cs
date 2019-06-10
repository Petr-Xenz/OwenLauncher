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

        public ICommand LaunchApp { get; }
        public ICommand DeleteApp { get; }

        protected ApplicationViewModel(ApplicationModel model)
        {
            _model = model;
            Image = model.Icon.ToImageSource();
            LaunchApp = new DelegateCommand(_model.StartApp, () => _model.IsInstalled);
            DeleteApp = new DelegateCommand(_model.DeleteApp, () => _model.IsInstalled);
        }

        public static ApplicationViewModel Create(ApplicationModel model)
        {
            return ViewModelSource.Create(() => new ApplicationViewModel(model));
        }
        
    }
}