using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using OwenLauncher.Applications;

namespace OwenLauncher
{
    [POCOViewModel]
    public class MainWindowViewModel
    {
        private readonly MainModel _model;
        public ObservableCollection<ApplicationViewModel> LocatedApplications { get; }

        protected MainWindowViewModel(MainModel model)
        {
            _model = model;
            LocatedApplications = new ObservableCollection<ApplicationViewModel>(model.Applications.Select(ApplicationViewModel.Create));
        }

        public Task CheckAppsForUpdate()
        {
            var tasks = LocatedApplications.Where(a => a.IsInstalled).Select(a => a.CheckForUpdate());
            return Task.WhenAll(tasks);
        }

        public static MainWindowViewModel Create(MainModel model)
        {
            return ViewModelSource.Create(() => new MainWindowViewModel(model));
        }
    }
}