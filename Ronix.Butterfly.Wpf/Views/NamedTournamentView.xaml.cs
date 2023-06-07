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
    /// Interaction logic for NamedTournamentView.xaml
    /// </summary>
    public partial class NamedTournamentView : UserControl
    {
        public NamedTournamentView()
        {
            InitializeComponent();
        }

        public void BuildColumns(string[] names)
        {
            DetailedGrid.Columns.Clear();
            DetailedGrid.Columns.Add(new DataGridTextColumn { Header = "Name", Binding = new Binding("Name") });
            var index = 0;
            foreach (var name in names)
            {
                DetailedGrid.Columns.Add(new DataGridTextColumn { Header = name, Binding = new Binding($"Score[{index++}]") });
            }
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is NamedTournamentVm oldValue)
            {
                oldValue.BuildColumns -= BuildColumns;
            }
            if (e.NewValue is NamedTournamentVm newValue)
            {
                newValue.BuildColumns += BuildColumns;
            }
        }
    }
}
