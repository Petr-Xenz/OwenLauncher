using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OwenLauncher.Applications
{
    public interface IInstallApplicationService
    {
        Task<bool> InstallApplication(string installerUrl, string arguments = "");
    }
}
