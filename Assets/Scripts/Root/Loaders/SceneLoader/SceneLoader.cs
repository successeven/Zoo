using System;
using Logic.Loaders.ResourcesLoader;
using Tools.Framework;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Logic.Loaders.SceneLoader
{
    public class SceneLoader : BaseDisposable, ISceneLoader
    {
        public struct Ctx
        {
            public IResourceLoader resourceLoader;
        }

        private readonly Ctx _ctx;

        private bool _isLoading;
        private IDisposable _unloadingLevel;
        private IDisposable _loadingLevel;

        public SceneLoader(Ctx ctx)
        {
            _ctx = ctx;
            //log.Mute = true;
            _isLoading = false;
        }

        public void LoadScene(string sceneName, Action onUnload, Action onComplete)
        {
            if (_isLoading)
            {
                log.Err($"Can't start load {sceneName}. Level loader is busy");
                onComplete?.Invoke();
                return;
            }

            _isLoading = true;
            log.Info($"Trying to load scene {sceneName}");
            UnityEngine.SceneManagement.Scene oldScene = SceneManager.GetActiveScene();
            LoadSceneAsync(sceneName, OnNewSceneLoaded);


            void OnNewSceneLoaded()
            {
                log.Info($"Scene {sceneName} loaded");
                _isLoading = false;
                _loadingLevel?.Dispose();
                onUnload?.Invoke();
                TryUnloadScene(oldScene, onComplete);
            }
        }

        protected override void OnDispose()
        {
            Reset();
            base.OnDispose();
        }

        private void Reset()
        {
            _isLoading = false;
            _unloadingLevel?.Dispose();
            _loadingLevel?.Dispose();
        }

        private void LoadSceneAsync(string name, Action onComplete)
        {
            // Resources.UnloadUnusedAssets();
            _loadingLevel?.Dispose();
            //AsyncOperation loadingSceneOp = SceneManager.LoadSceneAsync(name,new LoadSceneParameters(LoadSceneMode.Additive, LocalPhysicsMode.Physics3D));
            AsyncOperation loadingSceneOp = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
            _loadingLevel = loadingSceneOp.AsAsyncOperationObservable().Take(1)
                .Subscribe(_ => { onComplete?.Invoke(); });
        }

        private void TryUnloadScene(UnityEngine.SceneManagement.Scene sceneToUnload, Action onComplete)
        {
            _unloadingLevel?.Dispose();
            try
            {
                // SceneManager.CreateScene(EMPTY_SCENE_NAME);
                AsyncOperation unloadingSceneOp = SceneManager.UnloadSceneAsync(sceneToUnload);
                if (unloadingSceneOp == null)
                {
                    onComplete?.Invoke();
                    return;
                }

                _unloadingLevel = unloadingSceneOp.AsAsyncOperationObservable().Take(1)
                    .Subscribe(_ => { onComplete?.Invoke(); });
            }
            catch (Exception e)
            {
                log.Err($"Exception in unloading scene {e}.");
                onComplete?.Invoke();
            }
        }
    }
}