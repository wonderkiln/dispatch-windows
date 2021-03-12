using Dispatch.Helpers;
using System.Windows;

namespace Dispatch
{
    public partial class ModalWindow : Window
    {
        public static readonly DependencyProperty MyPropertyProperty = DependencyProperty.Register("ModalContent", typeof(object), typeof(ModalWindow));
        public object ModalContent
        {
            get { return GetValue(MyPropertyProperty); }
            set { SetValue(MyPropertyProperty, value); }
        }

        public ModalWindow(string title, object content)
        {
            InitializeComponent();

            Title = title;
            ModalContent = content;

            Owner = Application.Current.MainWindow;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowHelper.LoadWindowSettings(this);
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
