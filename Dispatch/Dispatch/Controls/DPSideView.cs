using System.Windows;
using System.Windows.Controls;

namespace Dispatch.Controls
{
    [TemplatePart(Name = "PART_CloseButton", Type = typeof(Button))]
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

        public DPSideView()
        {
            DefaultStyleKey = typeof(DPSideView);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var button = (Button)GetTemplateChild("PART_CloseButton");
            button.Click += CloseButton_Click;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            IsOpen = false;
        }
    }
}
