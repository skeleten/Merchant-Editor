using System.Windows.Media.Imaging;
using System.Drawing;
using System.IO;

namespace HändlerEditor.Code
{
    public class FiestaIconFile
    {
        private BitmapImage[] _bitmaps { get; set; }

        public FiestaIconFile(string path)
        {
            using (Bitmap bitmap = new Bitmap(Image.FromFile(path)))
            {
                _bitmaps = new BitmapImage[64];
                for(int index = 0; index < 64; index++)
                {
                    Bitmap bmp = new Bitmap(bitmap.Width / 8, bitmap.Height / 8);
                    long x = index % 8;
                    x *= (bitmap.Width / 8);
                    long y = (index - (index % 8)) / 8;
                    y *= (bitmap.Height / 8);

                    long w = bitmap.Width / 8;
                    long h = bitmap.Height / 8;

                    for (int dx = 0; dx < w; dx++)
                        for (int dy = 0; dy < h; dy++)
                        {
                            Color cl = bitmap.GetPixel((int)(x + dx), (int)(y + dy));
                            bmp.SetPixel(dx, dy, cl);
                        }

                    MemoryStream ms = new MemoryStream();
                    bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    ms.Seek(0, SeekOrigin.Begin);
                    BitmapImage b = new BitmapImage();
                    b.BeginInit();
                    b.StreamSource = ms;
                    b.EndInit();
                    _bitmaps[index] = b;
                }
                bitmap.Dispose();
            }
        }
        public BitmapImage this[int index]
        {
            get { return _bitmaps[index]; }
        }
    }
}
