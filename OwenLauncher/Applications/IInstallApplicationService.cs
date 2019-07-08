﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OwenLauncher.Applications
{
    public interface IInstallApplicationService
    {
        Task<bool> InstallApplicationAsync(string installerUrl, string arguments = "");
    }
}
