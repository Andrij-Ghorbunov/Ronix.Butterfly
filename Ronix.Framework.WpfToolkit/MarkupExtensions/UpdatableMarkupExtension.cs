using System;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;

using Ronix.Framework.Extensions;

namespace Ronix.Framework.WpfToolkit.MarkupExtensions
{
    public abstract class UpdatableMarkupExtension : MarkupExtension, IDisposable
    {
        /// <summary>
        /// Action to set the property value.
        /// </summary>
        protected Action<object> Set { get; private set; }

        /// <summary>
        /// Returns an object that is provided as the value of the target property for this markup extension. 
        /// </summary>
        /// <returns>
        /// The object value to set on the property where the extension is applied. 
        /// </returns>
        /// <param name="serviceProvider">A service provider helper that can provide services for the markup extension.</param>
        public sealed override object ProvideValue(IServiceProvider serviceProvider)
        {
            var target = serviceProvider.GetService<IProvideValueTarget>();
            if (target == null)
                return ProvideValueInternal(serviceProvider);

            var targetObject = target.TargetObject;
            var targetProperty = target.TargetProperty;
            var dependencyObject = targetObject as DependencyObject;

            var attached = targetProperty as MethodInfo;
            if (attached != null && attached.Name.StartsWith("Get") && attached.DeclaringType != null)
            {
                var setter = attached.DeclaringType.GetMethod("Set" + attached.Name.Substring(3));
                if (setter != null)
                    Set = val => setter.Invoke(null, new[] { dependencyObject, val });
                else
                    throw new Exception("Property has no setter: " + attached.Name.Substring(3));
            }
            else
            {
                var dependProp = targetProperty as DependencyProperty;
                var propInfo = targetProperty as PropertyInfo;
                if (dependProp != null && dependencyObject != null)
                    Set = val => SetDependencyProperty(dependencyObject, dependProp, val);
                else if (propInfo != null)
                    Set = val => propInfo.SetValue(targetObject, val);
            }

            return ProvideValueInternal(serviceProvider);
        }
        
        private static void SetDependencyProperty(DependencyObject dependencyObject, DependencyProperty dependProp, object val)
        {
            if (dependencyObject.Dispatcher.CheckAccess())
                dependencyObject.SetValue(dependProp, val);
            else
                dependencyObject.Dispatcher.Invoke(() => dependencyObject.SetValue(dependProp, val));
        }

        /// <summary>
        /// Updates the property with te new value.
        /// </summary>
        /// <param name="value"></param>
        protected void UpdateValue(object value)
        {
            if (Set == null)
            {
                Dispose();
                return;
            }
            Set(value);
        }

        /// <summary>
        /// When implemented in a derived class, returns actual value to set to the property.
        /// </summary>
        /// <returns>
        /// The object value to set on the property where the extension is applied. 
        /// </returns>
        /// <param name="serviceProvider">A service provider helper that can provide services for the markup extension.</param>
        protected abstract object ProvideValueInternal(IServiceProvider serviceProvider);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public abstract void Dispose();
    }
}