

using System.Collections.Generic;
using Logic.Settings;
using UnityEngine;

namespace Logic.Loaders.View
{
    public class ZooSceneContextView : SceneContextView
    {
        public Camera camera;
        public Transform PlaceForAnimals;
        public List<Transform> SpawnPositions;
        public GameSettings gameSettings;
    }
}