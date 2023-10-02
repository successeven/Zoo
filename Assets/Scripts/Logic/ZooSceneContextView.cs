

using System.Collections.Generic;
using Logic.Settings;
using UnityEngine;
using UnityEngine.Serialization;

namespace Logic.Loaders.View
{
    public class ZooSceneContextView : SceneContextView
    {
        public Camera sceneCamera;
        public Transform PlaceForAnimals;
        public List<Transform> SpawnPositions;
        public GameSettings gameSettings;
        public Transform PlaceForAnimalUi;
    }
}