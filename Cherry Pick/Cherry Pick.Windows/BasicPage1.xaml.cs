using Cherry_Pick.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Cherry_Pick
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class BasicPage1 : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public BasicPage1()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="Common.NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="Common.SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="Common.NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="Common.NavigationHelper.LoadState"/>
        /// and <see cref="Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        private async Task AggregateResults(List<Task<List<PersonShot>>> tasks)
        {
            var ess = new List<libengagement.EngagementSlice>();

            foreach (var task in tasks)
            {
                List<PersonShot> persons = await task;
                foreach (var ps in persons)
                {
                    var es = new libengagement.EngagementSlice();
                    var slice = new libengagement.Slice();

                    slice.start = ps.time - 1.0f;
                    slice.end = ps.time + 9.0f;

                    es.slice = slice;
                    es.engagement = ps.person.ToEngagement();

                    ess.Add(es);
                }
            }

            var b = new libengagement.Blackbox();

            var vmstor = await StorageFile.GetFileFromPathAsync(Package.Current.InstalledLocation.Path + @"\Charities.json");
            var vms = libengagement.VerdictMapsParser.Parse(await Windows.Storage.FileIO.ReadTextAsync(vmstor));
            foreach (var vm in vms)
                b.AddVerdictMap(vm);

            var ssstor = await StorageFile.GetFileFromPathAsync(Package.Current.InstalledLocation.Path + @"\SampleMovie1.json");
            var sss = libengagement.SubjectSlicesParser.Parse(await Windows.Storage.FileIO.ReadTextAsync(ssstor));
            foreach (var ss in sss)
                b.AddSubjectSlice(ss);

            var evs = b.Run(ess);

            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () => {
                if (ess.Count > 0)
                {
                    if (evs[0].engagement > 0.0f)
                        moodText.Text = "Please donate to your best matches:\n\n";
                    else
                        moodText.Text = "It's recommended not to donate:\n\n";

                    foreach (var ev in evs)
                        moodText.Text += String.Format("{0} ({1})\n\n", ev.verdict, ev.engagement);
                }
                else
                    moodText.Text = "Could not capture good images. Please try again.";
            });
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var tasks = (List<Task<List<PersonShot>>>) e.Parameter;
            Task.Run(() => AggregateResults(tasks));
            
            moodText.Text = "Mood: awaiting result";
        }

        #endregion
    }
}
