using System;
using Tools.Framework;
using UniRx;
using UnityEngine.SceneManagement;

namespace Logic.Loaders
{
    public class SceneReloader : BaseDisposable, ISceneReloader
    {
        public struct Ctx
        {
        }

        private readonly Ctx _ctx;

        private const string INTRO_SCENE_NAME = "Intro";

        public SceneReloader(Ctx ctx)
        {
            _ctx = ctx;
        }

        public void ReloadFirstScene(Action onComplete)
        {
            if (SceneManager.GetActiveScene().name == INTRO_SCENE_NAME)
            {
                onComplete?.Invoke();
                return;
            }

            // Resources.UnloadUnusedAssets();
            SceneManager.LoadScene(INTRO_SCENE_NAME);
            IDisposable dis = null;
            dis = Observable.NextFrame().Subscribe(_ =>
            {
                onComplete?.Invoke();
                dis?.Dispose();
            });
        }
    }
}