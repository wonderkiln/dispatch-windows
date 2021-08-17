using System.Windows;

namespace Dispatch.Service.Models
{
    public enum AppTheme { Auto, Light, Dark }

    public enum AppFontSize { Normal, Large, Small }

    public class Settings
    {
        public AppTheme Theme { get; set; }

        public Point? WindowPosition { get; set; }

        public Size? WindowSize { get; set; }

        public WindowState WindowState { get; set; }

        public string IconThemePath { get; set; }

        public AppFontSize FontSize { get; set; }
    }
}
