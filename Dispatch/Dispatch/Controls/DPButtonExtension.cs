using System.Windows;

namespace Dispatch.Controls
{
    public class DPButtonExtension
    {
        public static bool GetIsLoading(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsLoadingProperty);
        }

        public static void SetIsLoading(DependencyObject obj, bool value)
        {
            obj.SetValue(IsLoadingProperty, value);
        }

        public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.RegisterAttached("IsLoading", typeof(bool), typeof(DPButtonExtension), new PropertyMetadata(false));
    }
}
