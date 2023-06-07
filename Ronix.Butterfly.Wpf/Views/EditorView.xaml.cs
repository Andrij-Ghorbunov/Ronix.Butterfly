using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ronix.Butterfly.Wpf.Views
{
    /// <summary>
    /// Interaction logic for EditorView.xaml
    /// </summary>
    public partial class EditorView : UserControl
    {
        public EditorView()
        {
            InitializeComponent();
        }

        private void ConnectomePartChanged(int columnNumber)
        {
            SynapseEditor.Columns.Clear();

            SynapseEditor.Columns.Add(new DataGridTextColumn { IsReadOnly = true, Binding = new Binding("Number"), Header = "#" });
            SynapseEditor.Columns.Add(new DataGridTextColumn { Binding = new Binding("Bias.Value") { Mode = BindingMode.TwoWay }, Header = "Bias" });
            for (var index = 0; index < columnNumber; index++)
            {
                SynapseEditor.Columns.Add(new DataGridTextColumn { Binding = new Binding($"Data[{index}].Value") { Mode = BindingMode.TwoWay }, Header = $"{index + 1}" });
            }
        }

        private void OnEditorDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is EditorVm oldValue)
                oldValue.ConnectomePartChanged -= ConnectomePartChanged;
            if (e.NewValue is EditorVm newValue)
                newValue.ConnectomePartChanged += ConnectomePartChanged;
        }
    }
}
