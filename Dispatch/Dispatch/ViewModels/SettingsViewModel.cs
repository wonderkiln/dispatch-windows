using Dispatch.Helpers;
using Dispatch.Service.Models;
using Dispatch.Service.Theme;
using System;
using System.Collections.Generic;
using System.IO;
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

        private ThemePackage[] iconThemes;
        public ThemePackage[] IconThemes
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

        private ThemePackage iconTheme;
        public ThemePackage IconTheme
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

        public AppFontSize[] FontSizes
        {
            get
            {
                return new AppFontSize[]
                {
                    AppFontSize.Normal,
                    AppFontSize.Large,
                    AppFontSize.Small,
                };
            }
        }

        private AppFontSize fontSize;
        public AppFontSize FontSize
        {
            get
            {
                return fontSize;
            }
            set
            {
                fontSize = value;
                Notify();

                ((App)Application.Current).SetFontSize(value);
            }
        }

        public SettingsViewModel()
        {
            try
            {
                var settings = WindowHelper.SettingsStorage.Load(new Settings());
                theme = settings.Theme;
                fontSize = settings.FontSize;

                var dir = Path.Combine(Directory.GetCurrentDirectory(), "Themes");
                var files = Directory.GetFiles(dir, "*.zip");

                var themes = new List<ThemePackage>();

                foreach (var file in files)
                {
                    try
                    {
                        var theme = FileIconTheme.LoadThemeMetadata(file);
                        themes.Add(theme);

                        if (settings.IconThemePath == file)
                        {
                            IconTheme = theme;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                IconThemes = themes.ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
