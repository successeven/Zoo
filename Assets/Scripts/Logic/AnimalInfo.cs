using System;
using Logic.Scene.Animals.Core;
using Logic.Scene.Animals.Snake;
using UnityEngine;

namespace Logic.Loaders.View
{
    public struct AnimalInfo
    {
        public IDisposable Logic;
        public BaseAnimalModel Model;
        public GameObject AnimalView;
    }
}