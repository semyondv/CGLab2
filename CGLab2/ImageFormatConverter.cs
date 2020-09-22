using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CGLab2 {
    class ImageFormatConverter {

        class HSVFormat {
            public double Hue { get; set; }
            public double Saturation { get; set; }
            public double Value { get; set; }

            public HSVFormat(double h, double s, double v) {
                Hue = h;
                Saturation = s;
                Value = v;
            }
        }
        HSVFormat[,] image = null;
        int imageWidth;
        int imageHeight;
        public void ColorToHSV(System.Drawing.Color color, out double hue, out double saturation, out double value) {
            double max = Math.Max(color.R, Math.Max(color.G, color.B));
            double min = Math.Min(color.R, Math.Min(color.G, color.B));

            hue = 0;
            if (max == min) {
                hue = new Random().Next(0, 359);
            } else if (max == color.R && color.G >= color.B) {
                hue = 60 * (color.G - color.B) / (max - min);
            } else if (max == color.R && color.G < color.B) {
                hue = 60 * (color.G - color.B) / (max - min) + 360;
            } else if (max == color.G) {
                hue = 60 * (color.B - color.R) / (max - min) + 120;
            } else if (max == color.B) {
                hue = 60 * (color.R - color.G) / (max - min) + 240;
            }
            saturation = (max == 0) ? 0 : 1.0 - (min / max);
            value = max / 255.0;
        }
        public System.Drawing.Color HSVToColor(double hue, double saturation, double value) {
            int hi = Convert.ToInt32(Math.Floor(hue / 60.0)) % 6;
            double f = hue / 60.0 - Math.Floor(hue / 60);

            value = value * 255.0;
            int v = Convert.ToInt32(value);
            int vmin = Convert.ToInt32(value * (1 - saturation));
            int vdec = Convert.ToInt32(value * (1 - f * saturation));
            int vinc = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return System.Drawing.Color.FromArgb(255, v, vinc, vmin);
            else if (hi == 1)
                return System.Drawing.Color.FromArgb(255, vdec, v, vmin);
            else if (hi == 2)
                return System.Drawing.Color.FromArgb(255, vmin, v, vinc);
            else if (hi == 3)
                return System.Drawing.Color.FromArgb(255, vmin, vdec, v);
            else if (hi == 4)
                return System.Drawing.Color.FromArgb(255, vinc, vmin, v);
            else
                return System.Drawing.Color.FromArgb(255, v, vmin, vdec);
        }
        public System.Windows.Media.ImageSource ChangeHSV(BitmapImage source, double hMul, double sMul, double vMul) {
            imageWidth = (int)source.Width;
            imageHeight = (int)source.Height;
            image = new HSVFormat[imageWidth, imageHeight];
            System.Drawing.Bitmap bitmap;
            //Преобразование из BitmapImage в Bitmap
            using (MemoryStream outStream = new MemoryStream()) {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(source));
                enc.Save(outStream);
                bitmap = new System.Drawing.Bitmap(outStream);
            }

            double h, s, v;
            for (int i = 0; i < bitmap.Width; i++) {
                for (int j = 0; j < bitmap.Height; j++) {
                    System.Drawing.Color c = bitmap.GetPixel(i, j);
                    ColorToHSV(c, out h, out s, out v);
                    image[i, j] = new HSVFormat(h, s, v);
                    v += vMul;
                    s += sMul;
                    h += hMul;
                    v = SaveRange(v, 0, 1);
                    s = SaveRange(s, 0, 1);
                    h = SaveRange(h, 0, 360);
                    c = HSVToColor(h, s, v);
                    ////change ranres in XAML
                    //v *= vMul;
                    //s *= sMul;
                    //h *= hMul;
                    bitmap.SetPixel(i, j, c);
                }
            }
            //Преобразование из BitmapImage в Bitmap
            BitmapSource b =
                System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(),
                IntPtr.Zero,
                System.Windows.Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            return (System.Windows.Media.ImageSource)b;
        }
        private double SaveRange(double x, double begin, double end) {
            if (x < begin) {
                return begin;
            }
            if (x > end) {
                return end;
            }
            return x;
        }
        public System.Windows.Media.ImageSource GetOriginalHSV() {
            if (image == null)
                return null;
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(imageWidth, imageHeight);
            double h, s, v;
            for (int i = 0; i < imageWidth ; i++) {
                for (int j = 0; j < imageHeight; j++) {
                    System.Drawing.Color c = HSVToColor(image[i, j].Hue, image[i, j].Saturation, image[i, j].Value);
                    bitmap.SetPixel(i, j, c);
                }
            }
            //Преобразование из BitmapImage в Bitmap
            BitmapSource b =
                System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(),
                IntPtr.Zero,
                System.Windows.Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            return (System.Windows.Media.ImageSource)b;
        }
    }
}
