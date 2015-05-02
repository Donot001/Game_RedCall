using UnityEngine;
using System;

public class GameLocalNotification
{
    private string title;
    private string description;
    private DateTime fireDate;
    private GameCalendarUnit repeatInterval;

    public GameLocalNotification(string title, string description)
    {
        this.title = title;
        this.description = description;

        this.fireDate = DateTime.Now;
        this.repeatInterval = GameCalendarUnit.Day;
    }

    public string GetTitle()
    {
        return this.title;
    }

    public string GetDescription()
    {
        return this.description;
    }

    public void SetFireDate(DateTime fireDate)
    {
        this.fireDate = fireDate;
    }

    public DateTime GetFireDate()
    {
        return this.fireDate;
    }

    public void SetRepeatInterval(GameCalendarUnit repeatInterval)
    {
        this.repeatInterval = repeatInterval;
    }

    public GameCalendarUnit GetRepeatInterval()
    {
        return this.repeatInterval;
    }

}
