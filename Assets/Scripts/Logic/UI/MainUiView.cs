using TMPro;
using Tools.Framework;
using UnityEngine;

namespace Logic.Scene
{
    public class MainUiView : BaseMonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _predatorLabel;
        [SerializeField] private TextMeshProUGUI _preyLabel;

        public TextMeshProUGUI PredatorLabel => _predatorLabel;

        public TextMeshProUGUI PreyLabel => _preyLabel;
    }
}