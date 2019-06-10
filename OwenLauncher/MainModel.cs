using System.Collections;
using System.Collections.Generic;
using OwenLauncher.Applications;

namespace OwenLauncher
{
    public class MainModel
    {
        public MainModel(IEnumerable<ApplicationModel> applications)
        {
            Applications = applications;
        }

        public IEnumerable<ApplicationModel> Applications { get; }
    }
}