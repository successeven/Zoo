using TMPro;
using Tools.Extensions.Reactive;
using Tools.Framework;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Logic.Scene
{
    public class TastyView : BaseMonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI _Text;

		public TextMeshProUGUI Text => _Text;


	}
}