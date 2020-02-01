using ImageProcessor;
using Microsoft.Win32;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

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

        #region Event handlers
        private void ButtonLoad_Click(object sender, RoutedEventArgs e)
        {
            fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Файлы изображений|*.bmp;*.png;*.jpg|Все файлы|*.*";
            fileDialog.ShowDialog();

            imagePath = fileDialog.FileName;

            loadedImage = System.Drawing.Image.FromFile(imagePath);
            edittedImage = loadedImage.Clone() as System.Drawing.Image;

            BitmapSource source = GetImageSource(edittedImage);

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
        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as Button).Tag)
            {
                case "origin-image":
                    edittedImage = loadedImage;
                    ImageControl.Source = GetImageSource(edittedImage);
                    break;
                case "brightness-plus":
                    edittedImage = ChangeBrightness(edittedImage, 10);
                    ImageControl.Source = GetImageSource(edittedImage);
                    break;
            }
        }
        #endregion

        #region Utilities
        
        private System.Drawing.Image ChangeBrightness(System.Drawing.Image image, int step) =>
            new ImageFactory()
                .Load(image)
                .Brightness(step)
                .Image;

        #endregion

        #region Helpers

        /// <summary>
        /// Image to BitmapSource converting
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        /// <see cref="https://stackoverflow.com/questions/10077498/show-drawing-image-in-wpf"/>
        private BitmapSource GetImageSource(System.Drawing.Image image)
        {
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

        #endregion
    }
}
