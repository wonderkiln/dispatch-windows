using System.Windows;
using System.Windows.Controls;

namespace Dispatch.Controls
{
    [TemplatePart(Name = "PART_CloseButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_ScrollViewer", Type = typeof(ScrollViewer))]
    public class DPSideView : ContentControl
    {
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

        public static readonly DependencyProperty PanelContentProperty = DependencyProperty.Register("PanelContent", typeof(object), typeof(DPSideView), new PropertyMetadata(new PropertyChangedCallback(OnChangePanelContent)));
        public object PanelContent
        {
            get { return GetValue(PanelContentProperty); }
            set { SetValue(PanelContentProperty, value); }
        }

        public DPSideView()
        {
            DefaultStyleKey = typeof(DPSideView);
        }

        internal ScrollViewer PanelScrollViewer;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var closeButton = (Button)GetTemplateChild("PART_CloseButton");
            closeButton.Click += CloseButton_Click;

            PanelScrollViewer = (ScrollViewer)GetTemplateChild("PART_ScrollViewer");
            PanelScrollViewer.Content = PanelContent;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            IsOpen = false;
        }

        private static void OnChangePanelContent(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var item = (DPSideView)d;

            if (item.PanelScrollViewer != null)
            {
                item.PanelScrollViewer.Content = e.NewValue;
            }
        }
    }
}
