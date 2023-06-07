using System.Windows.Input;

using Ronix.Framework.Mvvm;

namespace Ronix.Framework.WpfToolkit.Windows
{
    /// <summary>
    /// Base class for data context of a <see cref="DialogWindow"/>.
    /// </summary>
    public class DialogVm : ViewModelBase
    {
        private bool? _dialogResult;

        /// <summary>
        /// Gets or sets the value of the DialogResult property.
        /// </summary>
        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { SetValue(() => DialogResult, ref _dialogResult, value); }
        }

        public DialogVm()
        {
            Ok = new Command(OkCommand);
            Cancel = new Command(CancelCommand);
        }

        /// <summary>
        /// By default, closes the window and returns <c>true</c> from ShowDialog().
        /// </summary>
        public ICommand Ok { get; private set; }

        /// <summary>
        /// By default, closes the window and returns <c>false</c> from ShowDialog().
        /// </summary>
        public ICommand Cancel { get; private set; }

        /// <summary>
        /// Execution logic for <see cref="Ok"/> command.
        /// </summary>
        protected virtual void OkCommand()
        {
            DialogResult = true;
        }

        /// <summary>
        /// Execution logic for <see cref="Cancel"/> command.
        /// </summary>
        protected virtual void CancelCommand()
        {
            DialogResult = false;
        }
    }
}
