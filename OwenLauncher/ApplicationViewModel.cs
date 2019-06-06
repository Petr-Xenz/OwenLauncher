using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;

namespace OwenLauncher
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
            Image = IconUtilities.IconFromFilePath(model.ExecutablePath).ToImageSource();
            LaunchApp = new DelegateCommand(_model.StartApp);
            DeleteApp = new DelegateCommand(_model.DeleteApp);
        }

        public static ApplicationViewModel Create(ApplicationModel model)
        {
            return ViewModelSource.Create(() => new ApplicationViewModel(model));
        }
        
    }
}