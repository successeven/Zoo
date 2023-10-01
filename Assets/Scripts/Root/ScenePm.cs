using System;
using System.Linq;
using Containers;
using Logic.Loaders.ResourcesLoader;
using Logic.Loaders.View;
using Tools.Extensions.Reactive;
using Tools.Framework;
using Tools.Pool;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Logic.Scene
{
    public class ScenePm : BaseDisposable
    {
        public struct Ctx
        {
            public IPoolManager poolManager;
            public IResourceLoader resourceLoader;
            // public MainConfiguration mainConfiguration;
            public SceneName sceneName;
            public LoadSt loadSt;
        }

        private readonly Ctx _ctx;

        public ScenePm(Ctx ctx)
        {
            _ctx = ctx;

            // choose logic depends on scene
            SceneContextView sceneContext = FindContext(_ctx.sceneName);
            switch (_ctx.sceneName)
            {
                case SceneName.ZooScene:
                    CreateZooScene();
                    break;
            }

            void CreateZooScene()
            {
                if (sceneContext is not ZooSceneContextView sceneContextView)
                {
                    log.Err("sceneContextView was null");
                    return;
                }

                ZooScenePm.Ctx zooSceneCtx = new ZooScenePm.Ctx
                {
                    resourceLoader = _ctx.resourceLoader,
                    poolManager = _ctx.poolManager,
                    sceneContextView = sceneContextView
                };
                ZooScenePm zooScenePm = new ZooScenePm(zooSceneCtx);
                AddDispose(zooScenePm);
            }
        }
        private static SceneContextView FindContext(SceneName sceneName)
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneAt(0));
//            Debug.Log($"Scene is {SceneManager.GetActiveScene().name} {SceneManager.GetActiveScene().path}");
            SceneContextView[] sceneContexts = Object.FindObjectsOfType<SceneContextView>();
            SceneContextView sceneContext = sceneContexts.FirstOrDefault(ctx =>
            {
                return sceneName switch
                {
                    SceneName.ZooScene => ctx is ZooSceneContextView,
                    _ => false
                };
            });
            return sceneContext;
        }

    }
}