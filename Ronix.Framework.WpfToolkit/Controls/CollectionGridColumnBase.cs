using System.Windows;
using System.Windows.Controls;

namespace Ronix.Framework.WpfToolkit.Controls
{
    public abstract class CollectionGridColumnBase
    {
        public GridLength Width { get; set; }

        public abstract void ApplyToControl(ContentControl control);

        protected CollectionGridColumnBase()
        {
            Width = GridLength.Auto;
        }
    }
}
