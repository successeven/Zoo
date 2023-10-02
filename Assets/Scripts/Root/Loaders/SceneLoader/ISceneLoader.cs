using System;

namespace Logic.Loaders
{
    public interface ISceneLoader: IDisposable
    {
        void LoadScene(string sceneName, Action onUnload, Action onComplete);
    }
}