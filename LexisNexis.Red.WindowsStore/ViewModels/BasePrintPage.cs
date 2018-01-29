using LexisNexis.Red.Common.Segment.Framework;
using LexisNexis.Red.WindowsStore.Views;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Data.Pdf;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Graphics.Printing;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Printing;
using Windows.UI.Xaml.Shapes;

namespace LexisNexis.Red.WindowsStore.ViewModels
{
    public class BasePrintPage
    {
        #region Application Content Size Constants given in percents ( normalized )

        /// <summary>
        /// The percent of app's margin width, content is set at 85% (0.85) of the area's width
        /// </summary>
        protected const double ApplicationContentMarginLeft = 0.075;

        /// <summary>
        /// The percent of app's margin height, content is set at 94% (0.94) of tha area's height
        /// </summary>
        protected const double ApplicationContentMarginTop = 0.03;

        #endregion

        /// <summary>
        /// PrintDocument is used to prepare the pages for printing. 
        /// Prepare the pages to print in the handlers for the Paginate, GetPreviewPage, and AddPages events.
        /// </summary>
        protected PrintDocument printDocument = null;

        /// <summary>
        /// Marker interface for document source
        /// </summary>
        protected IPrintDocumentSource printDocumentSource = null;

        /// <summary>
        /// A list of UIElements used to store the print preview pages.  This gives easy access
        /// to any desired preview page.
        /// </summary>
        internal List<UIElement> printPreviewPages = null;
        public IEnumerable<FrameworkElement> Pages { set; get; }

        public event EventHandler Notify;
        private PdfDocument _pdfDocument;
        public BasePrintPage()
        {
            printPreviewPages = new List<UIElement>();
        }

        /// <summary>
        /// This is the event handler for PrintManager.PrintTaskRequested.
        /// </summary>
        /// <param name="sender">PrintManager</param>
        /// <param name="e">PrintTaskRequestedEventArgs </param>
        protected virtual void PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs e)
        {
            PrintTask printTask = null;
            printTask = e.Request.CreatePrintTask("LexisNexis Red3.0 Printing", sourceRequested =>
                {
                    // Print Task event handler is invoked when the print job is completed.
                    printTask.Completed += (s, args) =>
                    {
                        // Notify the user when the print operation fails.
                        if (args.Completion == PrintTaskCompletion.Failed)
                        {
                            Notify.Invoke(null, null);
                        }
                    };

                    sourceRequested.SetSource(printDocumentSource);
                });
        }

        /// <summary>
        /// This function registers the app for printing with Windows and sets up the necessary event handlers for the print process.
        /// </summary>
        public void RegisterForPrinting()
        {
            // Create the PrintDocument.
            printDocument = new PrintDocument();
            // Save the DocumentSource.
            printDocumentSource = printDocument.DocumentSource;

            // Add an event handler which creates preview pages.
            printDocument.Paginate += CreatePrintPreviewPages;

            // Add an event handler which provides a specified preview page.
            printDocument.GetPreviewPage += GetPrintPreviewPage;

            // Add an event handler which provides all final print pages.
            printDocument.AddPages += AddPrintPages;

            // Create a PrintManager and add a handler for printing initialization.
            PrintManager printMan = PrintManager.GetForCurrentView();
            printMan.PrintTaskRequested += PrintTaskRequested;
        }

        /// <summary>
        /// This function unregisters the app for printing with Windows.
        /// </summary>
        public void UnregisterForPrinting()
        {
            if (printDocument == null)
                return;

            printDocument.Paginate -= CreatePrintPreviewPages;
            printDocument.GetPreviewPage -= GetPrintPreviewPage;
            printDocument.AddPages -= AddPrintPages;

            // Remove the handler for printing initialization.
            PrintManager printMan = PrintManager.GetForCurrentView();
            printMan.PrintTaskRequested -= PrintTaskRequested;
        }



        /// <summary>
        /// This is the event handler for PrintDocument.Paginate. It creates print preview pages for the app.
        /// </summary>
        /// <param name="sender">PrintDocument</param>
        /// <param name="e">Paginate Event Arguments</param>
        protected virtual void CreatePrintPreviewPages(object sender, PaginateEventArgs e)
        {
            // Clear the cache of preview pages 
            printPreviewPages.Clear();
            PrintTaskOptions printingOptions = ((PrintTaskOptions)e.PrintTaskOptions);
            PrintPageDescription pageDescription = printingOptions.GetPageDescription(0);
            foreach (var page in Pages)
            {
                page.Height = pageDescription.PageSize.Height;
                page.Width = pageDescription.PageSize.Width;
                printPreviewPages.Add(page);
            }

            PrintDocument printDoc = (PrintDocument)sender;

            // Report the number of preview pages created
            printDoc.SetPreviewPageCount(printPreviewPages.Count, PreviewPageCountType.Intermediate);
        }

        /// <summary>
        /// This is the event handler for PrintDocument.GetPrintPreviewPage. It provides a specific print preview page,
        /// in the form of an UIElement, to an instance of PrintDocument. PrintDocument subsequently converts the UIElement
        /// into a page that the Windows print system can deal with.
        /// </summary>
        /// <param name="sender">PrintDocument</param>
        /// <param name="e">Arguments containing the preview requested page</param>
        protected virtual void GetPrintPreviewPage(object sender, GetPreviewPageEventArgs e)
        {
            PrintDocument printDoc = (PrintDocument)sender;

            printDoc.SetPreviewPage(e.PageNumber, printPreviewPages[e.PageNumber - 1]);
        }

        /// <summary>
        /// This is the event handler for PrintDocument.AddPages. It provides all pages to be printed, in the form of
        /// UIElements, to an instance of PrintDocument. PrintDocument subsequently converts the UIElements
        /// into a pages that the Windows print system can deal with.
        /// </summary>
        /// <param name="sender">PrintDocument</param>
        /// <param name="e">Add page event arguments containing a print task options reference</param>
        protected virtual void AddPrintPages(object sender, AddPagesEventArgs e)
        {
            // Loop over all of the preview pages and add each one to  add each page to be printied
            for (int i = 0; i < printPreviewPages.Count; i++)
            {
                // We should have all pages ready at this point...
                printDocument.AddPage(printPreviewPages[i]);
            }

            PrintDocument printDoc = (PrintDocument)sender;

            // Indicate that all of the print pages have been provided
            printDoc.AddPagesComplete();
        }

        public async Task GetWebPages()
        {
            Windows.Storage.StorageFile pdfFile = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync("windows-pdf.pdf");
            _pdfDocument = await PdfDocument.LoadFromFileAsync(pdfFile);
            var _Pages = new List<FrameworkElement>();
            if (_pdfDocument != null && _pdfDocument.PageCount > 0)
            {
                for (uint pageNum = 0; pageNum < _pdfDocument.PageCount; pageNum++)
                {
                    var pdfPage = _pdfDocument.GetPage(pageNum);
                    Grid _Page = new Grid();
                    var image = new Image
                    {
                        Height = pdfPage.Size.Height,
                        Width = pdfPage.Size.Width,
                        Stretch = Stretch.Fill,
                        HorizontalAlignment=HorizontalAlignment.Center,
                        VerticalAlignment=VerticalAlignment.Center
                    };
                    _Page.Children.Add(image);
                    if (pdfPage != null)
                    {
                        StorageFolder tempFolder = ApplicationData.Current.TemporaryFolder;
                        StorageFile imgFile = await tempFolder.CreateFileAsync(Guid.NewGuid().ToString() + ".png", CreationCollisionOption.ReplaceExisting);
                        if (imgFile != null)
                        {
                            using (IRandomAccessStream randomStream = await imgFile.OpenAsync(FileAccessMode.ReadWrite))
                            {
                                await pdfPage.RenderToStreamAsync(randomStream);
                                await randomStream.FlushAsync();
                                BitmapImage src = new BitmapImage();
                                src.SetSource(randomStream);
                                image.Source = src;
                            }
                            _Pages.Add(_Page);
                        }
                    }
                }
            }
            Pages = _Pages;
        }

        public async Task GetWebPages(double printHeight)
        {
            //h:998.485714
            double heightSplit = 998.485714;
            var _PageCount = printHeight / heightSplit;
            _PageCount = _PageCount + ((_PageCount > (int)_PageCount) ? 1 : 0);
            StorageFile printImg = await ApplicationData.Current.LocalFolder.GetFileAsync("Print.png");
            // create the pages
            var _Pages = new List<FrameworkElement>();
            for (int i = 0; i < (int)_PageCount; i++)
            {
                StackPanel pagePanel = new StackPanel()
                {
                    Orientation = Orientation.Vertical
                };
                TextBlock header = new TextBlock
                {
                    Text = "Ritchie's Uniform Civil Procedure NSW / Civil Procedure Act 2005 / [ss 64–73] DIVISION 3 OTHER POWERS OF COURT",
                    FontSize = 10,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(18)
                };

                TextBlock footer = new TextBlock
                {
                    Text = string.Format("Currency date: 20 Apr 2015 © 2015 LexisNexis Printed page {0}", i + 1),
                    FontSize = 10,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Margin = new Thickness(18)
                };
                ImageBrush imgBrush = new ImageBrush();
                imgBrush.ImageSource = await GetCroppedBitmapAsync(printImg, new Point(0, i * heightSplit), new Size(704, heightSplit), 1);
                var _Page = new Windows.UI.Xaml.Shapes.Rectangle
                {
                    Height = heightSplit,
                    Width = 704,
                    Fill = imgBrush,
                    Stretch = Stretch.Fill,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center

                };
                pagePanel.Children.Add(header);
                pagePanel.Children.Add(_Page);
                pagePanel.Children.Add(footer);
                Viewbox layout = new Viewbox();
                layout.Child = pagePanel;
                _Pages.Add(layout);
            }
            Pages = _Pages;
        }

        /// <summary>
        /// Get a cropped bitmap from a image file.
        /// </summary>
        /// <param name="originalImageFile">
        /// The original image file.
        /// </param>
        /// <param name="startPoint">
        /// The start point of the region to be cropped.
        /// </param>
        /// <param name="corpSize">
        /// The size of the region to be cropped.
        /// </param>
        /// <returns>
        /// The cropped image.
        /// </returns>
        private async Task<ImageSource> GetCroppedBitmapAsync(StorageFile originalImageFile, Point startPoint, Size corpSize, double scale)
        {
            if (double.IsNaN(scale) || double.IsInfinity(scale))
            {
                scale = 1;
            }
            // Convert start point and size to integer.
            uint startPointX = (uint)Math.Floor(startPoint.X * scale);
            uint startPointY = (uint)Math.Floor(startPoint.Y * scale);
            uint height = (uint)Math.Floor(corpSize.Height * scale);
            uint width = (uint)Math.Floor(corpSize.Width * scale);

            using (IRandomAccessStream stream = await originalImageFile.OpenReadAsync())
            {

                // Create a decoder from the stream. With the decoder, we can get 
                // the properties of the image.
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);

                // The scaledSize of original image.
                uint scaledWidth = (uint)Math.Floor(decoder.PixelWidth * scale);
                uint scaledHeight = (uint)Math.Floor(decoder.PixelHeight * scale);


                // Refine the start point and the size. 
                if (startPointX + width > scaledWidth)
                {
                    //startPointX = scaledWidth - width;
                    width = scaledWidth = startPointX;
                }

                if (startPointY + height > scaledHeight)
                {
                    //startPointY = scaledHeight - height;
                    height = scaledHeight - startPointY;
                }

                // Get the cropped pixels.
                byte[] pixels = await GetPixelData(decoder, startPointX, startPointY, width, height,
                    scaledWidth, scaledHeight);

                // Stream the bytes into a WriteableBitmap
                WriteableBitmap cropBmp = new WriteableBitmap((int)width, (int)height);
                using (System.IO.Stream pixStream = cropBmp.PixelBuffer.AsStream())
                {
                    pixStream.Write(pixels, 0, (int)(width * height * 4));
                }

                return cropBmp;
            }

        }

        /// <summary>
        /// Use BitmapTransform to define the region to crop, and then get the pixel data in the region.
        /// If you want to get the pixel data of a scaled image, set the scaledWidth and scaledHeight
        /// of the scaled image.
        /// </summary>
        /// <returns></returns>
        private async Task<byte[]> GetPixelData(BitmapDecoder decoder, uint startPointX, uint startPointY,
            uint width, uint height, uint scaledWidth, uint scaledHeight)
        {

            BitmapTransform transform = new BitmapTransform();
            BitmapBounds bounds = new BitmapBounds();
            bounds.X = startPointX;
            bounds.Y = startPointY;
            bounds.Height = height;
            bounds.Width = width;
            transform.Bounds = bounds;

            transform.ScaledWidth = scaledWidth;
            transform.ScaledHeight = scaledHeight;

            // Get the cropped pixels within the bounds of transform.
            PixelDataProvider pix = await decoder.GetPixelDataAsync(
                BitmapPixelFormat.Bgra8,
                BitmapAlphaMode.Straight,
                transform,
                ExifOrientationMode.IgnoreExifOrientation,
                ColorManagementMode.ColorManageToSRgb);
            byte[] pixels = pix.DetachPixelData();
            return pixels;
        }
    }
}
