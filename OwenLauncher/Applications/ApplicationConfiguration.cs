﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OwenLauncher.Applications
{
    [Serializable]
    public class ApplicationConfiguration
    {
        public string UserName { get; set; }

        public InstallData InstallData { get; set; }

        public string ImageResourceName { get; set; }

        public string InstallUrl { get; set; }

        public string HistoryUrl { get; set; }

        public string InstallServiceType { get; set; }
        public string UpdateServiceType { get; set; }
    }
}
