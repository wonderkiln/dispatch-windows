using Dispatch.Controls;
using Dispatch.Helpers;
using Dispatch.Service.Models;
using System;
using System.IO;
using System.Linq;
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

        private FileIconTheme.Package[] iconThemes;
        public FileIconTheme.Package[] IconThemes
        {
            get
            {
                return iconThemes;
            }
            private set
            {
                iconThemes = value;
                Notify();
            }
        }

        private FileIconTheme.Package iconTheme;
        public FileIconTheme.Package IconTheme
        {
            get
            {
                return iconTheme;
            }
            set
            {
                iconTheme = value;
                Notify();

                try
                {
                    FileIconTheme.LoadTheme(value.Path);

                    var settings = WindowHelper.SettingsStorage.Load(new Settings());
                    settings.IconThemePath = value.Path;
                    WindowHelper.SettingsStorage.Save(settings);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public SettingsViewModel()
        {
            var settings = WindowHelper.SettingsStorage.Load(new Settings());
            theme = settings.Theme;

            var dir = Path.Combine(Directory.GetCurrentDirectory(), "Themes");
            var files = Directory.GetFiles(dir, "*.zip");
            var themes = files.Select(FileIconTheme.LoadThemeMetadata).ToArray();
            IconThemes = themes;
            iconTheme = themes.FirstOrDefault(e => e.Path == settings.IconThemePath);
        }
    }
}
