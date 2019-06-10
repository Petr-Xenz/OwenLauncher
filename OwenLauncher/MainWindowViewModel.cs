using System.Collections.ObjectModel;
using System.Linq;
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

        public static MainWindowViewModel Create(MainModel model)
        {
            return ViewModelSource.Create(() => new MainWindowViewModel(model));
        }
    }
}