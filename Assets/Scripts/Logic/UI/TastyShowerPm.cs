using System;
using System.Collections.Generic;
using Logic.Loaders.ResourcesLoader;
using Logic.Loaders.View;
using Logic.Scene.Animals.Snake;
using Tools.Extensions.Reactive;
using Tools.Framework;
using Tools.Pool;
using UniRx;
using UnityEngine;

namespace Logic.Scene
{
    public class TastyShowerPm : BaseDisposable
    {
        public struct Ctx
        {
            public ReactiveDictionary<int, AnimalInfo> allAnimals;
            public IResourceLoader resourceLoader;
            public IPoolManager poolManager;
            public IReadOnlyReactiveEvent<int> showLabel;
            public TimeStream timeStream;
            public Transform PlaceForAnimalUi;
            public Camera camera;
        }


        private readonly Ctx _ctx;
        private readonly Dictionary<int, TastyShowerInfo> _animalTasties;

        private GameObject _prefabDamage;

        private const string _prefabName = "Tasty";
        private float _showTime = 2f;

        public TastyShowerPm(Ctx ctx)
        {
            _ctx = ctx;
            _animalTasties = new Dictionary<int, TastyShowerInfo>();
            _ctx.resourceLoader.LoadPrefab(_prefabName, prefab =>
            {
                _prefabDamage = prefab;
                AddDispose(_ctx.showLabel.SubscribeWithSkip(ShowTastyInfo));
            });

            AddDispose(_ctx.allAnimals.ObserveRemove().Subscribe(deadAnimal =>
            {
                RemoveItem(deadAnimal.Value.Model.Id);
            }));

            var removeListId = new List<int>();
            AddDispose(_ctx.timeStream.SubscribeToStream(TimeStream.Streams.UPDATE, delta =>
            {
                removeListId.Clear();
                foreach (KeyValuePair<int, TastyShowerInfo> tastyShower in _animalTasties)
                {
                    Vector3 screenPos = _ctx.camera.WorldToScreenPoint(tastyShower.Value.target.position);
                    screenPos.z = 0;
                    tastyShower.Value.obj.transform.position = screenPos;
                    tastyShower.Value.timer -= delta;
                    if (tastyShower.Value.timer < 0)
                    {
                        tastyShower.Value.mainDispose?.Dispose();
                        tastyShower.Value.itemView.Text.gameObject.SetActive(false);
                        _ctx.poolManager.Return(_prefabDamage, tastyShower.Value.obj);
                        removeListId.Add(tastyShower.Key);
                    }
                }

                foreach (var id in removeListId)
                {
                    _animalTasties.Remove(id);
                }
            }));
        }

        private void RemoveItem(int Id)
        {
            if (_animalTasties.TryGetValue(Id, out var testyInfo))
            {
                testyInfo.mainDispose?.Dispose();
                testyInfo.itemView.Text.gameObject.SetActive(false);
                _ctx.poolManager.Return(_prefabDamage, testyInfo.obj);
                _animalTasties.Remove(Id);
            }
        }

        protected override void OnDispose()
        {
            foreach (KeyValuePair<int, TastyShowerInfo> tastyShower in _animalTasties)
            {
                tastyShower.Value.mainDispose?.Dispose();
                tastyShower.Value.itemView.Text.gameObject.SetActive(false);
                _ctx.poolManager.Return(_prefabDamage, tastyShower.Value.obj);
            }
            base.OnDispose();
        }

        private void ShowTastyInfo(int idAnimal)
        {
            RemoveItem(idAnimal);
            var animalView = _ctx.allAnimals[idAnimal].AnimalView.GetComponent<AnimalView>();
            Transform tastyInfoPoint = animalView.PointForUi;
            TastyShowerInfo item = CreateItem(tastyInfoPoint);
            _animalTasties.Add(idAnimal, item);
            item.show.Notify();
        }
        
        private TastyShowerInfo CreateItem(Transform animalUiPosition)
        {
            TastyShowerInfo tastyShowerInfo = new TastyShowerInfo();
            CompositeDisposable viewDisposable = AddDispose(new CompositeDisposable());
            GameObject obj = _ctx.poolManager.Get(_prefabDamage);
            obj.transform.SetParent(_ctx.PlaceForAnimalUi, false);
            tastyShowerInfo.itemView = obj.GetComponent<TastyView>();
            tastyShowerInfo.show = new ReactiveTrigger();
            tastyShowerInfo.obj = obj;
            tastyShowerInfo.mainDispose = viewDisposable;
            tastyShowerInfo.target = animalUiPosition;
            tastyShowerInfo.timer = _showTime;

            tastyShowerInfo.show.Subscribe(() => { tastyShowerInfo.itemView.Text.gameObject.SetActive(true); })
                .AddTo(viewDisposable);

            return tastyShowerInfo;
        }

        private class TastyShowerInfo
        {
            public TastyView itemView;
            public GameObject obj;
            public float timer;
            public CompositeDisposable mainDispose;
            public ReactiveTrigger show;
            public Transform target;
        }
    }
}