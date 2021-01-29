using Dispatch.Helpers;
using Dispatch.ViewModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Dispatch.View.Windows
{
    public class UpdateStatusDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NoUpdate { get; set; }
        public DataTemplate NewUpdate { get; set; }
        public DataTemplate DownloadingUpdate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (!(item is UpdateViewModel.StatusType)) return null;

            var status = (UpdateViewModel.StatusType)item;

            switch (status)
            {
                case UpdateViewModel.StatusType.None:
                    return NoUpdate;

                case UpdateViewModel.StatusType.UpdateAvailable:
                    return NewUpdate;

                case UpdateViewModel.StatusType.Downloading:
                    return DownloadingUpdate;

                default:
                    return null;
            }
        }
    }

    public partial class MainWindow : Window
    {
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowHelper.LoadWindowSettings(this);
            WindowHelper.EnableBlurForWindow(this);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            WindowHelper.SaveWindowSettings(this);
        }
    }
}
