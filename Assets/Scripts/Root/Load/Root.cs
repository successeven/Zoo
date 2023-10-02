using System;
using Containers;
using Logic.Loaders;
using Logic.Loaders.ResourcesLoader;
using Tools.Extensions.Reactive;
using Tools.Framework;
using UniRx;
using UnityEngine;

namespace Logic
{
    public class Root : BaseDisposable
    {
        public struct Ctx
        {
            public ReactiveEvent<LoadSt> reloadEvent;
        }

        private readonly Ctx _ctx;
        private IResourceLoader _resourceLoader;
        private ISceneLoader _sceneLoader;
        private IDisposable _gameCore;

        public Root(Ctx ctx)
        {
            _ctx = ctx;

            SceneReloader.Ctx reloaderCtx = new SceneReloader.Ctx();
            SceneReloader sceneReloader = new SceneReloader(reloaderCtx);
            AddDispose(sceneReloader);

            LoadersCreator.Ctx resCtx = new LoadersCreator.Ctx
            {
                onCreate = (resLoader, sceneLoader) =>
                {
                    _resourceLoader = resLoader;
                    _sceneLoader = sceneLoader;
                    StartObserve();
                }
            };
            LoadersCreator loadersCreator = new LoadersCreator(resCtx);
            AddDispose(loadersCreator);

            void StartObserve()
            {
                AddDispose(_ctx.reloadEvent.SubscribeWithSkip(ReStart));
            }

            // create game cycle
            void ReStart(LoadSt loadSt)
            {
                _gameCore?.Dispose();
                sceneReloader.ReloadFirstScene(() =>
                {
                    CorePm.Ctx coreCtx = new CorePm.Ctx
                    {
                        loadSt = loadSt,
                        resourceLoader = _resourceLoader,
                        sceneLoader = _sceneLoader,
                    };
                    _gameCore = new CorePm(coreCtx);
                });
            }
        }

        protected override void OnDispose()
        {
            _gameCore?.Dispose();
            base.OnDispose();
        }
    }
}