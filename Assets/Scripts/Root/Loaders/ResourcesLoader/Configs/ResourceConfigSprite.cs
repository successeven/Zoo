using UnityEngine;

namespace Logic.Loaders.ResourcesLoader.Configs
{
  [CreateAssetMenu(fileName = "ResourceConfigSprite.asset", menuName = "Zoo/Create ResourceConfigSprite")]
  public class ResourceConfigSprite : ScriptableObject
  {
    public Sprite[] sprites;
    public Sprite[] atlases;
  }
}