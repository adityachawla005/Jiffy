using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Jiffy.UI
{
    public partial class PreviewWindow : Window
    {
        public PreviewWindow(string filePath)
        {
            InitializeComponent();

            if (!File.Exists(filePath))
            {
                MessageBox.Show("File not found.");
                Close();
                return;
            }

            string ext = Path.GetExtension(filePath).ToLowerInvariant();

            if (ext == ".txt")
            {
                var text = File.ReadAllText(filePath);
                ContentDisplay.Content = new TextBox
                {
                    Text = text,
                    IsReadOnly = true,
                    TextWrapping = TextWrapping.Wrap,
                    Background = null,
                    BorderThickness = new Thickness(0)
                };
            }
            else if (ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".bmp" || ext == ".gif")
            {
                var bytes = File.ReadAllBytes(filePath);
                using var ms = new MemoryStream(bytes);
                BitmapImage img = new BitmapImage();
                img.BeginInit();
                img.StreamSource = ms;
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.EndInit();
                img.Freeze(); // 🔐 Allows access from other threads

                ContentDisplay.Content = new Image
                {
                    Source = img,
                    Stretch = Stretch.Uniform
                };
            }

            else
            {
                ContentDisplay.Content = new TextBlock
                {
                    Text = "Unsupported file format.",
                    Margin = new Thickness(10)
                };
            }
        }
    }
}
