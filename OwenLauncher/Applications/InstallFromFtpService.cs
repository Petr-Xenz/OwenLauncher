using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OwenLauncher.Applications
{
    internal class InstallFromFtpService : IInstallApplicationService
    {
        private readonly string _installerUrl;

        public InstallFromFtpService(string installerUrl)
        {
            _installerUrl = installerUrl ?? throw new ArgumentNullException(nameof(installerUrl));
        }

        public async Task<bool> InstallApplication(string arguments = "")
        {
            try
            {
                var installerData = await GetFromFtp(_installerUrl);
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
            var client = new HttpClient();
            client.BaseAddress = new Uri(installerUrl);
            var data = await client.GetAsync("config.cfg");
            if (!data.IsSuccessStatusCode)
            {
                return new byte[0];
            }

            var updateConfig = await data.Content.ReadAsStringAsync();

            var matchFile = Regex.Match(updateConfig, @"File: ([^\s]+)");
            if (!matchFile.Success)
            {
                return new byte[0];
            }

            var requestUri = matchFile.Groups[1].ToString();
            var installerResponse = await client.GetAsync(requestUri);

            if (installerResponse.IsSuccessStatusCode)
            {
                return await installerResponse.Content.ReadAsByteArrayAsync();
            }
            else
            {
                return new byte[0];
            }
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
}
