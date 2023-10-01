using Logic.Loaders.ResourcesLoader;
using Logic.Loaders.View;
using Tools.Extensions.Reactive;
using Tools.Framework;
using Tools.Pool;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;

namespace Logic.Scene
{
    public class ZooScenePm : BaseDisposable
    {
        public struct Ctx
        {
            public IPoolManager poolManager;
            public IResourceLoader resourceLoader;
            public ZooSceneContextView sceneContextView;
        }

        private Ctx _ctx;

        public ZooScenePm(Ctx ctx)
        {
            _ctx = ctx;
            //Camera.main =_ctx.sceneContextView.camera
            
            TimeStream timeStream = AddDispose(new TimeStream(new TimeStream.Ctx()));
            var animals = new ReactiveDictionary<int, AnimalInfo>();
            var tryEat = new ReactiveEvent<EatInfo>();

            AnimalSpawnManagerPm.Ctx spawnCtx = new AnimalSpawnManagerPm.Ctx()
            {
                resourceLoader = _ctx.resourceLoader,
                poolManager = _ctx.poolManager,
                timeStream = timeStream,
                zooSceneContextView = _ctx.sceneContextView,
                animals = animals,
                tryEat = tryEat
            };
            AddDispose(new AnimalSpawnManagerPm(spawnCtx));

            FoodChainPm.Ctx foodChainCtx = new FoodChainPm.Ctx()
            {
                animals = animals,
                tryEat = tryEat
            };
        }
    }
}