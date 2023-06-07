using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Ronix.Framework.WpfToolkit.Controls
{
    public class OpaqueClickableImage : Image
    {
        private static readonly PixelFormat[] FormatsWithAlpha =
        {
            PixelFormats.Bgra32, PixelFormats.Pbgra32,
            PixelFormats.Prgba64, PixelFormats.Prgba128Float, PixelFormats.Rgba128Float, PixelFormats.Rgba64
        };

        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            var source = (BitmapSource)Source;

            if (!FormatsWithAlpha.Contains(source.Format))
                return base.HitTestCore(hitTestParameters);

            // Get the pixel of the source that was hit
            var x = (int)(hitTestParameters.HitPoint.X / ActualWidth * source.PixelWidth);
            var y = (int)(hitTestParameters.HitPoint.Y / ActualHeight * source.PixelHeight);

            // Can occur sometimes due to approximate floating point operations
            if (x < 0 || y < 0 || x >= source.PixelWidth || y >= source.PixelHeight)
                return null;

            // Copy the single pixel into a new byte array representing RGBA
            var pixel = new byte[4];
            source.CopyPixels(new Int32Rect(x, y, 1, 1), pixel, 4, 0);

            // Check the alpha (transparency) of the pixel
            // - threshold can be adjusted from 0 to 255
            if (pixel[3] < 10)
                return null;

            return new PointHitTestResult(this, hitTestParameters.HitPoint);
        }
    }
}