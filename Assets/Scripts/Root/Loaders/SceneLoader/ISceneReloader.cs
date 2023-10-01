using System;

namespace Logic.Loaders
{
    public interface ISceneReloader
    {
        void ReloadFirstScene(Action onComplete);
    }
}