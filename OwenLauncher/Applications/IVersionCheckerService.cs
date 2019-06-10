using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OwenLauncher.Applications
{
    public interface IVersionCheckerService
    {
        Task<(bool isLast, string lastVersion)> IsLastVersion(Version currentVersion);
    }
}
