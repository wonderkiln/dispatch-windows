using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Dispatch.Helpers
{
    public class WindowHelper
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
            WCA_ACCENT_POLICY = 19
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

        public static void EnableBlurForWindow(Window window)
        {
            var windowHelper = new WindowInteropHelper(window);

            var accent = new AccentPolicy
            {
                AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND,
                AccentFlags = 0x20 | 0x40 | 0x80 | 0x100
            };

            var accentStructSize = Marshal.SizeOf(accent);
            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new WindowCompositionAttributeData
            {
                Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY,
                SizeOfData = accentStructSize,
                Data = accentPtr
            };

            SetWindowCompositionAttribute(windowHelper.Handle, ref data);

            Marshal.FreeHGlobal(accentPtr);
        }

        public static void LoadWindowSettings(Window window)
        {
            window.Width = Properties.Settings.Default.WindowSize.Width;
            window.Height = Properties.Settings.Default.WindowSize.Height;

            var x = Properties.Settings.Default.WindowPosition.X;
            var y = Properties.Settings.Default.WindowPosition.Y;

            if (x == -1 && y == -1)
            {
                window.Left = (SystemParameters.WorkArea.Width - window.Width) / 2;
                window.Top = (SystemParameters.WorkArea.Height - window.Height) / 2;
            }
            else
            {
                window.Left = x;
                window.Top = y;
            }

            if (Properties.Settings.Default.WindowMaximized)
                window.WindowState = WindowState.Maximized;
            else
                window.WindowState = WindowState.Normal;
        }

        public static void SaveWindowSettings(Window window)
        {
            if (window.WindowState == WindowState.Maximized)
            {
                Properties.Settings.Default.WindowMaximized = true;
            }
            else
            {
                Properties.Settings.Default.WindowSize = new System.Drawing.Size((int)window.Width, (int)window.Height);
                Properties.Settings.Default.WindowPosition = new System.Drawing.Point((int)window.Left, (int)window.Top);
                Properties.Settings.Default.WindowMaximized = false;
            }

            Properties.Settings.Default.Save();
        }

        public static bool GetBlurBehind(DependencyObject obj)
        {
            return (bool)obj.GetValue(BlurBehindProperty);
        }

        public static void SetBlurBehind(DependencyObject obj, bool value)
        {
            obj.SetValue(BlurBehindProperty, value);
        }

        public static readonly DependencyProperty BlurBehindProperty = DependencyProperty.RegisterAttached("BlurBehind", typeof(bool), typeof(WindowHelper), new PropertyMetadata(false, new PropertyChangedCallback(OnBlurBehindChanged)));

        public static void OnBlurBehindChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue && d is Window window)
            {
                EnableBlurForWindow(window);
            }
        }

        public static RelayCommand<Window> MinimizeCommand { get; } = new RelayCommand<Window>((window) =>
        {
            window.WindowState = WindowState.Minimized;
        });

        public static RelayCommand<Window> RestoreCommand { get; } = new RelayCommand<Window>((window) =>
        {
            window.WindowState = WindowState.Normal;
        });

        public static RelayCommand<Window> MaximizeCommand { get; } = new RelayCommand<Window>((window) =>
        {
            window.WindowState = WindowState.Maximized;
        });

        public static RelayCommand<Window> CloseCommand { get; } = new RelayCommand<Window>((window) =>
        {
            window.Close();
        });
    }
}
