using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace HwndExplorer.Utilities
{
    public static class ImageLibrary
    {
        private static readonly Lazy<ImageList> _images = new(GetImages);
        public static ImageList Images => _images.Value;

        private static ImageList GetImages()
        {
            using (var stream = typeof(ImageLibrary).Assembly!.GetManifestResourceStream(typeof(Resources).Namespace + ".ImageLibrary32.bmp"))
            {
                if (stream == null)
                    throw new InvalidOperationException();

                return GetImageList(stream);
            }
        }

        private static ImageList GetImageList(Stream imageStream)
        {
            var list = new ImageList
            {
                ColorDepth = ColorDepth.Depth32Bit,
                ImageSize = new Size(16, 16),
            };

            var bitmap = new Bitmap(imageStream);
            list.Images.AddStrip(bitmap);
            return list;
        }
    }
}
