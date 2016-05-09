using Android;
using Android.App;
using Android.Content;
using Badge.Plugin.Abstractions;

namespace Badge.Plugin
{
    /// <summary>
    /// Implementation of Badge for Android
    /// </summary>
    public class BadgeImplementation : IBadge
    {
        private const int BadgeNotificationId = int.MinValue;
        public static int NotificationIconId { get; set; }
        /// <summary>
        /// Sets the badge.
        /// </summary>
        /// <param name="badgeNumber">The badge number.</param>
        /// <param name="title">The title. Used only by Android</param>
        public void SetBadge(int badgeNumber, string title = null)
        {
            var notificationManager = getNotificationManager();
            var notification = createNativeNotification(badgeNumber, title ?? string.Format("{0} new messages", badgeNumber));

            notificationManager.Notify(BadgeNotificationId, notification);
        }

        /// <summary>
        /// Clears the badge.
        /// </summary>
        public void ClearBadge()
        {
            var notificationManager = getNotificationManager();
            notificationManager.Cancel(BadgeNotificationId);
        }
        public static Intent GetLauncherActivity()
        {
            var packageName = Application.Context.PackageName;
            return Application.Context.PackageManager.GetLaunchIntentForPackage(packageName);
        }
        private NotificationManager getNotificationManager()
        {
            var notificationManager = Application.Context.GetSystemService(Context.NotificationService) as NotificationManager;
            return notificationManager;
        }

        private Notification createNativeNotification(int badgeNumber, string title)
        {
            var builder = new Notification.Builder(Application.Context)
                .SetContentTitle(title)
                .SetTicker(title)
                .SetNumber(badgeNumber);
            if (NotificationIconId != 0)
            {
                builder.SetSmallIcon(NotificationIconId);
            }
            else
            {
                builder.SetSmallIcon(Resource.Drawable.plugin_lc_smallicon);
            }
            var resultIntent = GetLauncherActivity();
            resultIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
            var stackBuilder = Android.Support.V4.App.TaskStackBuilder.Create(Application.Context);
            stackBuilder.AddNextIntent(resultIntent);
            var resultPendingIntent =
                stackBuilder.GetPendingIntent(0, (int)PendingIntentFlags.UpdateCurrent);
            builder.SetContentIntent(resultPendingIntent);

            var nativeNotification = builder.Build();
            return nativeNotification;
        }
    }
}