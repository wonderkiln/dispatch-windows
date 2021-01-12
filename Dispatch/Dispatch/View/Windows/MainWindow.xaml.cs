using Dispatch.Helpers;
using Dispatch.Service.Updater;
using Dispatch.ViewModel;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace Dispatch.View.Windows
{
    public class DWMHelper
    {
        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        [StructLayout(LayoutKind.Sequential)]
        internal struct WindowCompositionAttributeData
        {
            public WindowCompositionAttribute Attribute;
            public IntPtr Data;
            public int SizeOfData;
        }

        internal enum WindowCompositionAttribute
        {
            // ...
            WCA_ACCENT_POLICY = 19
            // ...
        }

        internal enum AccentState
        {
            ACCENT_DISABLED = 0,
            ACCENT_ENABLE_GRADIENT = 1,
            ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
            ACCENT_ENABLE_BLURBEHIND = 3,
            ACCENT_INVALID_STATE = 4
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct AccentPolicy
        {
            public AccentState AccentState;
            public int AccentFlags;
            public int GradientColor;
            public int AnimationId;
        }
    }

    public partial class MainWindow : Window
    {
        public TabsViewModel ViewModel { get; } = new TabsViewModel();

        public QueueViewModel QueueViewModel { get; } = new QueueViewModel();

        private void NewTabButton_Click(object sender, RoutedEventArgs e)
        {
            TabsListBox.SelectedItem = ViewModel.NewTab();
        }

        private void CloseTabButton_Click(object sender, RoutedEventArgs e)
        {
            var button = e.OriginalSource as Button;
            var model = button.DataContext as TabViewModel;
            model.Disconnect();
            ViewModel.CloseTab(model);
        }

        private void TabListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;

            if (e.AddedItems.Count == 0)
            {
                listBox.SelectedIndex = 0;
            }
            else
            {
                listBox.ScrollIntoView(e.AddedItems[0]);
            }
        }

        private async void UpdateMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var updater = new ApplicationUpdater(new UpdateProvider());
            await updater.CheckForUpdate();
        }

        private void TransfersMenuItem_Click(object sender, RoutedEventArgs e)
        {
            TransfersPopup.IsOpen = true;
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"You have version {Constants.VERSION} ({Constants.CHANNEL})", "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        internal void EnableBlur()
        {
            var windowHelper = new WindowInteropHelper(this);

            var accent = new DWMHelper.AccentPolicy();
            var accentStructSize = Marshal.SizeOf(accent);
            accent.AccentState = DWMHelper.AccentState.ACCENT_ENABLE_BLURBEHIND;
            accent.AccentFlags = 0x20 | 0x40 | 0x80 | 0x100;

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new DWMHelper.WindowCompositionAttributeData();
            data.Attribute = DWMHelper.WindowCompositionAttribute.WCA_ACCENT_POLICY;
            data.SizeOfData = accentStructSize;
            data.Data = accentPtr;

            DWMHelper.SetWindowCompositionAttribute(windowHelper.Handle, ref data);

            Marshal.FreeHGlobal(accentPtr);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            EnableBlur();
        }
    }
}
