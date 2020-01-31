using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ImageProcessor.Imaging.Filters.Photo;
using ImageProcessor;

namespace ImageProcessing
{
    
    public partial class MainWindow : Window
    {
        #region Declarations

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteObject(IntPtr value);
        OpenFileDialog fileDialog;
        string imagePath;
        System.Drawing.Image loadedImage;
        System.Drawing.Image edittedImage;
        #endregion
        public MainWindow()
        {
            InitializeComponent();
        }
        private void ButtonLoad_Click(object sender, RoutedEventArgs e)
        {
            fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Файлы изображений|*.bmp;*.png;*.jpg|Все файлы|*.*";
            fileDialog.ShowDialog();

            imagePath = fileDialog.FileName;

            loadedImage = System.Drawing.Image.FromFile(imagePath);
          
            BitmapSource source = GetImageSource(loadedImage);

            ImageControl.Source = source;
        }
        private void ButtonUnload_Click(object sender, RoutedEventArgs e)
        {
            ImageControl.Source = null;
        }
        private void ButtonQuit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void ListViewItem_KeyDown(object sender, KeyEventArgs e)
        {
            switch ((sender as ListViewItem).Tag)
            {
                case "gray-scale":
                    break;
                        
            }
        }
        /// <summary>
        /// Image to BitmapSource converting
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public BitmapSource GetImageSource(System.Drawing.Image image)
        {
        // https://stackoverflow.com/questions/10077498/show-drawing-image-in-wpf

            var bitmap = new Bitmap(image);
            IntPtr bmpPt = bitmap.GetHbitmap();
            BitmapSource bitmapSource =
             System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                   bmpPt,
                   IntPtr.Zero,
                   Int32Rect.Empty,
                   BitmapSizeOptions.FromEmptyOptions());

            //freeze bitmapSource and clear memory to avoid memory leaks
            bitmapSource.Freeze();
            DeleteObject(bmpPt);

            return bitmapSource;
        }
    }
}
