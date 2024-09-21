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
//ALTENALT MUSTAFA YUSUF AKSU https://app.bio.link/dashboard/links

namespace Text_to_QR
{
    /// <summary>
    /// MainWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent(); // Kullanıcı arayüzünü başlat
        }

        private void GenerateQR_Click(object sender, RoutedEventArgs e)
        {
            GenerateQRCode(); // QR kodu oluşturma fonksiyonunu çağır
        }

        private void GenerateQRCode()
        {
            // Kullanıcının girdiği metin boşsa uyarı mesajı göster
            if (TextInput.Text == null || TextInput.Text == "")
            {
                MessageBox.Show("You need to enter a text");
                return;
            }

            // QR koduna çevirmek istediğin string verisi
            string data = TextInput.Text;

            // QR kodu oluştur
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData;

                // Hata düzeltme seviyesine göre QR kodunu oluştur
                if (error_Correction_Low.IsChecked == true)
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
                    qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q); // Varsayılan hata düzeltme seviyesi
                }

                using (QRCode qrCode = new QRCode(qrCodeData))
                {
                    // Bitmap olarak QR kodunu oluştur
                    BitmapImage qrCodeImage = BitmapToImageSource(qrCode.GetGraphic(20));

                    // Image kontrolüne QR kodunu atama
                    QrImage.Source = qrCodeImage;
                }
            }
        }

        // Bitmap'i WPF'in ImageSource'una dönüştür
        private BitmapImage BitmapToImageSource(System.Drawing.Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                // Bitmap'i bellek akışına kaydet
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0; // Akışın başına döndür
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory; // Akışı kaynak olarak ayarla
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage; // Dönüştürülmüş resmi döndür
            }
        }

        private void QR_Generate(object sender, RoutedEventArgs e)
        {
            // Otomatik oluşturma seçeneği açık ve metin girilmişse QR kodunu oluştur
            if (AutoGenerate != null && AutoGenerate.IsChecked == true && (TextInput.Text != null || TextInput.Text != ""))
            {
                GenerateQRCode();
            }
        }

        private void ImageSave_Click(object sender, RoutedEventArgs e)
        {
            // Resmin kaynağını kontrol et
            if (QrImage.Source is BitmapSource bitmapSource)
            {
                // Kullanıcının kaydetmek istediği yeri seçmesi için dosya kaydetme diyaloğu aç
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "PNG Image|*.png", // Sadece PNG formatını göster
                    Title = "Save Image as PNG" // Pencere başlığı
                };

                // Kullanıcı bir dosya seçerse
                if (saveFileDialog.ShowDialog() == true)
                {
                    using (FileStream fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                    {
                        // PNG encoder oluştur
                        PngBitmapEncoder encoder = new PngBitmapEncoder();

                        // Bitmap kaynağını encode edip dosyaya kaydet
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
            // Resmin kaynağı varsa panoya kopyala
            if (QrImage.Source is BitmapSource bitmapSource)
            {
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
//ALTENALT MUSTAFA YUSUF AKSU https://app.bio.link/dashboard/links