using System;
using System.Collections.Generic;

namespace Ronix.Framework.WpfToolkit.MarkupExtensions
{
    public abstract class LocalizationBase : UpdatableMarkupExtension
    {
        private static readonly List<LocalizationBase> UpdateList = new List<LocalizationBase>();

        protected LocalizationBase()
        {
            UpdateList.Add(this);
        }

        /// <summary>
        /// Refreshes values of all localization instances in the application (call after changing UI culture).
        /// </summary>
        public static void UpdateAll()
        {
            foreach (var localization in UpdateList)
            {
                localization.Update();
            }
        }

        private void Update()
        {
            UpdateValue(GetValue());
        }

        /// <summary>
        /// Returns actual value to set to the property.
        /// </summary>
        /// <returns>
        /// The object value to set on the property where the extension is applied. 
        /// </returns>
        /// <param name="serviceProvider">A service provider helper that can provide services for the markup extension.</param>
        protected override sealed object ProvideValueInternal(IServiceProvider serviceProvider)
        {
            return GetValue();
        }

        /// <summary>
        /// When implemented in a derived class, returns localization value read from resources.
        /// </summary>
        /// <returns>The localized string.</returns>
        protected abstract string GetValue();

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            UpdateList.Remove(this);
        }
    }
}
