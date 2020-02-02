using ImageProcessor;
using ImageProcessor.Imaging;
using ImageProcessor.Imaging.Filters.Photo;
using Microsoft.Win32;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Image = System.Drawing.Image;

namespace ImageProcessing
{
    public partial class MainWindow : Window
    {
        #region Declarations

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteObject(IntPtr value);

        Image loadedImage;
        Image edittedImage;

        string filePath;
        string fileName;
        string fileNameExtension;
        string fileDirectory;

        ImageFactory factory = new ImageFactory();

        #endregion
        public MainWindow()
        {
            InitializeComponent();
        }

        #region Event handlers
        private void MenuLoad_Click(object sender, RoutedEventArgs e)
        {

            filePath = GetFilePath();
            if (filePath != null)
            {
                loadedImage = Image.FromFile(filePath);
                edittedImage = loadedImage.Clone() as System.Drawing.Image;

                BitmapSource source = GetImageSource(edittedImage);

                ImageControl.Source = source;

                fileName = Path.GetFileNameWithoutExtension(filePath);
                fileNameExtension = Path.GetExtension(filePath);
                fileDirectory = Path.GetDirectoryName(filePath);
            }
        }
        private void MenuSaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFile();
        }
        private void MenuUnload_Click(object sender, RoutedEventArgs e)
        {
            ImageControl.Source = null;

            filePath = string.Empty;
            fileName = String.Empty;
            fileNameExtension = String.Empty;
            fileDirectory = String.Empty;
        }
        private void MenuQuit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as Button).Tag)
            {
                case "reset-image":
                    edittedImage = loadedImage;
                    break;
                case "brightness-plus":
                    edittedImage = ChangeBrightness(edittedImage, 10);
                    break;
                case "pixelate":
                    edittedImage = Pixelate(edittedImage, 10);
                    break;
                case "gray-scale":
                    edittedImage = GrayScale(edittedImage);
                    break;
                case "invert":
                    edittedImage = Invert(edittedImage);
                    break;
                case "comic":
                    edittedImage = Comic(edittedImage);
                    break;
                case "watermark":
                    edittedImage = Watermark(edittedImage);
                    break;
                case "overlay":
                    edittedImage = Overlay(edittedImage);
                    break;
            }
            ImageControl.Source = GetImageSource(edittedImage);
        }
        #endregion

        #region Utilities
        private Image ChangeBrightness(Image image, int step) =>
            factory
                .Load(image)
                .Brightness(step)
                .Image;
        private Image Pixelate(Image image, int size) =>
            factory
                .Load(image)
                .Pixelate(size, null)
                .Image;
        private Image GrayScale(Image image) =>
           factory
               .Load(image)
               .Filter(MatrixFilters.GreyScale)
               .Image;
        private Image Invert(Image image) =>
           factory
               .Load(image)
               .Filter(MatrixFilters.Invert)
               .Image;
        private Image Comic(Image image) =>
          factory
              .Load(image)
              .Filter(MatrixFilters.Comic)
              .Image;

        private Image Watermark(Image image) =>
          factory
              .Load(image)
              .Watermark(new TextLayer()
              {
                  Text = "Protected!",
                  FontSize = 160,
                  Opacity = 70,
                  DropShadow = true,
                  Style = System.Drawing.FontStyle.Bold,
                  FontColor = Color.White
              })
              .Image;
        private Image Overlay(Image image) =>
         factory
             .Load(image)
            .Overlay(new ImageLayer()
            {
                Image = Image.FromFile(GetFilePath()),
                Opacity = 80,
                Size = new System.Drawing.Size(800, 500),
                Position = new System.Drawing.Point(800, 0)
            })
             .Image;
        #endregion

        #region Helpers

        /// <summary>
        /// Image to BitmapSource converting
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        /// <see cref="https://stackoverflow.com/questions/10077498/show-drawing-image-in-wpf"/>
        private BitmapSource GetImageSource(Image image)
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

        private string GetFilePath()
        {
            var fileDialog = new OpenFileDialog();
            fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            fileDialog.Filter = "Файлы изображений|*.bmp;*.png;*.jpg|Все файлы|*.*";

            bool? result = fileDialog.ShowDialog();

            if (result == true)
                return fileDialog.FileName;
            else return null;
        }
        public void SaveFile()
        {
            var dlg = new SaveFileDialog();

            dlg.Title = "Save As ...";
            dlg.InitialDirectory = fileDirectory;
            dlg.FileName = fileName; 
            dlg.DefaultExt = fileNameExtension;
            dlg.Filter = "Файлы изображений|*.bmp;*.png;*.jpg|Все файлы|*.*";

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
                factory.Save(filename);
            }
            #endregion

        }
    }
}
