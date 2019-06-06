using System.Collections.Generic;

namespace OwenLauncher
{
    public interface IApplicationSearcher
    {
        IEnumerable<ApplicationModel> FindApplications();
    }
}