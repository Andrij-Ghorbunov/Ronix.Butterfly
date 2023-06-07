using System;
using System.Windows.Input;

using Ronix.Framework.Mvvm;

namespace Ronix.Framework.Extensions
{
    /// <summary>
    /// Extensions for <see cref="ICommand"/>.
    /// </summary>
    public static class CommandExtensions
    {
        /// <summary>
        /// Raises the CanExecuteChanged event for <see cref="Command"/>.
        /// </summary>
        /// <param name="this">An <see cref="ICommand"/>. Should be of type <see cref="Command"/>.</param>
        public static void RaiseCanExecuteChanged(this ICommand @this)
        {
            var command = @this as IDelegateCommand;
            if (command != null)
                command.RaiseCanExecuteChanged();
            else
                throw @this == null
                          ? (Exception)new ArgumentNullException("this")
                          : new NotSupportedException("Third-party ICommand implementations not supported");
        }
    }
}
