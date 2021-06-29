using System.Windows;
using System.Windows.Controls;

namespace Dispatch.Controls
{
    public class DPTextBox : TextBox
    {
        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register("Placeholder", typeof(string), typeof(DPTextBox));
        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }

        private static readonly DependencyPropertyKey IsPlaceholderShownPropertyKey = DependencyProperty.RegisterReadOnly("IsPlaceholderShown", typeof(bool), typeof(DPTextBox), new PropertyMetadata(true));
        public static readonly DependencyProperty IsPlaceholderShownProperty = IsPlaceholderShownPropertyKey.DependencyProperty;
        public bool IsPlaceholderShown
        {
            get { return (bool)GetValue(IsPlaceholderShownProperty); }
        }

        public static readonly DependencyProperty LabelSharedSizeGroupProperty = DependencyProperty.Register("LabelSharedSizeGroup", typeof(string), typeof(DPTextBox));
        public string LabelSharedSizeGroup
        {
            get { return (string)GetValue(LabelSharedSizeGroupProperty); }
            set { SetValue(LabelSharedSizeGroupProperty, value); }
        }

        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(DPTextBox), new PropertyMetadata(new PropertyChangedCallback(OnLabelChanged)));
        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        private static readonly DependencyPropertyKey IsLabelShownPropertyKey = DependencyProperty.RegisterReadOnly("IsLabelShown", typeof(bool), typeof(DPTextBox), new PropertyMetadata(false));
        public static readonly DependencyProperty IsLabelShownProperty = IsLabelShownPropertyKey.DependencyProperty;
        public bool IsLabelShown
        {
            get { return (bool)GetValue(IsLabelShownProperty); }
        }

        protected static void OnLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var label = (string)e.NewValue;
            d.SetValue(IsLabelShownPropertyKey, label.Length != 0);
        }

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(DPTextBox));
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty TrailingProperty = DependencyProperty.Register("Trailing", typeof(object), typeof(DPTextBox));
        public object Trailing
        {
            get { return GetValue(TrailingProperty); }
            set { SetValue(TrailingProperty, value); }
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            SetValue(IsPlaceholderShownPropertyKey, Text.Length == 0);
        }

        public DPTextBox()
        {
            DefaultStyleKey = typeof(DPTextBox);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var PART_ContentHost = GetTemplateChild("PART_ContentHost") as ScrollViewer;

            if (PART_ContentHost != null)
            {
                PART_ContentHost.Padding = new Thickness(0);
                PART_ContentHost.Margin = new Thickness(0);
            }
        }
    }
}
