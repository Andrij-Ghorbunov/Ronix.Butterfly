using System.Windows;
using System.Windows.Controls;

namespace Ronix.Framework.WpfToolkit.Controls
{
    public class CollectionGridColumn : CollectionGridColumnBase
    {
        public DataTemplate DataTemplate { get; set; }

        public override void ApplyToControl(ContentControl control)
        {
            control.ContentTemplate = DataTemplate;
        }
    }
}
