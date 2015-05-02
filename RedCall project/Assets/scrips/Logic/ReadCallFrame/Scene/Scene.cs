using UnityEngine;
using System.Collections;

namespace RedCallFrame
{
    public class Scene
    {
        private SceneManager sceneManager;
        //private AnimationPlayer animationPlayer;

        public void OnEnter()
        {
            DoOnEnter();
        }

        protected virtual void DoOnEnter()
        {
        }

        public void OnExit()
        {
            DoOnExit();

            //if (animationPlayer != null)
            //{
            //    animationPlayer.Dispose();
            //    animationPlayer = null;
            //}
        }

        protected virtual void DoOnExit()
        {
        }

        public void Update()
        {
            //if (animationPlayer != null)
            //{
            //    animationPlayer.Update();
            //}

            DoUpdate();
        }

        protected virtual void DoUpdate()
        {
        }

        //public AnimationPlayer GetAnimationPlayer()
        //{
        //    if (animationPlayer == null)
        //    {
        //        animationPlayer = new AnimationPlayer();
        //    }
        //    return animationPlayer;
        //}

        public SceneManager GetSceneManager()
        {
            return sceneManager;
        }

        public void SetSceneManager(SceneManager sceneManager)
        {
            this.sceneManager = sceneManager;
        }

    }

}
