using UnityEngine;
using System.Collections;
using RedCallFrame;

public class RedCallApplication : GameApplication
{
    new public static RedCallApplication GetInstance()
    {
        return (RedCallApplication)(GameApplication.GetInstance());
    }

    protected override void DoInitialize()
    {
        //DebugButton.Initialize();
        //Scene scene = null;
        //if (!GameConfig.noDisplayLogo)
        //{
        //    scene = sceneManager.GetScene<LogoScene>();
        //}
        //else
        //{
        //    scene = sceneManager.GetScene<EntryScene>();
        //}
        //sceneManager.SwitchScene(scene);
    }

    protected override void DoUpdate()
    {
        //SimpleStateManager.GetInstance().Update();
    }

    protected override void DoShutdown()
    {
        GameDataManager.GetInstance().Save();
    }

    protected override void DoPause()
    {
        GameNotificationCenter.GetInstance().OnPause();
    }

    protected override void DoResume()
    {
        GameNotificationCenter.GetInstance().OnResume();
    }

    public long GetPlayingTicks()
    {
        return 0;
    }

}
