using OwenLauncher.Applications;

namespace OwenLauncher
{
    public interface IInstalledApplicationsService
    {
        void UpdateInstallStatus(ApplicationModel model);
    }
}