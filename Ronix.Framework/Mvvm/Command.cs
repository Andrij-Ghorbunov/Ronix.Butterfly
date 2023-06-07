using System;
using System.Diagnostics;

namespace Ronix.Framework.Mvvm
{
    /// <summary>
    /// Base class for <see cref="Command"/> and <see cref="Command{T}"/> classes.
    /// </summary>
    public abstract class CommandBase
    {
        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Raises the CanExecuteChanged event.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Implements a command.
    /// </summary>
    public class Command : CommandBase, IDelegateCommand
    {
        protected readonly Action ExecuteDelegate;

        protected readonly Func<bool> CanExecuteDelegate;

        /// <summary>
        /// Creates a new instance of command.
        /// </summary>
        /// <param name="execute">Execution delegate.</param>
        /// <param name="canExecute">Determines whether the command can be executed at the moment.</param>
        public Command(Action execute, Func<bool> canExecute = null)
        {
            ExecuteDelegate = execute;
            CanExecuteDelegate = canExecute ?? (() => true);
        }

        /// <summary>
        /// Invokes the command.
        /// </summary>
        /// <param name="parameter">Not used.</param>
        public void Execute(object parameter)
        {
            ExecuteDelegate();
        }

        /// <summary>
        /// Detects whether the command can be executed at the moment.
        /// </summary>
        /// <param name="parameter">Not used.</param>
        /// <returns><c>True</c> if the command can be executed; otherwise, <c>false</c>.</returns>
        public bool CanExecute(object parameter)
        {
            return CanExecuteDelegate();
        }
    }

    /// <summary>
    /// Implements a command with a typed parameter.
    /// </summary>
    /// <typeparam name="T">Type of the command parameter.</typeparam>
    public class Command<T> : CommandBase, IDelegateCommand
    {
        protected readonly Action<T> ExecuteDelegate;

        protected readonly Func<T, bool> CanExecuteDelegate;

        /// <summary>
        /// Creates a new instance of command.
        /// </summary>
        /// <param name="execute">Execution delegate.</param>
        /// <param name="canExecute">Determines whether the command can be executed with the given parameter.</param>
        public Command(Action<T> execute, Func<T, bool> canExecute = null)
        {
            ExecuteDelegate = execute;
            CanExecuteDelegate = canExecute ?? (it => true);
        }

        /// <summary>
        /// Invokes the command.
        /// </summary>
        /// <param name="parameter">The command parameter (should be of type <see cref="T"/>).</param>
        public void Execute(object parameter)
        {
            T param;
            if (!DefaultConverter.TryConvert(parameter, out param))
            {
                Debug.WriteLine(
                    "DefaultConverter failed to convert {0} of type {1} to type {2}",
                    parameter,
                    parameter.GetType(),
                    typeof(T));
                return;
            }
            ExecuteDelegate(param);
        }

        /// <summary>
        /// Detects whether the command can be executed with the given parameter.
        /// </summary>
        /// <param name="parameter">The command parameter (should be of type <see cref="T"/>).</param>
        /// <returns><c>True</c> if the command can be executed; otherwise, <c>false</c>.</returns>
        public bool CanExecute(object parameter)
        {
            T param;
            if (!DefaultConverter.TryConvert(parameter, out param))
            {
                Debug.WriteLine(
                    "DefaultConverter failed to convert {0} of type {1} to type {2}",
                    parameter,
                    parameter.GetType(),
                    typeof(T));
                return false;
            }
            return CanExecuteDelegate(param);
        }
    }
}
