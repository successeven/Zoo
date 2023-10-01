using UnityEngine;

namespace Logic.Loaders.ResourcesLoader.Configs
{
  [CreateAssetMenu(fileName = "ResourceConfig.asset", menuName = "Zoo/Create ResourceConfig")]
  public class ResourceConfig : ScriptableObject
  {
    public GameObject[] prefabs;
  }
}