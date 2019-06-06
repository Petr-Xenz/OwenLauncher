using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.Win32;

namespace OwenLauncher
{
    //Computer\HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\OWEN Logic_is1
    //Computer\HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{0C03E123-6035-4EEE-8F65-20BF0FA76B7D}_is1
    public class RegistryApplicationSearcher : IApplicationSearcher
    {

        private const string ConfigurerRegistryPath =
            @"{0C03E123-6035-4EEE-8F65-20BF0FA76B7D}_is1";

        private const string OlRegistryPath =
            @"OWEN Logic_is1";

        private const string OpcRegistryPath =
            @"{87B1AABA-D679-40F4-AD6E-1B1451F41F7F}_is1";

        private const string ExecutablePath = "DisplayIcon";
        private const string NamePath = "DisplayName";
        private const string VersionPath = "DisplayVersion";
        private const string UninstallPath = "QuietUninstallString";

        public IEnumerable<ApplicationModel> FindApplications()
        {
            var applicationsRegistryPath = new List<string>
            {
                ConfigurerRegistryPath,
                OlRegistryPath,
                OpcRegistryPath
            };
            var reg = GetApplicationInstallPath(ConfigurerRegistryPath);
            return applicationsRegistryPath
                .Select(GetApplicationInstallPath)
                .Select(CreateModelFromRegistryData)
                .Where(m => m != null);
        }

        public static Dictionary<string, string> GetApplicationInstallPath(string nameOfAppToFind)
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

        private ApplicationModel CreateModelFromRegistryData(Dictionary<string, string> regData)
        {
            if (!regData.Any())
                return null;

            return new ApplicationModel(regData[NamePath], regData[VersionPath], regData[ExecutablePath], regData[ExecutablePath]);
        }
    }
}
