using UnityEngine;
using System;

namespace RedCallFrame
{
    public class GameApplication
    {
        //private ResourceManager resourceManager;
        protected SceneManager sceneManager;
        private TouchManager touchManager;
        //private NetManager netManager;
        private static GameApplication gameApplication;
        //private GUIFrameMgr guiManager;

        public float LF_INTERVAL = 1.0f / GameConfig.SystemConfig.LFFrameRate;
        public float MF_INTERVAL = 1.0f / GameConfig.SystemConfig.MFFrameRate;
        /// <summary>
        ///  帧更新队列（中频/高频/低频）
        /// </summary>
        private UpdateQueue<IGUpdateListener> _activeQueue = new UpdateQueue<IGUpdateListener>();
        private UpdateQueue<IGUpdateListener> _activeQueueHF = new UpdateQueue<IGUpdateListener>();
        private UpdateQueue<IGUpdateListener> _activeQueueLF = new UpdateQueue<IGUpdateListener>();

        public static GameApplication GetInstance()
        {
            return gameApplication;
        }

        public SceneManager GetSceneManager()
        {
            return sceneManager;
        }

        public GameApplication()
        {
            gameApplication = this;

            sceneManager = new SceneManager();
            touchManager = TouchManager.GetInstance();
            //resourceManager = ResourceManager.Singleton();
            //netManager = NetManager.Singleton();
            //guiManager = GUIFrameMgr.Singleton();
        }

        public void Startup()
        {
            DoInitialize();
            sceneManager.Startup();
            touchManager.Startup();
        }

        public void Terminate()
        {
            sceneManager.Terminate();
            touchManager.Terminate();
            DoShutdown();
        }

        protected virtual void DoInitialize()
        {

        }

        public void Update(float deltaTime)
        {
            try
            {
                ExecuteUpdate(deltaTime);
            }
            catch (System.Exception e)
            {
                Debug.Log(String.Format("[Main Update Exception Info]{0}", e.ToString()));
            }
        }

        private void ExecuteUpdate(float deltaTime)
        {
            touchManager.ProcessTouchEvents();
            sceneManager.Update();

            if (LF_INTERVAL <= TimeUtils.LFDeltaTime)
            {
                _activeQueueLF.Update(TimeUtils.LFDeltaTime);
                TimeUtils.LFDeltaTime = 0;
            }

            if (MF_INTERVAL <= TimeUtils.MFDeltaTime)
            {
                _activeQueue.Update(TimeUtils.MFDeltaTime);
                TimeUtils.MFDeltaTime = 0;
            }

            _activeQueueHF.Update(deltaTime);

            DoUpdate();
        }

        protected virtual void DoUpdate()
        {
        }

        protected virtual void DoShutdown()
        {
        }

        public void Pause()
        {
            DoPause();
        }

        protected virtual void DoPause()
        {
        }

        public void Resume()
        {
            DoResume();
        }

        protected virtual void DoResume()
        {
        }

        // return time in seconds
        public float GetFrameDeltaTime()
        {
            return Time.deltaTime;
        }

        public void OnLevelWasLoaded()
        {
            //resourceManager.OnLevelWasLoaded();
        }

        public void addUpdateListener(IGUpdateListener listener)
        {
            UpdateQueue<IGUpdateListener> selectedQueue = getQueueFromListener(listener);

            selectedQueue.Add(listener);
        }

        public void removeUpdateListener(IGUpdateListener listener)
        {
            UpdateQueue<IGUpdateListener> selectedQueue = getQueueFromListener(listener);

            selectedQueue.Remove(listener);
        }

        public UpdateQueue<IGUpdateListener> getQueueFromListener(IGUpdateListener listener)
        {
            if (listener is IGHFUpdateListener)
            {
                return _activeQueueHF;
            }
            else if (listener is IGLFUpdateListener)
            {
                return _activeQueueLF;
            }
            else
            {
                return _activeQueue;
            }
        }
    }
}

