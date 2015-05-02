using UnityEngine;
using System.Collections.Generic;

namespace RedCallFrame
{
    public class SceneManager : IGameManager
    {
        private Scene currentScene;
        private Dictionary<string, Scene> sceneMap;

        public SceneManager()
        {
            sceneMap = new Dictionary<string, Scene>();
        }

        public void SwitchScene(Scene scene)
        {
            if (currentScene != null)
            {
                currentScene.OnExit();
            }

            currentScene = scene;

            if (currentScene != null)
            {
                currentScene.OnEnter();
            }
        }

        public Scene GetScene<T>() where T : Scene, new()
        {
            System.Type t = typeof(T);
            string name = t.FullName;
            Scene scene;
            //			if(! sceneMap.TryGetValue(name, out scene))
            {
                scene = new T();
                scene.SetSceneManager(this);
                //				sceneMap.Add(name, scene);
            }
            return scene;
        }

        public void Startup(System.Object param = null)
        {

        }
        public void Terminate(System.Object param = null)
        {

        }

        public void Update()
        {
            if (currentScene != null)
            {
                currentScene.Update();
            }
        }
    }

}
