using Tools.Framework;
using UnityEngine;

namespace Logic.Loaders.View
{
    public class SceneContextView: BaseMonoBehaviour
    {
        [SerializeField]
        private Canvas mainCanvas;
        [SerializeField]
        private Transform uiParent;

        public Transform UiParent
            => uiParent;
        public Canvas MainCanvas
            => mainCanvas;
    }
}