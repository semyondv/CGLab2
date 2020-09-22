using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Drawing;

namespace CGLab2_1 {
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            mainImage = new BitmapImage(new Uri(@"D:\Projects\CGLab2\pic.jpg", UriKind.Absolute));
        }
        BitmapImage mainImage;

        private void picture_Loaded(object sender, RoutedEventArgs e) {
            picture.Source = mainImage;
        }

        int[] plot_gray1;
        int[] plot_gray2;

        private void Button_Click(object sender, RoutedEventArgs e) {
            plot_gray1 = new int[256];
            plot_gray2 = new int[256];

            Bitmap gray1_bitmap, gray2_bitmap, diff;
            //Преобразование из BitmapImage в Bitmap
            using (MemoryStream outStream = new MemoryStream()) {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(mainImage));
                enc.Save(outStream);
                gray1_bitmap = new System.Drawing.Bitmap(outStream);
                gray2_bitmap = new System.Drawing.Bitmap(outStream);
                diff = new System.Drawing.Bitmap(outStream);
            }

            for (int i = 0; i < gray1_bitmap.Width; i++) {
                for (int j = 0; j < gray1_bitmap.Height; j++) {
                    //gray1
                    System.Drawing.Color c1 = gray1_bitmap.GetPixel(i, j);
                    byte gr = (byte)(0.2126 * c1.R + 0.7152 * c1.G + 0.0722 * c1.B);
                    c1 = System.Drawing.Color.FromArgb(255, gr, gr, gr);
                    gray1_bitmap.SetPixel(i, j, c1);
                    plot_gray1[gr]++;

                    //gray2
                    System.Drawing.Color c2 = gray2_bitmap.GetPixel(i, j);
                    byte gr2;
                    if ((bool)type_gr.IsChecked) {
                        gr2 = (byte)(0.299 * c2.R + 0.587 * c2.G + 0.114 * c2.B);
                    } else 
                        gr2 = (byte)(0.333 * c2.R + 0.333 * c2.G + 0.333 * c2.B);
                    c2 = System.Drawing.Color.FromArgb(255, gr2, gr2, gr2);
                    gray2_bitmap.SetPixel(i, j, c2);
                    plot_gray2[gr2]++;

                    //difference
                    byte gr_diff = (byte)Math.Abs(gr - gr2);
                    diff.SetPixel(i, j, System.Drawing.Color.FromArgb(255, gr_diff, gr_diff, gr_diff));
                }
            }

            BitmapSource b =
                System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(gray1_bitmap.GetHbitmap(),
                IntPtr.Zero,
                System.Windows.Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            BitmapSource b2 =
                System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(gray2_bitmap.GetHbitmap(),
                IntPtr.Zero,
                System.Windows.Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            BitmapSource b3 =
                System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(diff.GetHbitmap(),
                IntPtr.Zero,
                System.Windows.Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            gray1.Source = (System.Windows.Media.ImageSource)b;
            gray2.Source = (System.Windows.Media.ImageSource)b2;
            difference.Source = (System.Windows.Media.ImageSource)b3;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) {
            canvas_gray1.Children.RemoveRange(0, 255);
            canvas_gray2.Children.RemoveRange(0, 255);

            for (int i = 0; i < plot_gray1.Length; i++) {
                Line l = new Line();
                l.Stroke = System.Windows.SystemColors.GrayTextBrush;
                l.X1 = i;
                l.X2 = i;
                l.Y1 = 0;
                l.Y2 = plot_gray1[i] / 5;
                canvas_gray1.Children.Add(l);

                Line l2 = new Line();
                l2.Stroke = System.Windows.SystemColors.GrayTextBrush;
                l2.X1 = i;
                l2.X2 = i;
                l2.Y1 = 0;
                l2.Y2 = plot_gray2[i] / 5;
                canvas_gray2.Children.Add(l2);
            }
        }
    }
}
