using System;
using System.Collections.Generic;
using Logic.Loaders.ResourcesLoader.Configs;
using Tools.Extensions.Reactive;
using Tools.Framework;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Logic.Loaders.ResourcesLoader
{
    public class ResourcePreLoader: BaseDisposable, IResourceLoader
  {
    public struct Ctx
    {
      public DDebug parentLog;
      public float minLoadDelay;
      public float maxLoadDelay;
    }

    private readonly Ctx _ctx;
    private readonly ResourceConfigMain _configMain;
    private readonly Dictionary<string, GameObject> _prefabsToLoadCache;
    private readonly Dictionary<string, Sprite> _spritesToLoadCache;
    private readonly HashSet<string> _cacheImitator;

    private const string PRELOADED_FILES_PATH = "fakebundles/ResourceConfigMain";

    public ResourcePreLoader(Ctx ctx)
    {
      _ctx = ctx;
      _prefabsToLoadCache = new Dictionary<string, GameObject>();
      _spritesToLoadCache = new Dictionary<string, Sprite>();
      _configMain = Resources.Load<ResourceConfigMain>(PRELOADED_FILES_PATH);
      // sprites
      foreach (ResourceConfigSprite content in _configMain.spriteConfigs)
      {
        if (content == null)
          continue;
        FillDictionaryFromArray(_spritesToLoadCache, content.sprites);
      }
      // prefabs
      foreach (ResourceConfig content in _configMain.contentConfigs)
      {
        if (content == null)
          continue;
        FillDictionaryFromArray(_prefabsToLoadCache, content.prefabs);
      }
      _cacheImitator = new HashSet<string>();
    }
    
    public void LoadPrefab(string prefabName, Action<GameObject> onComplete)
    {
      GameObject prefab = GetResource(_prefabsToLoadCache, prefabName);
      onComplete?.Invoke(prefab);
    }

    public void LoadSprite(string spriteName, Action<Sprite> onComplete)
    {
      Sprite sprite = GetResource(_spritesToLoadCache, spriteName);
      onComplete?.Invoke(sprite);
    }

    protected override void OnDispose()
    {
      _prefabsToLoadCache.Clear();
      _spritesToLoadCache.Clear();
      _cacheImitator.Clear();
      Resources.UnloadAsset(_configMain);
    }

    private T GetResource<T>(IReadOnlyDictionary<string, T> map, string resourceName) where T : Object
    {
      if (map == null)
      {
        log.Err($"Can't get resource from nullable map by '{resourceName}'");
        return null;
      }
      string key = resourceName;
      if (!map.TryGetValue(key, out T ret))
      {
        log.Err($"Can't get resource '{resourceName}'");
        return null;
      }
      return ret;
    }

    private void FillDictionaryFromArray<T>(IDictionary<string, T> dict, IReadOnlyList<T> gameObjects) where T : Object
    {
      if (gameObjects == null || gameObjects.Count <= 0)
        return;
      for (int i = 0, ik = gameObjects.Count; i < ik; ++i)
      {
        T gameObj = gameObjects[i];
        if (gameObj != null)
        {
          if (dict.ContainsKey(gameObj.name))
            log.Info($"duplicating '{gameObj.name}' in content");
          dict[gameObj.name] = gameObj;
        }
      }
    }
  }
}