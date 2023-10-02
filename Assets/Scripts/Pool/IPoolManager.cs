using System;
using UnityEngine;

namespace Tools.Pool
{
  public interface IPoolManager : IDisposable
  {
    TransformPool CreateOrGetPool(GameObject prefab, int preloadCount = 1);
    GameObject Get(GameObject prefab);
    void Return(GameObject prefab, GameObject obj);
  }
}