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
    /// Interaction logic for TrainingView.xaml
    /// </summary>
    public partial class TrainingView : UserControl
    {
        public TrainingView()
        {
            InitializeComponent();
        }

        private void TrainingDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is TrainingVm oldValue)
                oldValue.OnNewLogRecord -= OnNewLogRecord;
            if (e.NewValue is TrainingVm newValue)
                newValue.OnNewLogRecord += OnNewLogRecord;

        }

        private void OnNewLogRecord()
        {
            LogListViewer.ScrollToEnd();
        }
    }
}
