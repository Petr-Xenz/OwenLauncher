using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OwenLauncher.Applications
{
    internal class InstallFromSite : IInstallApplicationService
    {
        public async Task<bool> InstallApplicationAsync(string installerUrl, string arguments = "")
        {
            var tempFolder = Path.GetTempPath();
            var tempName = DateTime.Now.Ticks.ToString();
            var extractFolder = Path.Combine(tempFolder, tempName);
            var extractFile = Path.Combine(tempFolder, $"{tempName}.zip");
            try
            {
                var client = new HttpClient();
                var data = await client.GetAsync(installerUrl);                
                if (!data.IsSuccessStatusCode)
                {
                    return false;
                }
                using (var file = new FileStream(extractFile, FileMode.OpenOrCreate))
                {
                    await data.Content.CopyToAsync(file);
                }
                ZipFile.ExtractToDirectory(extractFile, extractFolder);
                var installer = Directory.GetFiles(extractFolder, "*.exe", SearchOption.AllDirectories).FirstOrDefault();
                if (installer != null)
                {
                    using (var p = new Process { StartInfo = new ProcessStartInfo(installer, arguments) })
                    {
                        p.Start();
                        await Task.Run(() => p.WaitForExit());
                        return true;
                    };
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (File.Exists(extractFile))
                {
                    File.Delete(extractFile);
                }
                if (Directory.Exists(extractFolder))
                {
                    Directory.Delete(extractFolder, true);
                }
            }
        }
    }
}
