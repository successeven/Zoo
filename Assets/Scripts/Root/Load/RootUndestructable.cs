using Containers;
using Tools.Extensions.Reactive;
using Tools.Framework;
using UniRx;
using UnityEngine;

namespace Logic
{
    public class RootUndestructable : BaseMonoBehaviour
	{
		public struct Ctx
		{
		}

		[SerializeField]
		private GameObject debugObjects;

		private Ctx _ctx;

		public void SetCtx(Ctx ctx)
		{
			_ctx = ctx;
			// дефолтные параметры тут

			ReactiveEvent<LoadSt> reloadEvent = new ReactiveEvent<LoadSt>();
			
			DebugView debugView = debugObjects.GetComponent<DebugView>();
			debugView.SetCtx(new DebugView.Ctx
			{
				reloadEvent = reloadEvent,
			});

			// start root
			Root.Ctx rootCtx = new Root.Ctx
			{
				reloadEvent = reloadEvent,
			};
			Root root = new Root(rootCtx);
			root.AddTo(this);
			Observable.OnceApplicationQuit().Subscribe(_ => root?.Dispose()).AddTo(this);
		}
	}
}