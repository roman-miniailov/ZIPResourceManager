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

namespace Sample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ZIPResourceManager.ZIPResourceManager _zipResourceManager;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _zipResourceManager = new ZIPResourceManager.ZIPResourceManager(Sample.Resource.ResourceManager, null);

            image1.Source = LoadImage((byte[])_zipResourceManager.Resources["1.jpg"]);
            image2.Source = LoadImage((byte[])_zipResourceManager.Resources["2.jpg"]);
            image3.Source = LoadImage((byte[])_zipResourceManager.Resources["3.jpg"]);
            image4.Source = LoadImage((byte[])_zipResourceManager.Resources["_4"]);
            image5.Source = LoadImage((byte[])_zipResourceManager.Resources["_5"]);
        }

        private static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }
    }
}
