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
            
            TimeStream timeStream = AddDispose(new TimeStream(new TimeStream.Ctx()));
            var animals = new ReactiveDictionary<int, AnimalInfo>();
            var showLabel = new ReactiveEvent<int>();

            AnimalSpawnManagerPm.Ctx spawnCtx = new AnimalSpawnManagerPm.Ctx()
            {
                resourceLoader = _ctx.resourceLoader,
                poolManager = _ctx.poolManager,
                timeStream = timeStream,
                zooSceneContextView = _ctx.sceneContextView,
                animals = animals,
                showLabel = showLabel
            };
            AddDispose(new AnimalSpawnManagerPm(spawnCtx));

            TastyShowerPm.Ctx tastyCtx = new TastyShowerPm.Ctx
            {
                resourceLoader = _ctx.resourceLoader,
                poolManager = _ctx.poolManager,
                timeStream = timeStream,
                allAnimals = animals,
                showLabel = showLabel,
                PlaceForAnimalUi = _ctx.sceneContextView.PlaceForAnimalUi,
                camera = _ctx.sceneContextView.sceneCamera
            };
            AddDispose(new TastyShowerPm(tastyCtx));

            MainUIPm.Ctx mainUiCtx = new MainUIPm.Ctx
            {
                resourceLoader = _ctx.resourceLoader,
                poolManager = _ctx.poolManager,
                PlaceForAllUi = _ctx.sceneContextView.UiParent,
                allAnimals = animals
            };
            AddDispose(new MainUIPm(mainUiCtx));
        }
    }
}