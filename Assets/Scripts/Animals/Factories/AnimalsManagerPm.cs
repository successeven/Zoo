using System;
using System.Collections.Generic;
using Animals.Frog;
using Logic.Loaders.ResourcesLoader;
using Logic.Loaders.View;
using Logic.Scene;
using Logic.Scene.Animals.Core;
using Logic.Scene.Animals.Snake;
using Logic.Settings;
using Tools.Extensions.Reactive;
using Tools.Framework;
using Tools.Pool;
using UniRx;
using UnityEngine;

namespace Animals.Factories
{
    public class AnimalsManagerPm : BaseDisposable
    {
        public struct Ctx
        {
            public IPoolManager poolManager;
            public IResourceLoader resourceLoader;
            public TimeStream timeStream;
            public Camera camera;
            public IReadOnlyReactiveTrigger<CreateInfo> createAnimal;
            public Transform placeForAnimal;
            public ReactiveDictionary<int, AnimalInfo> allAnimals;
            public GameSettings gameSettings;
            public ReactiveEvent<EatInfo> tryEat;
        }

        private Ctx _ctx;
        private Dictionary<AnimalNames, GameObject> _animalPrefs;
        private int _indexator;

        public AnimalsManagerPm(Ctx ctx)
        {
            _ctx = ctx;
            _animalPrefs = new Dictionary<AnimalNames, GameObject>();
            AddDispose(_ctx.createAnimal.Subscribe(createInfo =>
            {
                GameObject view = LoadAndInitViewAnimal(createInfo);
                switch (createInfo.AnimalName)
                {
                    case AnimalNames.Frog:
                        CreateFrog(view);
                        break;
                    case AnimalNames.Snake:
                       CreateSnake(view);
                        break;
                }
            }));
            
        }
        
        

        private void CreateSnake(GameObject view )
        {
            var model = new BaseAnimalModel()
            {
                Id = _indexator++,
                AnimalType = AnimalType.Predator,
                Speed = { Value = _ctx.gameSettings.SnakeInfo.Speed },
            };
            SnakePm.Ctx snakeCtx = new SnakePm.Ctx
            {
                timeStream = _ctx.timeStream,
                camera = _ctx.camera,
                model = model,
                view = view,
                tryEat = _ctx.tryEat
            };
            var snake = new SnakePm(snakeCtx);
            _ctx.allAnimals.Add(model.Id, new AnimalInfo
            {
                Logic = snake,
                Model = model,
                AnimalView = view
            });
        }

        private void CreateFrog(GameObject view )
        {
            var model = new FrogModel()
            {
                Id = _indexator++,
                AnimalType = AnimalType.Predator,
                Speed = { Value = _ctx.gameSettings.FrogInfo.Speed },
                JumpDistance = { Value = _ctx.gameSettings.FrogInfo.distanceJump},
                JumpHeight = { Value = _ctx.gameSettings.FrogInfo.heightJump},
                JumpCooldown = { Value = _ctx.gameSettings.FrogInfo.CooldownJump}
            };
            FrogPm.Ctx ctx = new FrogPm.Ctx
            {
                timeStream = _ctx.timeStream,
                camera = _ctx.camera,
                model = model,
                view = view,
                tryEat = _ctx.tryEat
            };
            var frog = new FrogPm(ctx);
            _ctx.allAnimals.Add(model.Id, new AnimalInfo
            {
                Logic = frog,
                Model = model,
                AnimalView = view
            });
        }

        private GameObject LoadAndInitViewAnimal(CreateInfo createInfo)
        {
            GameObject view = null;
            if (_animalPrefs.TryGetValue(createInfo.AnimalName, out var prefab))
            {
                view = _ctx.poolManager.Get(prefab);
            }
            else
            {
                _ctx.resourceLoader.LoadPrefab(createInfo.AnimalName.ToString(), prefab =>
                {
                    _animalPrefs.Add(createInfo.AnimalName, prefab);
                    view = _ctx.poolManager.Get(prefab);
                });
            }
            
            view.transform.SetParent(_ctx.placeForAnimal);
            view.transform.localPosition = Vector3.zero;
            view.transform.position = createInfo.SpawnPosition;
            return view;
        }
    }
}