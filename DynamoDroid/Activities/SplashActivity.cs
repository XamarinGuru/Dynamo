using Android.App;
using Android.Content;
using Android.OS;
using System.Threading.Tasks;
using Android.Content.PM;
using PortableLibrary;
using Com.GrapeCity.Xuni.Core;
using Firebase.Iid;
using System.Net;
using Android.Support.V4.App;
using System.Threading;
using Android.Widget;
using Android.Views;

namespace goheja
{
	[Activity(Theme = "@android:style/Theme.NoTitleBar.Fullscreen", MainLauncher = true, NoHistory = true, ScreenOrientation = ScreenOrientation.Portrait)]

	public class SplashActivity : BaseActivity
	{
		VideoView videoView;
		protected override void OnCreate(Bundle savedInstanceState)
		{
			Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
			base.OnCreate(savedInstanceState);

			LicenseManager.Key = License.Key;

			ConfigureFireBase();

			SetContentView(Resource.Layout.SplashActivity);
			videoView = FindViewById<VideoView>(Resource.Id.videoView);

//#if DEBUG
//			GotoMainIfAlreadyLoggedin(null, null);
//#else
            var aaa = Android.Net.Uri.Parse("android.resource://" + Application.PackageName + "/" + Resource.Raw.splash);
            videoView.SetVideoURI(aaa);
            videoView.Start();

            videoView.Completion += GotoMainIfAlreadyLoggedin;
//#endif
		}

		private void GotoMainIfAlreadyLoggedin(object sender, System.EventArgs e)
		{
			if (!IsNetEnable()) return;

			NotificationManager notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
			notificationManager.Notify(1, CreateNotification());

			ThreadPool.QueueUserWorkItem(delegate
			{
				var currentUser = AppSettings.CurrentUser;

				Intent nextIntent = new Intent(this, typeof(InitActivity));
				if (currentUser != null)
				{
					if (currentUser.userType == Constants.USER_TYPE.ATHLETE)
					{
						nextIntent = new Intent(this, typeof(SwipeTabActivity));
					}
					else if (currentUser.userType == (int)Constants.USER_TYPE.COACH)
					{
						nextIntent = new Intent(this, typeof(CoachHomeActivity));
					}
				}

				StartActivityForResult(nextIntent, 0);
				Finish();
			});
		}

		public Notification CreateNotification()
		{
			Intent intent = new Intent(this, typeof(SplashActivity));

			if (AppSettings.CurrentUser != null)
			{
				intent = new Intent(this, typeof(NotificationActivity));
				intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
			}
			var contentIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.UpdateCurrent);

			var textStyle = new NotificationCompat.BigTextStyle();
			textStyle.SetBigContentTitle(ApplicationInfo.LoadLabel(PackageManager) + " on the go");
			textStyle.BigText("Tap to open");

			var clossIntent = new Intent(this, typeof(CloseApplicationActivity));
			clossIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask | ActivityFlags.ClearTop);
			var dismissIntent = PendingIntent.GetActivity(this, 0, clossIntent, PendingIntentFlags.CancelCurrent);
			var closeAction = new NotificationCompat.Action(Resource.Drawable.switch_off, "Switch off", dismissIntent);

			var builder = new NotificationCompat.Builder(this)
												.SetContentTitle(ApplicationInfo.LoadLabel(PackageManager) + " on the go")
												.SetContentText("Tap to open")
												.SetSmallIcon(Resource.Drawable.icon_notification)
												.SetPriority(1)
												.SetContentIntent(contentIntent)
												.SetCategory("tst")
												.AddAction(closeAction);

			var n = builder.Build();
			n.Flags |= NotificationFlags.NoClear;
			return n;
		}

		private void ConfigureFireBase()
		{
#if DEBUG
			Task.Run(() =>
			{
				var instanceId = FirebaseInstanceId.Instance;
				instanceId.DeleteInstanceId();
				Android.Util.Log.Debug("TAG", "{0} {1}", instanceId?.Token?.ToString(), instanceId.GetToken(GetString(Resource.String.gcm_defaultSenderId), Firebase.Messaging.FirebaseMessaging.InstanceIdScope));
			});
			// For debug mode only - will accept the HTTPS certificate of Test/Dev server, as the HTTPS certificate is invalid /not trusted
			ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) => true;
#endif
		}
	}
}

