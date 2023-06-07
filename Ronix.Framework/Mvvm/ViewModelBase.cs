using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Ronix.Framework.Mvvm
{
    /// <summary>
    /// Base class for all view models in the application.
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Raises PropertyChanged event.
        /// </summary>
        /// <param name="property">Path to the property.</param>
        protected void RaisePropertyChanged<T>(Expression<Func<T>> property)
        {
            RaisePropertyChanged(GetMemberName(property));
        }

        /// <summary>
        /// Sets the value of a property and raises PropertyChanged event.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="property">Expression pointing the property (like "it =&gt; it.PropertyName").</param>
        /// <param name="variable">Reference to the backing variable.</param>
        /// <param name="value">New value of the property.</param>
        /// <param name="callback">Action to invoke if the property has changed (old and new values are passed into the delegate).</param>
        protected void SetValue<T>(Expression<Func<T>> property, ref T variable, T value, Action<T, T> callback = null)
        {
            if ((IsNull(variable) && IsNull(value)) || (!IsNull(variable) && variable.Equals(value))) return;
            if (callback != null)
            {
                var temp = variable;
                variable = value;
                callback(temp, value);
            }
            else
            {
                variable = value;
            }
            RaisePropertyChanged(GetMemberName(property));
        }

        /// <summary>
        /// Sets the value of a property and raises PropertyChanged event.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="variable">Reference to the backing variable.</param>
        /// <param name="value">New value of the property.</param>
        /// <param name="callback">Action to invoke if the property has changed (old and new values are passed into the delegate).</param>
        /// <param name="propName">Property name (auto-set).</param>
        protected void SetValue<T>(ref T variable, T value, Action<T, T> callback = null, [CallerMemberName]string propName = "")
        {
            SetValueByName(propName, ref variable, value, callback);
        }

        /// <summary>
        /// Sets the value of a property and raises PropertyChanged event.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="property">Expression pointing the property (like "it =&gt; it.PropertyName").</param>
        /// <param name="variable">Reference to the backing variable.</param>
        /// <param name="value">New value of the property.</param>
        /// <param name="callback">Action to invoke if the property has changed (new value is passed into the delegate).</param>
        protected void SetValue<T>(Expression<Func<T>> property, ref T variable, T value, Action<T> callback)
        {
            if (callback == null)
            {
                SetValue(property, ref variable, value);
                return;
            }
            SetValue(property, ref variable, value, (oldValue, newValue) => callback(newValue));
        }

        /// <summary>
        /// Sets the value of a property and raises PropertyChanged event.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="variable">Reference to the backing variable.</param>
        /// <param name="value">New value of the property.</param>
        /// <param name="callback">Action to invoke if the property has changed (new value is passed into the delegate).</param>
        /// <param name="propName">Property name (auto-set).</param>
        protected void SetValue<T>(ref T variable, T value, Action<T> callback, [CallerMemberName]string propName = "")
        {
            SetValueByName(propName, ref variable, value, callback);
        }

        /// <summary>
        /// Sets the value of a property and raises PropertyChanged event.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="property">Expression pointing the property (like "it =&gt; it.PropertyName").</param>
        /// <param name="variable">Reference to the backing variable.</param>
        /// <param name="value">New value of the property.</param>
        /// <param name="callback">Action to invoke if the property has changed.</param>
        protected void SetValue<T>(Expression<Func<T>> property, ref T variable, T value, Action callback)
        {
            if (callback == null)
            {
                SetValue(property, ref variable, value);
                return;
            }
            SetValue(property, ref variable, value, (oldValue, newValue) => callback());
        }

        /// <summary>
        /// Sets the value of a property and raises PropertyChanged event.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="variable">Reference to the backing variable.</param>
        /// <param name="value">New value of the property.</param>
        /// <param name="callback">Action to invoke if the property has changed.</param>
        /// <param name="propName">Property name (auto-set).</param>
        protected void SetValue<T>(ref T variable, T value, Action callback, [CallerMemberName]string propName = "")
        {
            SetValueByName(propName, ref variable, value, callback);
        }

        /// <summary>
        /// Sets the value of a property and raises PropertyChanged event.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="variable">Reference to the backing variable.</param>
        /// <param name="value">New value of the property.</param>
        /// <param name="callback">Action to invoke if the property has changed (old and new values are passed into the delegate).</param>
        protected void SetValueByName<T>(string propertyName, ref T variable, T value, Action<T, T> callback = null)
        {
            if ((IsNull(variable) && IsNull(value)) || (!IsNull(variable) && variable.Equals(value))) return;
            if (callback != null)
            {
                var temp = variable;
                variable = value;
                callback(temp, value);
            }
            else
            {
                variable = value;
            }
            RaisePropertyChanged(propertyName);
        }

        /// <summary>
        /// Sets the value of a property and raises PropertyChanged event.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="variable">Reference to the backing variable.</param>
        /// <param name="value">New value of the property.</param>
        /// <param name="callback">Action to invoke if the property has changed (new value is passed into the delegate).</param>
        protected void SetValueByName<T>(string propertyName, ref T variable, T value, Action<T> callback)
        {
            if (callback == null)
            {
                SetValueByName(propertyName, ref variable, value);
                return;
            }
            SetValueByName(propertyName, ref variable, value, (oldValue, newValue) => callback(newValue));
        }

        /// <summary>
        /// Sets the value of a property and raises PropertyChanged event.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="variable">Reference to the backing variable.</param>
        /// <param name="value">New value of the property.</param>
        /// <param name="callback">Action to invoke if the property has changed.</param>
        protected void SetValueByName<T>(string propertyName, ref T variable, T value, Action callback)
        {
            if (callback == null)
            {
                SetValueByName(propertyName, ref variable, value);
                return;
            }
            SetValueByName(propertyName, ref variable, value, (oldValue, newValue) => callback());
        }

        /// <summary>
        /// Converts an expression (e. g. it => it.PropertyName) into member name (e. g. "PropertyName").
        /// </summary>
        /// <typeparam name="T">Type of the member.</typeparam>
        /// <param name="expression">Member access expression.</param>
        /// <returns>Member name.</returns>
        public static string GetMemberName<T>(Expression<Func<T>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
            {
                var unaryExpression = expression.Body as UnaryExpression;
                if (unaryExpression != null)
                    memberExpression = (MemberExpression)unaryExpression.Operand;
            }

            return memberExpression == null ? string.Empty : memberExpression.Member.Name;
        }

        /// <summary>
        /// Used to simplify cast from T to object.
        /// </summary>
        /// <param name="obj">The object being checked.</param>
        private static bool IsNull(object obj)
        {
            return obj == null;
        }
    }
}
