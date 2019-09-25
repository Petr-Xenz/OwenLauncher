using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
using DevExpress.Mvvm.POCO;
using OwenLauncher.Applications;

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
            var installedService = new RegistryInstalledApplicationsService();
            var models = CreateModels(ReadEmbbededConfigurations()).ToList();
            
            models.ForEach(m => installedService.UpdateInstallStatus(m));

            var model = new MainModel(models);
            var viewModel = MainWindowViewModel.Create(model);
            MainWindow = new MainWindow
            {
                DataContext = viewModel
            };

            Task.Run(viewModel.CheckAppsForUpdate);

            MainWindow.Show();
        }


        private IEnumerable<ApplicationConfiguration> ReadEmbbededConfigurations()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourcesLocation = "OwenLauncher.Applications.Configurations";

            var resources = assembly.GetManifestResourceNames().Where(r => r.StartsWith(resourcesLocation));

            foreach (var r in resources)
            {
                using (var stream = assembly.GetManifestResourceStream(r))
                {
                    var reader = new XmlSerializer(typeof(ApplicationConfiguration));
                    yield return (ApplicationConfiguration)reader.Deserialize(stream);
                }
            }
        }

        private IEnumerable<ApplicationModel> CreateModels(IEnumerable<ApplicationConfiguration> configs)
        {
            return configs.Select(ApplicationModelFactory.Create);
        }
    }
}
