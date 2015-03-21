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
        private MediaCapture captureManager;
        private List<Task<List<PersonShot>>> tasks;
        private static DispatcherTimer aTimer;

        void goToResults()
        {
            this.Frame.Navigate(typeof(BasicPage1), tasks);
        }

        void buttonClick(object sender, RoutedEventArgs e)
        {
            goToResults();
        }

        void mediaEnded(object sender, RoutedEventArgs e)
        {
            aTimer.Stop();
            goToResults();
        }

        void everySecond<TEventArgs>(object sender, TEventArgs e)
        {
            tasks.Add(Task.Run<List<PersonShot>>(() => { return this.TakePicture(); } ));
        }

        private async static Task<MediaCapture> InitCamera()
        {
            MediaCapture captureManager = new MediaCapture();
            var videoDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            var frontCamera = videoDevices.FirstOrDefault(
                item => item.EnclosureLocation != null
                && item.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Front);

            var captureSettings = new MediaCaptureInitializationSettings { };
            if (frontCamera != null)
                captureSettings.VideoDeviceId = frontCamera.Id;

            captureSettings.PhotoCaptureSource = Windows.Media.Capture.PhotoCaptureSource.Auto;
            await captureManager.InitializeAsync(captureSettings);
            
            if(captureManager.VideoDeviceController.ExposureControl.Supported)
                await captureManager.VideoDeviceController.ExposureControl.SetAutoAsync(true);

            captureManager.VideoDeviceController.Brightness.TrySetAuto(true);

            return captureManager;
        }

        public MainPage()
        {
            InitCamera().ContinueWith(cm =>
            {
                this.captureManager = cm.Result;
                this.InitializeComponent();
                tasks = new List<Task<List<PersonShot>>>();
                aTimer = new DispatcherTimer();
                aTimer.Interval = TimeSpan.FromMilliseconds(10000);
                aTimer.Tick += new EventHandler<object>(everySecond);
                aTimer.Start();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public async Task<List<PersonShot>> TakePicture()
        {
            ImageEncodingProperties imgFormat = ImageEncodingProperties.CreateJpeg();

            // create storage file in local app storage
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(
                "TestPhoto.jpg",
                CreationCollisionOption.GenerateUniqueName
            );
            var stream = new InMemoryRandomAccessStream();

            await captureManager.CapturePhotoToStreamAsync(imgFormat, stream);
            float time = -1.0f;
            await this.video.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () => {
                time = (float)this.video.Position.TotalSeconds;
            });

            byte[] b = new byte[stream.Size];
            stream.Seek(0);
            stream.AsStream().Read(b, 0, b.Length);

            using (var s = await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite))
                await s.WriteAsync(b.AsBuffer());
                
            libFace.API api = new libFace.API(
                "e3445ac4dc914e468311861668611d7c",
                "53ae1674d8334c6ca527277b2049ee5d"
            );

            var result = new List<PersonShot>();
            foreach(var person in api.Process(b))
            {
                PersonShot ps = new PersonShot();
                ps.person = person;
                ps.time = time;
                result.Add(ps);
            }
            return result;
        }
    }
}
