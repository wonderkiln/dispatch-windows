using Dispatch.Helpers;
using Dispatch.Service.Models;
using Dispatch.ViewModels;
using System.ComponentModel;
using System.Windows;

namespace Dispatch
{
    public partial class LicenseWindow : Window
    {
        private LicenseViewModel ViewModel
        {
            get
            {
                return (LicenseViewModel)DataContext;
            }
        }

        public LicenseWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowHelper.EnableBlurForWindow(this);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (ViewModel.Status == DeviceStatus.LicenseStatus.None ||
                ViewModel.Status == DeviceStatus.LicenseStatus.TrialExpired ||
                ViewModel.Status == DeviceStatus.LicenseStatus.LicenseExpiredWrongVersion)
            {
                Application.Current.Shutdown();
            }
        }
    }
}
