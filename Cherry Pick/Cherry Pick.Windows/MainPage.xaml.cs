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
using System.Windows.Input;
using Cherry_Pick.Common;
using Windows.Devices.Enumeration;
using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Cherry_Pick
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private string mood;
        private List<Task<List<libFace.Person>>> tasks;

        private static DispatcherTimer aTimer;


        async void goToResults()
        {
            foreach(var task in tasks)
            {
                List<libFace.Person> list = await task;
            }
            this.Frame.Navigate(typeof(BasicPage1), mood);
        }

        void buttonClick(object sender, RoutedEventArgs e)
        {
            goToResults();
        }

        void mediaEnded(object sender, RoutedEventArgs e)
        {
            goToResults();
        }

        async void everySecond<TEventArgs>(object sender, TEventArgs e)
        {
            tasks.Add(this.TakePicture());
        }

        public MainPage()
        {
            this.InitializeComponent();
            tasks = new List<Task<List<libFace.Person>>>();
            aTimer = new DispatcherTimer();
            aTimer.Interval = TimeSpan.FromMilliseconds(1000);
            aTimer.Tick += new EventHandler<object>(everySecond);
            aTimer.Start();
        }

        public async Task<List<libFace.Person>> TakePicture()
        {
            var videoDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            var frontCamera = videoDevices.FirstOrDefault(
                item => item.EnclosureLocation != null
                && item.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Front);
            MediaCapture captureManager = new MediaCapture();

            ImageEncodingProperties imgFormat = ImageEncodingProperties.CreateJpeg();

            // create storage file in local app storage
            try {
                var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(
                    "TestPhoto.jpg",
                    CreationCollisionOption.GenerateUniqueName
                );
                var stream = new InMemoryRandomAccessStream();

                var s = await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite);

                if (frontCamera != null)
                {
                    var captureSettings = new MediaCaptureInitializationSettings { VideoDeviceId = frontCamera.Id };
                    await captureManager.InitializeAsync(captureSettings);
                }
                //await captureManager.VideoDeviceController.ExposureControl.SetAutoAsync(true);
                await captureManager.CapturePhotoToStreamAsync(imgFormat, stream);

                byte[] b = new byte[stream.Size];
                stream.Seek(0);
                await stream.AsStream().ReadAsync(b, 0, b.Length);
                await s.WriteAsync(b.AsBuffer());
                
                libFace.API api = new libFace.API(
                    "e3445ac4dc914e468311861668611d7c",
                    "53ae1674d8334c6ca527277b2049ee5d"
                );

                return api.Process(b);
            } catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                throw e;
            }
        }
    }
}
