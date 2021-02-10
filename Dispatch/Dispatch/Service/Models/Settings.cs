using System.Windows;

namespace Dispatch.Service.Models
{
    public enum AppTheme { Auto, Light, Dark }

    public class Settings
    {
        public AppTheme Theme { get; set; }

        public Point? WindowPosition { get; set; }

        public Size? WindowSize { get; set; }

        public WindowState WindowState { get; set; }
    }
}
