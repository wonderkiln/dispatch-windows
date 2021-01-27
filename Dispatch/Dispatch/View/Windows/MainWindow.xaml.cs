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

        private void TransfersButton_Click(object sender, RoutedEventArgs e)
        {
            //if (SideView.IsOpen && SideView.PanelContent is QueueView)
            //{
            //    SideView.IsOpen = false;
            //    return;
            //}

            //SideView.Title = "Transfers";
            //SideView.PanelContent = new QueueView { ViewModel = QueueViewModel };
            //SideView.IsOpen = true;
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            //if (SideView.IsOpen && SideView.PanelContent is MoreView)
            //{
            //    SideView.IsOpen = false;
            //    return;
            //}

            //var view = new MoreView();
            //view.CloseSidebar = () => { SideView.IsOpen = false; };
            //view.ChangeSidebar = (title, newView) =>
            //{
            //    SideView.Title = title;
            //    SideView.PanelContent = newView;
            //};

            //SideView.Title = "More";
            //SideView.PanelContent = view;
            //SideView.IsOpen = true;
        }
    }
}
