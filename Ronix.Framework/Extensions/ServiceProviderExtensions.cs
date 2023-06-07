using System;

namespace Ronix.Framework.Extensions
{
    /// <summary>
    /// Extensions for <see cref="IServiceProvider"/> interface.
    /// </summary>
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="this">The service provider.</param>
        /// <returns>The service object.</returns>
        public static T GetService<T>(this IServiceProvider @this)
        {
            return (T)@this.GetService(typeof(T));
        }
    }
}
