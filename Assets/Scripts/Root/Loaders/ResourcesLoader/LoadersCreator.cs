using System;
using Tools.Framework;

namespace Logic.Loaders.ResourcesLoader
{
    public class LoadersCreator : BaseDisposable
    {
        public struct Ctx
        {
            public Action<IResourceLoader, ISceneLoader> onCreate;
        }

        private readonly Ctx _ctx;
        private IResourceLoader _resourceLoader;

        public LoadersCreator(Ctx ctx)
        {
            _ctx = ctx;
            CreateResourceLoader(() =>
            {
                SceneLoader.SceneLoader.Ctx sceneLoadCtx = new SceneLoader.SceneLoader.Ctx
                {
                    resourceLoader = _resourceLoader
                };
                SceneLoader.SceneLoader sceneLoader = new SceneLoader.SceneLoader(sceneLoadCtx);
                AddDispose(sceneLoader);

                _ctx.onCreate?.Invoke(_resourceLoader, sceneLoader);
            });
        }

        private void CreateResourceLoader(Action created)
        {
            ResourcePreLoader.Ctx previewCtx = new ResourcePreLoader.Ctx
            {
                parentLog = log,
                maxLoadDelay = 0f,
                minLoadDelay = 0f
            };
            _resourceLoader = new ResourcePreLoader(previewCtx);
            created?.Invoke();
        }
    }
}