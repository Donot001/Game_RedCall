using UnityEngine;
using System;

public class GameNotificationCenter
{
    private static GameNotificationCenter instance;

    private bool allowPush;

    public static GameNotificationCenter GetInstance()
    {
        if (instance == null)
        {
            instance = new GameNotificationCenter();
        }

        return instance;
    }

    public GameNotificationCenter()
    {
        this.allowPush = true;
    }

    public void OnPause()
    {
        DoUpdateNotifications();
    }

    public void OnResume()
    {
        DoUpdateNotifications();
    }

    public void SetAllowPush(bool allowPush)
    {
        this.allowPush = allowPush;
    }

    private void DoUpdateNotifications()
    {
        GameNotificationService.CancelAllLocalNotifications();

        if (!this.allowPush)
        {
            return;
        }

        GameNotificationService.ScheduleLocalNotification(DoCreateNotificationGeneralPush1210());
        GameNotificationService.ScheduleLocalNotification(DoCreateNotification3Days());

        if (RedCallApplication.GetInstance().GetPlayingTicks() > (10L * 1000L * 1000L) * 60L * 90L)
        { // 90 minutes
            // nothing to push
        }
        else
        {
            if (true)
            { // finished two quests
                GameNotificationService.ScheduleLocalNotification(DoCreateNotificationTwoTasks());
            }
            else
            {
                GameNotificationService.ScheduleLocalNotification(DoCreateNotificationAdventure());
            }
        }
    }

    private GameLocalNotification DoCreateNotificationGeneralPush1210()
    {
        GameLocalNotification notification = new GameLocalNotification("12:10", "I miss you");
        DateTime dateTime = DateTime.Now.AddDays(1);
        DateTime fireTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 12, 10, 0);
        notification.SetFireDate(fireTime);
        notification.SetRepeatInterval(GameCalendarUnit.Day);
        return notification;
    }

    private GameLocalNotification DoCreateNotificationAdventure()
    {
        GameLocalNotification notification = new GameLocalNotification("20:00 adventure", "I miss you");
        DateTime dateTime = DateTime.Now.AddDays(0);
        DateTime fireTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 20, 0, 0);
        notification.SetFireDate(fireTime);
        notification.SetRepeatInterval(GameCalendarUnit.Day);
        return notification;
    }

    private GameLocalNotification DoCreateNotificationTwoTasks()
    {
        GameLocalNotification notification = new GameLocalNotification("20:00 two tasks", "I miss you");
        DateTime dateTime = DateTime.Now.AddDays(0);
        DateTime fireTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 20, 0, 0);
        notification.SetFireDate(fireTime);
        notification.SetRepeatInterval(GameCalendarUnit.Day);
        return notification;
    }

    private GameLocalNotification DoCreateNotification3Days()
    {
        GameLocalNotification notification = new GameLocalNotification("3 days", "I miss you");
        DateTime dateTime = DateTime.Now.AddDays(3);
        DateTime fireTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 21, 0, 0);
        notification.SetFireDate(fireTime);
        notification.SetRepeatInterval(GameCalendarUnit.Week);
        return notification;
    }

}

