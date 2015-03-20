using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Cherry_Pick
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.TakePicture();
        }

        public async void TakePicture()
        {
            MediaCapture captureManager = new MediaCapture();
            await captureManager.InitializeAsync();
            ImageEncodingProperties imgFormat = ImageEncodingProperties.CreateJpeg();

            // create storage file in local app storage
            try {
                var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(
                    "TestPhoto.jpg",
                    CreationCollisionOption.GenerateUniqueName
                );
                var stream = new InMemoryRandomAccessStream();
                await captureManager.CapturePhotoToStreamAsync(imgFormat, stream);

                byte[] b = new byte[stream.Size];
                await stream.WriteAsync(b.AsBuffer());
                
            } catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }
    }
}
