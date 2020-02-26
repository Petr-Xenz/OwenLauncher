using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OwenLauncher.Applications
{

    internal class FtpDownloader
    {
        public async Task<byte[]> GetFileDataFromFtp(Uri fileUrl)
        {
            using (var client = new HttpClient())
            {
                var dataResponse = await client.GetAsync(fileUrl);
                if (!dataResponse.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException(dataResponse.ReasonPhrase);
                }

                return await dataResponse.Content.ReadAsByteArrayAsync();
            }
        }

        public async Task<string> GetFileStringFromFtp(Uri fileUrl)
        {
            using (var client = new HttpClient())
            {
                var dataResponse = await client.GetAsync(fileUrl);
                if (!dataResponse.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException(dataResponse.ReasonPhrase);
                }

                return await dataResponse.Content.ReadAsStringAsync();
            }
        }
    }

    internal class InstallFromFtp : IInstallApplicationService
    {
        private readonly FtpDownloader _downloader = new FtpDownloader();

        public async Task<bool> InstallApplicationAsync(string installerUrl, string arguments = "")
        {
            try
            {
                var installerData = await GetFromFtp(installerUrl);
                if (installerData.Length == 0)
                    return false;

                return await LaunchInstaller(installerData, arguments);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<byte[]> GetFromFtp(string installerUrl)
        {
            var baseUrl = new Uri(installerUrl);
            var updateConfig = await _downloader.GetFileStringFromFtp(new Uri(baseUrl, "config.cfg"));

            var matchFile = Regex.Match(updateConfig, @"File: ([^\s]+)");
            if (!matchFile.Success)
            {
                return new byte[0];
            }

            var requestUri = matchFile.Groups[1].ToString();
            return await _downloader.GetFileDataFromFtp(new Uri(baseUrl, requestUri));

        }

        private async Task<bool> LaunchInstaller(byte[] installerData, string arguments)
        {
            var tempFile = $@"{Path.GetTempPath()}{DateTime.Now.Ticks}{DateTime.Now.Millisecond}.exe";
            try
            {
                File.WriteAllBytes(tempFile, installerData);
                var p = new Process
                {
                    StartInfo = new ProcessStartInfo(tempFile, arguments),
                };

                var result = await Task.Run(() =>
                {
                    p.Start();
                    try
                    {
                        p.WaitForExit();
                        return p.ExitCode == 0;
                    }
                    finally
                    {
                        p.Dispose();
                    }
                    
                });

                return result;

            }
            finally
            {
                File.Delete(tempFile);
            }
        }
    }

    internal class UpdateFromFtp : IUpdateApplicationService
    {
        private readonly FtpDownloader _downloader = new FtpDownloader();

        public async Task<(bool canUpdate, Version newVersion)> CanUpdateApplication(string url, Version currentVersion)
        {
            var baseUrl = new Uri(url);
            var updateConfig = await _downloader.GetFileStringFromFtp(new Uri(baseUrl, "config.cfg"));

            var versionMatch = Regex.Match(updateConfig, @"Version: ([^\s]+)");
            if (!versionMatch.Success)
            {
                return (false, new Version());
            }

            var version = versionMatch.Groups[1].Value;
            var isVersionParsed = Version.TryParse(version, out var serverVersion);
            if (!isVersionParsed)
            {
                return (false, new Version());
            }

            if (currentVersion.Revision == -1 && serverVersion.Revision == 0)
            {
                currentVersion = new Version(currentVersion.Major, currentVersion.Minor, currentVersion.Build, 0);
            }

            return (currentVersion < serverVersion, serverVersion);
        }
    }
}
