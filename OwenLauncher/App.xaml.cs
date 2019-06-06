using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Mvvm.POCO;

namespace OwenLauncher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var appSearcher = new RegistryApplicationSearcher();
            var model = new MainModel(appSearcher.FindApplications());
            MainWindow = new MainWindow
            {
                DataContext = MainWindowViewModel.Create(model)
            };

            MainWindow.Show();
        }
    }
}
