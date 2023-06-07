using System;
using System.Windows;
using System.Windows.Markup;
using Ronix.Framework.Extensions;
using Ronix.Framework.Mvvm;

namespace Ronix.Framework.WpfToolkit.MarkupExtensions
{
    public class ToggleCommandExtension : MarkupExtension
    {
        private readonly string _path;

        public ToggleCommandExtension(string path)
        {
            _path = path;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var service = serviceProvider.GetService<IProvideValueTarget>();
            if (service == null)
                return null;
            var target = service.TargetObject as FrameworkElement;
            if (target == null)
                return null;
            return new Command(() =>
            {
                var dc = target.DataContext;
                var prop = dc.GetType().GetProperty(_path);
                prop.SetValue(dc, !(bool)prop.GetValue(dc));
            });
        }
    }
}
