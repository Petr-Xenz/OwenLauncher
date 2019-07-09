using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using OwenLauncher.Applications;

namespace OwenLauncher
{
    public class RegistryInstalledApplicationsService : IInstalledApplicationsService
    {

        private const string ExecutablePath = "InstallLocation";
        private const string NamePath = "DisplayName";
        private const string VersionPath = "DisplayVersion";
        private const string UninstallPath = "UninstallString";

        public void UpdateInstallStatus(ApplicationModel model)
        {
            try
            {
                var installStatus = GetApplicationInstallPath(model.InstallData.InstallId);
                if (!installStatus.Any())
                {
                    model.UpdateInstallInfo("", "", "");
                }
                else
                {
                    var installPath = Path.Combine(installStatus[ExecutablePath], model.InstallData.MainExe);
                    model.UpdateInstallInfo(installPath, installStatus[UninstallPath], installStatus[VersionPath]);
                }
            }
            catch (Exception)
            {
                model.UpdateInstallInfo("", "", "");
            }
        }

        private static Dictionary<string, string> GetApplicationInstallPath(string nameOfAppToFind)
        {
            var attributes = new List<string>
            {
                NamePath, VersionPath, ExecutablePath, UninstallPath
            };

            // search in: LocalMachine_32
            var keyName = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            var installedPath = ExistsInSubKey(Registry.LocalMachine, keyName, nameOfAppToFind, attributes);
            if (installedPath.Any())
            {
                return installedPath;
            }

            // search in: LocalMachine_64
            keyName = @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall";
            installedPath = ExistsInSubKey(Registry.LocalMachine, keyName, nameOfAppToFind, attributes);
            if (installedPath.Any())
            {
                return installedPath;
            }

            // search in: CurrentUser
            keyName = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            installedPath = ExistsInSubKey(Registry.CurrentUser, keyName, nameOfAppToFind, attributes);
            if (installedPath.Any())
            {
                return installedPath;
            }


            return new Dictionary<string, string>();
        }

        private static Dictionary<string, string> ExistsInSubKey(RegistryKey root, string subKeyName, string idOfAppToFind, IEnumerable<string> attributes)
        {
            var result = new Dictionary<string, string>();

            using (var key = root.OpenSubKey(subKeyName))
            {
                if (key is null)
                    return result;
                var appsList = key.GetSubKeyNames();
                foreach (var kn in appsList)
                {
                    if (kn != idOfAppToFind)
                        continue;
                    using (var subkey = key.OpenSubKey(kn))
                    {
                        if (subkey is null)
                            return result;

                        foreach (var attribute in attributes)
                        {
                            result[attribute] = subkey.GetValue(attribute, string.Empty).ToString();
                        }
                    }
                }
            }
            return result;
        }
    }
}
