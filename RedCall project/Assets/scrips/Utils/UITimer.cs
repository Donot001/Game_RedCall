using UnityEngine;
using System.Collections;
using System;
using RedCallFrame;

public class UITimer : IGUpdateListener
{
    public delegate void TickHandler(UITimer uiTimer);
    /// <summary>
    /// 计时结束
    /// </summary>
    public event SimpleSignal OnTimeOut;

    /// <summary>
    /// 每跳一秒抛一次(计时结束的最後一次不抛)
    /// </summary>
    public event TickHandler OnTick;
    /// <summary>
    /// 传递数据
    /// </summary>
    public object Data;
    /// <summary>
    /// 获取剩余时间
    /// </summary>
    public float RemainingTime
    {
        get { return remainingTime; }
    }

    private bool bTicking = false;
    private float remainingTime = 0f;

    /// <summary>
    /// 开始倒计时
    /// </summary>
    /// <param name="seconds">时间（秒）</param>
    public void StartTimer(float seconds)
    {
        remainingTime = seconds;
        if (!bTicking)
        {
            bTicking = true;
            GameApplication.GetInstance().addUpdateListener(this);
        }
    }

    /// <summary>
    /// 停止倒计时
    /// </summary>
    public void StopTimer()
    {
        remainingTime = 0f;
        if (bTicking)
        {
            bTicking = false;
            GameApplication.GetInstance().removeUpdateListener(this);
        }
    }

    public void FrameUpdate(float deltaTime)
    {
        if (!bTicking)
            return;

        float old = remainingTime;
        remainingTime -= deltaTime;

        if (remainingTime <= 0f)
        {
            StopTimer();
            if (OnTimeOut != null)
            {
                OnTimeOut();
            }
        }
        else
        {
            if (Mathf.Floor(old) > Mathf.Floor(remainingTime))
            {
                if (OnTick != null)
                {
                    OnTick(this);
                }
            }
        }
    }

    public void ClearEvent()
    {
        OnTimeOut = null;

        OnTick = null;
    }
}
