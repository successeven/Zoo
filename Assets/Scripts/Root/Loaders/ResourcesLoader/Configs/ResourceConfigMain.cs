using UnityEngine;

namespace Logic.Loaders.ResourcesLoader.Configs
{
  [CreateAssetMenu(fileName = "ResourceConfigMain.asset", menuName = "Zoo/Create ResourceConfigMain")]
  public class ResourceConfigMain : ScriptableObject
  {
    public ResourceConfig[] contentConfigs;
    public ResourceConfigSprite[] spriteConfigs;
  }
}