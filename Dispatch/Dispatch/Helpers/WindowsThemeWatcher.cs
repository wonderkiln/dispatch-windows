using Microsoft.Win32;
using System;
using System.Globalization;
using System.Management;
using System.Security.Principal;

namespace Dispatch.Helpers
{
    public class WindowsThemeWatcher
    {
        private const string RegistryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
        private const string RegistryValueName = "AppsUseLightTheme";

        public enum WindowsTheme { Light, Dark }

        private EventHandler<WindowsTheme> onChangeWindowsTheme;
        public event EventHandler<WindowsTheme> OnChangeWindowsTheme
        {
            add
            {
                onChangeWindowsTheme += value;
                value?.Invoke(this, Theme);
            }
            remove
            {
                onChangeWindowsTheme -= value;
            }
        }

        public WindowsTheme Theme { get; private set; }

        public WindowsThemeWatcher()
        {
            var currentUser = WindowsIdentity.GetCurrent();

            string query = string.Format(
                CultureInfo.InvariantCulture,
                @"SELECT * FROM RegistryValueChangeEvent WHERE Hive = 'HKEY_USERS' AND KeyPath = '{0}\\{1}' AND ValueName = '{2}'",
                currentUser.User.Value,
                RegistryKeyPath.Replace(@"\", @"\\"),
                RegistryValueName);

            try
            {
                var watcher = new ManagementEventWatcher(query);

                watcher.EventArrived += (sender, args) =>
                {
                    Theme = GetWindowsTheme();
                    onChangeWindowsTheme?.Invoke(this, Theme);
                };

                watcher.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Theme = GetWindowsTheme();
        }

        private WindowsTheme GetWindowsTheme()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath))
            {
                var registryValueObject = key?.GetValue(RegistryValueName);

                if (registryValueObject == null)
                {
                    return WindowsTheme.Light;
                }

                int registryValue = (int)registryValueObject;

                return registryValue > 0 ? WindowsTheme.Light : WindowsTheme.Dark;
            }
        }
    }
}
