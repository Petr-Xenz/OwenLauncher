using System;
using System.Threading.Tasks;

namespace OwenLauncher.Applications
{
    public interface IUpdateApplicationService
    {
        Task<(bool canUpdate, Version newVersion)> CanUpdateApplication(string url, Version currentVersion);
    }

    internal class NoUpdateService : IUpdateApplicationService
    {
        public Task<(bool canUpdate, Version newVersion)> CanUpdateApplication(string url, Version currentVersion) => Task.FromResult((false, new Version()));
    }
}