using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Bitmap image;

        public MainWindow()
        {
            InitializeComponent();
            image = new Bitmap(@"D:\Projects\CGLab2\pic.jpg");
        }

        //Выделение канала RGB
        private static Bitmap GetRGB(Bitmap image, string name_ch)
        {
            Bitmap bmp = new Bitmap(image.Width, image.Height);
            switch (name_ch)
            {
                case "Red":
                    for (int i = 0; i < image.Height; i++)
                    {
                        for (int j = 0; j < image.Width; j++)
                        {
                            System.Drawing.Color color = image.GetPixel(j, i);
                            color = System.Drawing.Color.FromArgb(255, color.R, 0, 0);
                            bmp.SetPixel(j, i, color);
                        }
                    }
                    break;
                case "Green":
                    for (int i = 0; i < image.Height; i++)
                    {
                        for (int j = 0; j < image.Width; j++)
                        {
                            System.Drawing.Color color = image.GetPixel(j, i);
                            color = System.Drawing.Color.FromArgb(255, 0, color.G, 0);
                            bmp.SetPixel(j, i, color);
                        }
                    }
                    break;
                case "Blue":
                    for (int i = 0; i < image.Height; i++)
                    {
                        for (int j = 0; j < image.Width; j++)
                        {
                            System.Drawing.Color color = image.GetPixel(j, i);
                            color = System.Drawing.Color.FromArgb(255, 0, 0, color.B);
                            bmp.SetPixel(j, i, color);
                        }
                    }
                    break;
                default: break;

            }
            return bmp;
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch (colorlist.Text)
            {
                case "Red": picture2.Source = GetBitmapSource(GetRGB(image, "Red")); break;
                case "Green": picture2.Source = GetBitmapSource(GetRGB(image, "Green")); break;
                case "Blue": picture2.Source = GetBitmapSource(GetRGB(image, "Blue")); break;
                default: break;
            }
        }

        //получить BitmapSource из Bitmap
        private static BitmapSource GetBitmapSource(Bitmap b)
        {
            var ScreenCapture = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
            b.GetHbitmap(),
            IntPtr.Zero,
            System.Windows.Int32Rect.Empty,
            BitmapSizeOptions.FromWidthAndHeight(b.Width, b.Height));

            return ScreenCapture;
        }

        private void picture1_Loaded(object sender, RoutedEventArgs e)
        {
            picture1.Source = GetBitmapSource(image);
        }


        public static Bitmap BuildHist(Bitmap image, string name_ch)
        {
                // определяем размеры гистограммы.   
                int width = 700;
                int height = 500;
              
                Bitmap hist = new Bitmap(width, height);
                
                // создаем массив, в котором будут содержаться количества повторений для каждого из значений каналов.
                int[] arr_col = new int[256];
  
                System.Drawing.Color color;
                // собираем статистику для изображения
                for (int i = 0; i < width; ++i)
                    for (int j = 0; j < height; ++j)
                    {
                        color = image.GetPixel(i, j);
                        if(name_ch == "Red")
                        {
                            arr_col[color.R]++;
                        }
                        else if (name_ch == "Green")
                        {
                            arr_col[color.G]++;
                        }
                        else
                        {
                            arr_col[color.B]++;
                        }
                    }

                // находим самый высокий столбец, чтобы корректно масштабировать гистограмму по высоте
                int max = 0;
                for (int i = 0; i < 256; ++i)
                {
                    if (arr_col[i] > max)
                        max = arr_col[i];
                }

                // определяем коэффициент масштабирования по высоте
                double point = (double)max / height;

                //цвет для гистограммы
                System.Drawing.Color color_hist;

                if (name_ch == "Red")
                {
                    color_hist = System.Drawing.Color.Red;
                }
                else if (name_ch == "Green")
                {
                    color_hist = System.Drawing.Color.Green;
                }
                else
                {
                   color_hist = System.Drawing.Color.Blue;
                }

            // отрисовываем гистограмму с учетом масштаба
            for (int i = 0; i < width - 3; ++i)
            {
                for (int j = height - 1; j > height - arr_col[i / 3] / point; --j)
                {
                    hist.SetPixel(i, j, color_hist);
                }
                
            }

            return hist;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            picture3.Source = GetBitmapSource(BuildHist(image, "Red"));
            picture4.Source = GetBitmapSource(BuildHist(image, "Green"));
            picture5.Source = GetBitmapSource(BuildHist(image, "Blue"));
        }

        
    }
}
