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
            public ReactiveEvent<int> showLabel;
        }

        private Ctx _ctx;
        private Dictionary<AnimalNames, GameObject> _animalPrefs;
        private int _indexator;
        private ReactiveTrigger<int> _death;

        public AnimalsManagerPm(Ctx ctx)
        {
            _ctx = ctx;
            _death = new ReactiveTrigger<int>();
            _animalPrefs = new Dictionary<AnimalNames, GameObject>();
            AddDispose(_death.Subscribe(Id =>
            {
                if (!_ctx.allAnimals.TryGetValue(Id, out var animalInfo))
                {
                    Debug.Log($"Dont find animal with id {Id}");
                    return;
                }
                
                animalInfo.Logic?.Dispose();
                if (_animalPrefs.TryGetValue(animalInfo.Model.AnimalName, out var animalPref))
                    _ctx.poolManager.Return(animalPref, animalInfo.AnimalView);
                _ctx.allAnimals.Remove(Id);
            }));
            
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

        protected override void OnDispose()
        {
            foreach (var animal in _ctx.allAnimals)
            {
                animal.Value.Logic?.Dispose();
                if (_animalPrefs.TryGetValue(animal.Value.Model.AnimalName, out var animalPref))
                    _ctx.poolManager.Return(animalPref, animal.Value.AnimalView);
            }
        }


        private void CreateSnake(GameObject view )
        {
            var model = new BaseAnimalModel()
            {
                Id = _indexator++,
                AnimalType = AnimalType.Predator,
                AnimalName = AnimalNames.Snake,
                Speed = { Value = _ctx.gameSettings.SnakeInfo.Speed },
            };
            SnakePm.Ctx snakeCtx = new SnakePm.Ctx
            {
                timeStream = _ctx.timeStream,
                camera = _ctx.camera,
                model = model,
                view = view,
                death = _death,
                showLabel = _ctx.showLabel
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
                AnimalType = AnimalType.Prey,
                AnimalName = AnimalNames.Frog,
                Speed = { Value = _ctx.gameSettings.FrogInfo.Speed },
                JumpHeight = { Value = _ctx.gameSettings.FrogInfo.heightJump},
                JumpCooldown = { Value = _ctx.gameSettings.FrogInfo.CooldownJump}
            };
            FrogPm.Ctx ctx = new FrogPm.Ctx
            {
                timeStream = _ctx.timeStream,
                camera = _ctx.camera,
                model = model,
                view = view,
                death = _death,
                showLabel = _ctx.showLabel
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