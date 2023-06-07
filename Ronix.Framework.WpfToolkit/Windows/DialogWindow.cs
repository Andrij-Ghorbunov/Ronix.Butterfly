using System.Windows;
using System.Windows.Data;

namespace Ronix.Framework.WpfToolkit.Windows
{
    /// <summary>
    /// Window with bindable DialogResult.
    /// </summary>
    public class DialogWindow : Window
    {
        public static readonly DependencyProperty DialogResultBindingProperty =
            DependencyProperty.Register("DialogResultBinding", typeof(bool?), typeof(DialogWindow), new PropertyMetadata(null, DialogResultBindingChanged));

        /// <summary>
        /// Gets or sets the dialog result of the window.
        /// </summary>
        public bool? DialogResultBinding
        {
            get { return (bool?)GetValue(DialogResultBindingProperty); }
            set { SetValue(DialogResultBindingProperty, value); }
        }

        private static void DialogResultBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DialogWindow)d).DialogResult = (bool?)e.NewValue;
        }

        public DialogWindow()
        {
            BindingOperations.SetBinding(this, DialogResultBindingProperty, new Binding("DialogResult"));
        }
    }
}
