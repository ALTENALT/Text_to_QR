using System;
using System.Collections.Generic;
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
using QRCoder;
using System.IO;
using Microsoft.Win32;

namespace Text_to_QR
{
    /// <summary>
    /// MainWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void GenerateQR_Click(object sender, RoutedEventArgs e)
        {
            GenerateQRCode();
        }

        private void GenerateQRCode()
        {
            if (TextInput.Text == null || TextInput.Text == "") { MessageBox.Show("You need to enter a text"); return; }

            // QR koduna çevirmek istediğin string verisi
            string data = TextInput.Text;

            // QR kodu oluştur
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData;
                if(error_Correction_Low.IsChecked == true)
                {
                    qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.L);
                }
                else if (error_Correction_Medium.IsChecked == true)
                {
                    qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.M);
                }
                else if (error_Correction_Quartile.IsChecked == true)
                {
                    qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
                }
                else if (error_Correction_High.IsChecked == true)
                {
                    qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.H);
                }
                else 
                { 
                    qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
                }

                using (QRCode qrCode = new QRCode(qrCodeData))
                {
                    // Bitmap olarak QR kodunu oluştur
                    BitmapImage qrCodeImage = BitmapToImageSource(qrCode.GetGraphic(20));

                    // Image kontrolüne QR kodu atama
                    QrImage.Source = qrCodeImage;
                }
            }
        }

        // Bitmap'i WPF'in ImageSource'una dönüştür
        private BitmapImage BitmapToImageSource(System.Drawing.Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }
        private void QR_Generate(object sender, RoutedEventArgs e)
        {
            if (AutoGenerate != null && AutoGenerate.IsChecked == true && (TextInput.Text != null || TextInput.Text != ""))
            {
                GenerateQRCode();
            }
        }

        private void ImageSave_Click(object sender, RoutedEventArgs e)
        {
            // Check the source of the Image
            if (QrImage.Source is BitmapSource bitmapSource)
            {
                // Use SaveFileDialog to choose the location to save the file
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "PNG Image|*.png", // Show only PNG format
                    Title = "Save Image as PNG"
                };

                // If the user selected a file
                if (saveFileDialog.ShowDialog() == true)
                {
                    using (FileStream fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                    {
                        // Create PNG encoder
                        PngBitmapEncoder encoder = new PngBitmapEncoder();

                        // Add the bitmap source to the encoder and save it to the file
                        encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                        encoder.Save(fileStream);
                    }
                    MessageBox.Show("Image successfully saved as PNG!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("No image source found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ImageCopy_Click(object sender, RoutedEventArgs e)
        {
            if (QrImage.Source is BitmapSource bitmapSource)
            {
                // Copy the image to the clipboard
                Clipboard.SetImage(bitmapSource);
                MessageBox.Show("Image copied!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("No image available to copy.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
