using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OwenLauncher.Properties;

namespace OwenLauncher.Applications
{
    internal static class ApplicationModelFactory
    {
        public static ApplicationModel Create(ApplicationConfiguration configuration)
        {
            Bitmap image;
            try
            {
                image = (Bitmap)Resources.ResourceManager.GetObject(configuration.ImageResourceName) ?? Resources.Default;
            }
            catch (Exception)
            {
                image = Resources.Default;
            }

            var installService = GetInstallService(configuration.InstallServiceType);

            return new ApplicationModel(configuration.UserName, configuration.InstallId, image, configuration.UpdateUrl, configuration.HistoryUrl, installService);
        }

        private static IInstallApplicationService GetInstallService(string serviceName)
        {
            if (string.IsNullOrWhiteSpace(serviceName))
                return null;

            switch(serviceName.ToLower())
            {
                case "installfromftp": 
                    return new InstallFromFtp();
                default:
                    return null;
            }
        }
    }
}
