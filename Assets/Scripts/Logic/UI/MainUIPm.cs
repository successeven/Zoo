using Logic.Loaders.ResourcesLoader;
using Logic.Loaders.View;
using Logic.Scene.Animals.Core;
using Tools.Framework;
using Tools.Pool;
using UniRx;
using UnityEngine;

namespace Logic.Scene
{
    public class MainUIPm : BaseDisposable
    {
        public struct Ctx
        {
            public ReactiveDictionary<int, AnimalInfo> allAnimals;
            public IResourceLoader resourceLoader;
            public Transform PlaceForAllUi;
        }

        private readonly Ctx _ctx;
        private const string _prefabName = "MainUi";
        private MainUiView _view;
        private int _preyDied;
        private int _predatorDied;
        
        public MainUIPm(Ctx ctx)
        {
            _ctx = ctx;
            _ctx.resourceLoader.LoadPrefab(_prefabName, prefab =>
            {
                
                GameObject objView = AddComponent(Object.Instantiate(prefab, _ctx.PlaceForAllUi, false));
                _view = objView.GetComponent<MainUiView>();

                AddDispose(_ctx.allAnimals.ObserveRemove().Subscribe(deadInfo =>
                {
                    if (deadInfo.Value.Model.AnimalType == AnimalType.Predator)
                    {
                        _predatorDied++;
                        _view.PredatorLabel.text = $"Predator died: {_predatorDied}";
                    }
                    else 
                    if (deadInfo.Value.Model.AnimalType == AnimalType.Prey)
                    {
                        _preyDied++;
                        _view.PreyLabel.text = $"Prey died: {_preyDied}";
                    }
                }));
            });
        }
    }
}