using UnityEngine;
using System.Collections;
using RedCallFrame;

public class main : MonoBehaviour {

    public string applicationClass;
    GameApplication application;

    void Awake()
    {
        GameObject.DontDestroyOnLoad(gameObject);
    }
	// Use this for initialization
	void Start () {
        System.Type applicationType = System.Type.GetType(applicationClass);
        application = (GameApplication)System.Activator.CreateInstance(applicationType);
        application.Startup();
	}
	
	// Update is called once per frame
	void Update () {
        application.Update(Time.deltaTime);
	}

    private void InitPlatform()
    {
        Application.targetFrameRate = GameConfig.SystemConfig.targetFrameRate;

        //让程序在後台进也运行
        Application.runInBackground = true;
        //设置後台加栽进程的优先级
        Application.backgroundLoadingPriority = UnityEngine.ThreadPriority.Low;
    }

    void OnApplicationQuit()
    {
        application.Terminate();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (application != null)
        {
            if (pauseStatus)
            {
                application.Pause();
            }
            else
            {
                application.Resume();
            }
        }
    }
}
