using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Carpeddit.App.Dialogs
{
    public sealed partial class ImagePreviewDialog : ContentDialog
    {
        private bool isSingle;

        private readonly List<Reddit.Things.Image> _images = new();

        private Uri _singleImageUri;

        public ImagePreviewDialog()
        {
            InitializeComponent();
        }

        public ImagePreviewDialog(List<Reddit.Things.Image> images)
        {
            InitializeComponent();
            _images.AddRange(images);
        }

        public ImagePreviewDialog(Uri uri)
        {
            InitializeComponent();
            _singleImageUri = uri;
            isSingle = true;
        }

        private void SingleImage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ImageViewer.ChangeView(0, 0, 1);
            SingleImage.Source = new BitmapImage()
            {
                UriSource = _singleImageUri
            };
        }

        private void CloseButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Hide();
        }
    }
}
