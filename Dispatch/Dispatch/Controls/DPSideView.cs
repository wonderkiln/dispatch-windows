using Dispatch.Helpers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Dispatch.Controls
{
    public class DPSideView : ContentControl
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(DPSideView), new PropertyMetadata("Panel"));
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(DPSideView), new PropertyMetadata(false));
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        public static readonly DependencyProperty PanelWidthProperty = DependencyProperty.Register("PanelWidth", typeof(double), typeof(DPSideView), new PropertyMetadata(300.0));
        public double PanelWidth
        {
            get { return (double)GetValue(PanelWidthProperty); }
            set { SetValue(PanelWidthProperty, value); }
        }

        public static readonly DependencyProperty PanelContentProperty = DependencyProperty.Register("PanelContent", typeof(object), typeof(DPSideView));
        public object PanelContent
        {
            get { return GetValue(PanelContentProperty); }
            set { SetValue(PanelContentProperty, value); }
        }

        public ICommand CloseCommand { get; }

        public DPSideView()
        {
            CloseCommand = new RelayCommand(Close);
            DefaultStyleKey = typeof(DPSideView);
        }

        private void Close()
        {
            IsOpen = false;
        }
    }
}
