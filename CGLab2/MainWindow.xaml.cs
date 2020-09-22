using System;
using System.Collections.Generic;
using System.Drawing;
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

namespace CGLab2 {
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            picture = new BitmapImage(new Uri(@"D:\Projects\CGLab2\pic.jpg", UriKind.Absolute));
            converter = new ImageFormatConverter();
        }
        BitmapImage picture;
        ImageFormatConverter converter;
        private void Image_Loaded(object sender, RoutedEventArgs e) {
            img.Source = picture;
        }
        private void Button_Click(object sender, RoutedEventArgs e) {
            
            img2.Source = converter.ChangeHSV(picture, Hue.Value, Saturation.Value, Value.Value);
        }
        private void Button_Click_1(object sender, RoutedEventArgs e) {
            if (img2.Source != null) {
                string filePath = @"D:\Projects\CGLab2\result.jpg";

                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)img2.Source));
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                    encoder.Save(stream);
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e) {
            img2.Source = converter.GetOriginalHSV();
        }
    }
}
