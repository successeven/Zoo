using System.Collections.Generic;
using Tools.Framework;
using Tools.Pool;
using UnityEngine;

namespace Pool
{
  public class PoolManager : BaseDisposable, IPoolManager
  {
    public struct Ctx
    {
      public string name;
      public bool dontDestroyPool;
    }

    private readonly Ctx _ctx;
    private readonly Transform _poolRoot;
    // prefab / pool
    private readonly Dictionary<GameObject, (TransformPool, Transform)> _pools;

    public PoolManager(Ctx ctx)
    {
      _ctx = ctx;
      GameObject poolObj = AddComponent(new GameObject($"Pool of {_ctx.name}"));
      if (_ctx.dontDestroyPool)
        Object.DontDestroyOnLoad(poolObj);
      _poolRoot = poolObj.transform;
      _pools = new Dictionary<GameObject, (TransformPool, Transform)>();
    }

    public TransformPool CreateOrGetPool(GameObject prefab, int preloadCount = 0)
    {
      if (_pools.TryGetValue(prefab, out (TransformPool, Transform) existedPool))
        return existedPool.Item1;
      GameObject poolObj = AddComponent(new GameObject($"pool [{prefab.name}]"));
      poolObj.transform.SetParent(_poolRoot);
      BasePool<Transform>.BaseCtx poolBaseCtx = new BasePool<Transform>.BaseCtx
      {
        prefab = prefab.transform,
        preloadCount = preloadCount,
        parent = poolObj.transform
      };
      TransformPool createdPool = new TransformPool(new TransformPool.Ctx(), poolBaseCtx);
      _pools.Add(prefab, (createdPool, poolObj.transform));
      return createdPool;
    }

    public GameObject Get(GameObject prefab)
    {
      if (isDisposed)
        return null;
      TransformPool pool = CreateOrGetPool(prefab);
      return pool.Rent().gameObject;
    }

    public void Return(GameObject prefab, GameObject obj)
    {
      if (isDisposed)
        return;
      TransformPool pool = CreateOrGetPool(prefab);
      pool?.Return(obj.transform);
    }

    protected override void OnDispose()
    {
      foreach (KeyValuePair<GameObject, (TransformPool, Transform)> poolPair in _pools)
      {
        poolPair.Value.Item1?.Dispose();
      }
      base.OnDispose();
    }
  }
}