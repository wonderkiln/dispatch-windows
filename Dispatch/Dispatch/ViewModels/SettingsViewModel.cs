using Dispatch.Helpers;
using Dispatch.Service.Models;
using System.Windows;

namespace Dispatch.ViewModels
{
    public class SettingsViewModel : Observable
    {
        public AppTheme[] Themes
        {
            get
            {
                return new AppTheme[] {
                    AppTheme.Auto,
                    AppTheme.Light,
                    AppTheme.Dark,
                };
            }
        }

        private AppTheme theme;
        public AppTheme Theme
        {
            get
            {
                return theme;
            }
            set
            {
                theme = value;
                Notify();

                ((App)Application.Current).SetTheme(value);
            }
        }

        public SettingsViewModel()
        {
            var settings = WindowHelper.SettingsStorage.Load(new Settings());
            theme = settings.Theme;
        }
    }
}
