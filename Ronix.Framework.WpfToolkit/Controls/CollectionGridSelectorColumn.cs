using System.Windows.Controls;

namespace Ronix.Framework.WpfToolkit.Controls
{
    public class CollectionGridSelectorColumn : CollectionGridColumnBase
    {
        public DataTemplateSelector DataTemplateSelector { get; set; }

        public override void ApplyToControl(ContentControl control)
        {
            control.ContentTemplateSelector = DataTemplateSelector;
        }
    }
}
