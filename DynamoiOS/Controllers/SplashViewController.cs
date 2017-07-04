using System;
using UIKit;
using PortableLibrary;
using MediaPlayer;
using Foundation;

namespace location2
{
    public partial class SplashViewController : BaseViewController
	{
		MPMoviePlayerController _player;
		NSObject _playbackObserver;

		const string NOTIFICATION_PRELOAD_FINISH = "MPMoviePlayerContentPreloadDidFinishNotification";
		const string NOTIFICATION_PLAYBACK_FINISH = "MPMoviePlayerPlaybackDidFinishNotification";

		public SplashViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			PlayVideo("splash.mp4");
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

		private void GotoMainIfAlreadyLoggedin()
		{
            if (!IsNetEnable()) return;

			var nextVC = Storyboard.InstantiateViewController("InitViewController");

			var currentUser = AppSettings.CurrentUser;
			if (currentUser != null)
			{
				if (currentUser.userType == Constants.USER_TYPE.ATHLETE)
				{
					nextVC = Storyboard.InstantiateViewController("MainPageViewController") as MainPageViewController;
				}
				else if (currentUser.userType == (int)Constants.USER_TYPE.COACH)
				{
					var tabVC = Storyboard.InstantiateViewController("CoachHomeViewController") as CoachHomeViewController;
					nextVC = new UINavigationController(tabVC);

					AppDelegate myDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
					myDelegate.navVC = nextVC as UINavigationController;
				}
			}

			PresentViewController(nextVC, false, null);
		}
    }
}
