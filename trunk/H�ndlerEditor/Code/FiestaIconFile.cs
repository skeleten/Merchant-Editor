using System;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.IO;

namespace HändlerEditor.Code
{
    [Serializable]
    public class FiestaIconFile
    {
        private BitmapSource[] Bitmaps { get; set; }

        public FiestaIconFile(string path)
        {
            using (var bitmap = new Bitmap(Image.FromFile(path)))
            {
                long subIconWidth = bitmap.Width / 8;
                long subIconHeight = bitmap.Height / 8;

                Bitmaps = new BitmapImage[64];
                for(int index = 0; index < 64; index++)
                {
                    using(var subIcon = GetSubIcon(bitmap, index, subIconWidth, subIconHeight))
                    {
                        Bitmaps[index] = ConvertToBitmapImage(subIcon);
                    }
                }
            }
        }

        private static Bitmap GetSubIcon(Bitmap source, int index, long subIconWidth, long subIconHeight)
        {
            var subIcon = new Bitmap(source.Width / 8, source.Height / 8);

            long originX = index % 8;
            originX *= (source.Width / 8);
            long originY = (index - (index % 8)) / 8;
            originY *= (source.Height / 8);

            for (int deltaX = 0; deltaX < subIconWidth; deltaX++)
                for (int deltaY = 0; deltaY < subIconHeight; deltaY++)
                {
                    Color pixelColor = source.GetPixel((int)(originX + deltaX), (int)(originY + deltaY));
                    subIcon.SetPixel(deltaX, deltaY, pixelColor);
                }
            return subIcon;
        }

        private static BitmapImage ConvertToBitmapImage(Bitmap subIcon)
        {
            var temporalStorage = new MemoryStream();
            subIcon.Save(temporalStorage, System.Drawing.Imaging.ImageFormat.Png);
            temporalStorage.Seek(0, SeekOrigin.Begin);
            var b = new BitmapImage();
            b.BeginInit();
            b.StreamSource = temporalStorage;
            b.EndInit();
            return b;
        }

        public BitmapImage this[int index]
        {
            get { return (BitmapImage) Bitmaps[index]; }
        }
    }
}
