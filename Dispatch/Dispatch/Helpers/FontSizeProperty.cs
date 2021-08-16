using System.Windows;
using System.Windows.Controls;

namespace Dispatch.Helpers
{
    public static class FontSizeProperty
    {
        public static double GetBaseFontSize(Control control)
        {
            return (double)control.GetValue(BaseFontSizeProperty);
        }

        public static void SetBaseFontSize(Control control, double value)
        {
            control.SetValue(BaseFontSizeProperty, value);
        }

        public static readonly DependencyProperty BaseFontSizeProperty = DependencyProperty.RegisterAttached(
            "BaseFontSize",
            typeof(double),
            typeof(FontSizeProperty),
            new UIPropertyMetadata(5.0, OnFontSizeChanged));

        static void OnFontSizeChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            Control control = depObj as Control;
            if (control == null)
                return;

            if (e.NewValue is double == false)
                return;

            var baseFontSize = (double)e.NewValue;
            control.FontSize = FontSizeProvider.Instance.GetValue() * baseFontSize;

            var target = new MarkupExtensionTarget(control, Control.FontSizeProperty);
            var fontSizeExtension = new FontSizeExtension();
            fontSizeExtension.Value = baseFontSize;
            fontSizeExtension.ProvideValue(target);
        }
    }
}
