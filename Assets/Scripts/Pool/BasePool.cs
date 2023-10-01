using System;
using UniRx;
using UniRx.Toolkit;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tools.Pool
{
  public abstract class BasePool<T> : ObjectPool<T> where T : Component
  {
    public struct BaseCtx
    {
      public T prefab;
      public Transform parent;
      public int preloadCount;
    }

    protected readonly BaseCtx baseCtx;
    private readonly IDisposable _preloader;

    protected BasePool(BaseCtx baseCtx)
    {
      this.baseCtx = baseCtx;
      _preloader = PreloadAsync(baseCtx.preloadCount, 1).Subscribe(_ => { });
    }

    protected override void Dispose(bool disposing)
    {
      _preloader?.Dispose();
      base.Dispose(disposing);
    }

    protected override T CreateInstance()
    {
      T obj = Object.Instantiate(baseCtx.prefab, baseCtx.parent, false);
      return obj;
    }

    protected override void OnBeforeReturn(T instance)
    {
      instance.gameObject.SetActive(false);
      instance.transform.SetParent(baseCtx.parent, false);
      base.OnBeforeReturn(instance);
    }

    protected override void OnBeforeRent(T instance)
    {
      // do
      base.OnBeforeRent(instance);
    }
  }
}