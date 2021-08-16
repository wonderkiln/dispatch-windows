using System;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;

namespace Dispatch.Helpers
{
    [MarkupExtensionReturnType(typeof(double))]
    public class FontSizeExtension : DynamicResourceExtension, FontSizeListener
    {
        public double Value { get; set; }

        private FrameworkElement targetObject;
        private PropertyInfo targetProperty;

        public FontSizeExtension(): base("foo")
        {
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            try
            {
                var providerValueTarget = (IProvideValueTarget) serviceProvider.GetService(typeof(IProvideValueTarget));

                targetObject = (FrameworkElement)providerValueTarget.TargetObject;
                targetObject.Unloaded += targetObject_Unloaded;
                var dependencyProperty = (DependencyProperty)providerValueTarget.TargetProperty;
                targetProperty = targetObject.GetType().GetProperty(dependencyProperty.Name);

                FontSizeProvider.Instance.AddListener(this);
                return Value * FontSizeProvider.Instance.GetValue();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Value;
            }
        }

        void targetObject_Unloaded(object sender, RoutedEventArgs e)
        {
            targetObject.Unloaded -= targetObject_Unloaded;
            FontSizeProvider.Instance.RemoveListener(this);
        }

        public void InformFontSizeChanged()
        {
            var newValue = Value * FontSizeProvider.Instance.GetValue();
            targetProperty.SetValue(targetObject, newValue, null);
        }
    }
}
