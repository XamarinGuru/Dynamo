using System;
using UIKit;
using System.Threading.Tasks;
using PortableLibrary;
using Foundation;
using CoreGraphics;
using MediaPlayer;
using System.Drawing;

namespace location2
{
    public partial class SplashViewController : UIViewController
    {
		MPMoviePlayerController _player;
		NSObject _playbackObserver;

		const string NOTIFICATION_PRELOAD_FINISH = "MPMoviePlayerContentPreloadDidFinishNotification";
		const string NOTIFICATION_PLAYBACK_FINISH = "MPMoviePlayerPlaybackDidFinishNotification";

        public SplashViewController (IntPtr handle) : base (handle)
        {
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			PlayVideo("splash.mp4");

            //await Task.Delay(10000);
			//GotoMainIfAlreadyLoggedin();
		}

        void PlayVideo(string videoFileName)
        {
            _player = new MPMoviePlayerController(NSUrl.FromFilename(videoFileName));
            _player.ControlStyle = MPMovieControlStyle.None;
            _player.ScalingMode = MPMovieScalingMode.AspectFill;
            _player.View.Frame = this.View.Frame;
            this.View.InsertSubview(_player.View, 0);

            _playbackObserver = NSNotificationCenter.DefaultCenter.AddObserver(
                (NSString)NOTIFICATION_PLAYBACK_FINISH,
                (notify) =>
                {
                    GotoMainIfAlreadyLoggedin();
                    //_player.Play();
                    notify.Dispose();
                });

            _player.Play();
        }

		public override void ViewDidUnload()
		{
			base.ViewDidUnload();

			if (_player != null)
			{
				NSNotificationCenter.DefaultCenter.RemoveObserver(_playbackObserver);

				_playbackObserver = null;

				_player.Dispose();
				_player = null;
			}
		}

		void GotoMainIfAlreadyLoggedin()
		{
			var nextVC = Storyboard.InstantiateViewController("InitViewController");

			var currentUser = AppSettings.CurrentUser;
			if (currentUser != null)
			{
				if (currentUser.userType == (int)Constants.USER_TYPE.ATHLETE)
				{
					nextVC = Storyboard.InstantiateViewController("MainPageViewController") as MainPageViewController;
				}
				else if (currentUser.userType == (int)Constants.USER_TYPE.COACH)
				{
					var tabVC = Storyboard.InstantiateViewController("CoachHomeViewController") as CoachHomeViewController;
					nextVC = new UINavigationController(tabVC);
					//nextVC = Storyboard.InstantiateViewController("CoachHomeViewController") as CoachHomeViewController;
				}
			}

			PresentViewController(nextVC, false, null);
		}

		void HandleLoadFinished(object sender, EventArgs e)
		{
			var webView = sender as UIWebView;
			CGSize contentSize = webView.ScrollView.ContentSize;
			CGSize viewSize = webView.Bounds.Size;

			float rw = (float)viewSize.Height / (float)contentSize.Height;

			webView.ScrollView.MinimumZoomScale = rw;
			webView.ScrollView.MaximumZoomScale = rw;
			webView.ScrollView.ZoomScale = rw;
		}
    }
}