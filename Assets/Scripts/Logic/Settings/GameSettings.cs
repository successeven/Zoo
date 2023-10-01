using Animals.Frog;
using Logic.Scene.Animals.Snake;
using UnityEngine;

namespace Logic.Settings
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Zoo/Settings/Create game settings")]
    public class GameSettings : ScriptableObject
    {
        public int MaxCountAnimals;
        public float SpawnCooldown;
        public SnakeInfo SnakeInfo;
        public FrogInfo FrogInfo;
    }
}