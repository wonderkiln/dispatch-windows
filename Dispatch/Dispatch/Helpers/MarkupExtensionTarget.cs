using System;
using System.Windows;
using System.Windows.Markup;

namespace Dispatch.Helpers
{
    public struct MarkupExtensionTarget : IServiceProvider, IProvideValueTarget
    {
        private readonly DependencyObject targetObject;
        private readonly DependencyProperty targetProperty;

        public MarkupExtensionTarget(DependencyObject targetObject, DependencyProperty targetProperty)
        {
            this.targetObject = targetObject;
            this.targetProperty = targetProperty;
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IProvideValueTarget))
            {
                return this;
            }
            return null;
        }

        object IProvideValueTarget.TargetObject { get { return targetObject; } }
        object IProvideValueTarget.TargetProperty { get { return targetProperty; } }
    }
}
