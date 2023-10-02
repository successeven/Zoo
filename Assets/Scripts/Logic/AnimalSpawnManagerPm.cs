using Animals.Factories;
using Logic.Loaders.ResourcesLoader;
using Logic.Scene;
using Logic.Scene.Animals.Core;
using Logic.Scene.Animals.Snake;
using Logic.Settings;
using Tools.Extensions.Reactive;
using Tools.Framework;
using Tools.Pool;
using UniRx;
using UnityEngine;

namespace Logic.Loaders.View
{
    public class AnimalSpawnManagerPm : BaseDisposable
    {
        public struct Ctx
        {
            public IPoolManager poolManager;
            public IResourceLoader resourceLoader;
            public ZooSceneContextView zooSceneContextView;
            public TimeStream timeStream;
            public ReactiveDictionary<int, AnimalInfo> animals;
            public ReactiveEvent<int> showLabel;
        }

        private Ctx _ctx;
        private float _lastTimeSpawn;
        private int _indexator;

        public AnimalSpawnManagerPm(Ctx ctx)
        {
            _ctx = ctx;
            var gameSettings = _ctx.zooSceneContextView.gameSettings;
            
            var createAnimal = new ReactiveTrigger<CreateInfo>();
            AnimalsManagerPm.Ctx animalsManagerCtx = new AnimalsManagerPm.Ctx
            {
                camera = _ctx.zooSceneContextView.sceneCamera,
                poolManager = _ctx.poolManager,
                resourceLoader = _ctx.resourceLoader,
                placeForAnimal = _ctx.zooSceneContextView.PlaceForAnimals,
                gameSettings = gameSettings,
                timeStream = _ctx.timeStream,
                allAnimals = _ctx.animals,
                createAnimal = createAnimal,
                showLabel = _ctx.showLabel
            };
            AddDispose(new AnimalsManagerPm(animalsManagerCtx));

            AddDispose(_ctx.timeStream.SubscribeToStream(TimeStream.Streams.UPDATE, delta =>
            {
                if (_ctx.animals.Count >= gameSettings.MaxCountAnimals)
                    return;
                
                if (_lastTimeSpawn > Time.time - gameSettings.SpawnCooldown)
                    return;

                _lastTimeSpawn = Time.time;

                var indexSpawn = Random.Range(0, _ctx.zooSceneContextView.SpawnPositions.Count - 1);

                var values = typeof(AnimalNames).GetEnumValues();
                var randomIndexAnimal = Random.Range(0, values.Length);
                createAnimal.Notify(new CreateInfo()
                {
                    SpawnPosition = _ctx.zooSceneContextView.SpawnPositions[indexSpawn].position,
                    AnimalName = (AnimalNames)values.GetValue(randomIndexAnimal)
                });
            }));

        }
    }
}