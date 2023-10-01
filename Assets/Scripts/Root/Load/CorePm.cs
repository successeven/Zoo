using System;
using Containers;
using Logic.Loaders;
using Logic.Loaders.ResourcesLoader;
using Logic.Scene;
using Pool;
using Tools.Framework;
using Tools.Pool;
using UniRx;

namespace Logic
{
    public class CorePm : BaseDisposable
    {
        public struct Ctx
        {
            public IResourceLoader resourceLoader;
            public ISceneLoader sceneLoader;
            public LoadSt loadSt;
        }

        private readonly Ctx _ctx;
        private readonly ReactiveProperty<SceneName> _currentScene;
        private IDisposable _scene;

        public CorePm(Ctx ctx)
        {
            _ctx = ctx;
            _currentScene = new ReactiveProperty<SceneName>(SceneName.ZooScene);
            
            // create pool
            PoolManager.Ctx poolCtx = new PoolManager.Ctx
            {
                name = GetType().Name,
                dontDestroyPool = true
            };
            IPoolManager pool = AddDispose(new PoolManager(poolCtx));
            
            // start observe scene
            AddDispose(_currentScene.Subscribe(scene =>
            {
                string sceneName = scene.ToString();
				
                _ctx.sceneLoader.LoadScene(sceneName, () => { _scene?.Dispose(); }, () =>
                {
                    // create scene context
                    ScenePm.Ctx sceneCtx = new ScenePm.Ctx
                    {
                        resourceLoader = _ctx.resourceLoader,
                        poolManager = pool,
                        loadSt = _ctx.loadSt,
                        sceneName = scene
                    };
                    _scene = new ScenePm(sceneCtx);
                });
            }));
        }

        protected override void OnDispose()
        {
            _scene?.Dispose();
            base.OnDispose();
        }
    }
}