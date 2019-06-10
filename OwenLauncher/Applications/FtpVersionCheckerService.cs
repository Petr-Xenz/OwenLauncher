using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OwenLauncher.Applications
{
    internal class FtpVersionCheckerService : IVersionCheckerService
    {
        private readonly string _appUrl;

        public FtpVersionCheckerService(string appUrl)
        {
            _appUrl = appUrl ?? throw new ArgumentNullException(nameof(appUrl));
        }

        public async Task<(bool isLast, string lastVersion)> IsLastVersion(Version currentVersion)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(_appUrl);
            var data = await client.GetAsync("config.cfg");
            if (!data.IsSuccessStatusCode)
            {
                return (true, currentVersion.ToString());
            }

            var updateConfig = await data.Content.ReadAsStringAsync();

            var matchFile = Regex.Match(updateConfig, @"Version: ([^\s]+)");
            if (!matchFile.Success)
            {
                return (true, currentVersion.ToString());
            }

            var isParsed = Version.TryParse(matchFile.Groups[1].ToString(), out var lastVersion);

            if (!isParsed)
            {
                return (true, currentVersion.ToString());
            }

            var isLast = lastVersion <= currentVersion;
            var lastVersionString = isLast? currentVersion.ToString() : lastVersion.ToString();
            return (isLast,  lastVersionString);
        }
    }
}
