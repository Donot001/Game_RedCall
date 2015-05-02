using UnityEngine;
using System.Collections;

public class GameNotificationService
{
    public static void ScheduleLocalNotification(GameLocalNotification notification)
    {
#if UNITY_IPHONE
		string title = notification.GetTitle();
		bool hasTitle = (title != null);
		if(title == null) {
			title = "";
		}

		LocalNotification localNotification = new LocalNotification();
		localNotification.alertAction = title;
		localNotification.alertBody = notification.GetDescription();
		localNotification.fireDate = notification.GetFireDate();
		localNotification.applicationIconBadgeNumber = 1;
		localNotification.soundName = LocalNotification.defaultSoundName;
		localNotification.repeatInterval = (UnityEngine.CalendarUnit)(notification.GetRepeatInterval());
		localNotification.hasAction = hasTitle;

		NotificationServices.ScheduleLocalNotification(localNotification);
#endif
    }

    public static void CancelAllLocalNotifications()
    {
#if UNITY_IPHONE
		NotificationServices.CancelAllLocalNotifications();
#endif
    }

}
