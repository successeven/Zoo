using System;
using System.Collections.Generic;
using Tools.Extensions.Reactive;
using Tools.Framework;
using UniRx;
using UnityEngine;

namespace Logic.Scene
{
    public class TimeStream: BaseDisposable
	{
		public enum Streams
		{
			UPDATE,
			PHYSICS,
			LATEUPDATE,
		}

		public struct Ctx
		{
			public float serverTickRate;
		}

		private readonly Ctx _ctx;
		private readonly Dictionary<Streams, ReactiveEvent<float>> _triggers;
		private readonly Dictionary<Streams, IDisposable> _streams;
		private bool _rateIsPause;
		private bool _updateIsPause;
		private bool _physicsIsPause;
		private bool _lateUpdateIsPause;
		private bool _uiUpdateIsPause;

		public TimeStream(Ctx ctx)
		{
			_ctx = ctx;
			_triggers = new Dictionary<Streams, ReactiveEvent<float>>();
			_streams = new Dictionary<Streams, IDisposable>();
			SetTicksPerSecond(Streams.PHYSICS, _ctx.serverTickRate);
			SetTicksPerSecond(Streams.UPDATE, _ctx.serverTickRate);
			SetTicksPerSecond(Streams.LATEUPDATE, _ctx.serverTickRate);
		}

		public IDisposable SubscribeToStream(Streams stream, Action<float> callback)
		{
			return GetTrigger(stream)?.Subscribe(callback);
		}

		public void Pause(Streams streamToPause)
		{
			switch (streamToPause)
			{
				case Streams.UPDATE:
					_updateIsPause = true;
					break;
				case Streams.PHYSICS:
					_physicsIsPause = true;
					break;
				case Streams.LATEUPDATE:
					_lateUpdateIsPause = true;
					break;
				default: throw new ArgumentOutOfRangeException(nameof(streamToPause), streamToPause, null);
			}
		}

		public void Play(Streams streamToPlay)
		{
			switch (streamToPlay)
			{
				case Streams.UPDATE:
					_updateIsPause = false;
					break;
				case Streams.PHYSICS:
					_physicsIsPause = false;
					break;
				case Streams.LATEUPDATE:
					_lateUpdateIsPause = false;
					break;
				default: throw new ArgumentOutOfRangeException(nameof(streamToPlay), streamToPlay, null);
			}
		}
		
		protected override void OnDispose()
		{
			foreach (KeyValuePair<Streams, IDisposable> pair in _streams)
			{
				pair.Value?.Dispose();
			}
			base.OnDispose();
		}

		private void SetTicksPerSecond(Streams stream, float tps)
		{
			// dispose prev
			if (_streams.TryGetValue(stream, out IDisposable curStream))
				curStream?.Dispose();
			
			if (stream == Streams.PHYSICS)
			{
				_streams[stream] = ReactiveExtensions.StartFixedUpdate( () =>
				{
					UpdateTick(Streams.PHYSICS, Time.fixedDeltaTime);
				});
			}
			else if (stream == Streams.UPDATE)
			{
				_streams[stream] = ReactiveExtensions.StartUpdate( () =>
				{
					UpdateTick(Streams.UPDATE, Time.deltaTime);
				});
			}
			else if (stream == Streams.LATEUPDATE)
			{
				_streams[stream] = ReactiveExtensions.StartLateUpdate( () =>
				{
					UpdateTick(Streams.LATEUPDATE, Time.deltaTime);
				});
			}
			else
			{
				float interval = Math.Min(1 / tps, 0.001f);
				_streams[stream] = Observable.Interval(TimeSpan.FromSeconds(interval)).Subscribe(_ => UpdateTick(stream, interval));
			}
		}

		private void UpdateTick(Streams stream, float deltaTime)
		{
			switch (stream)
			{
				case Streams.UPDATE:
					if (_updateIsPause) return;
					break;
				case Streams.PHYSICS:
					if (_physicsIsPause) return;
					break;
				case Streams.LATEUPDATE:
					if (_lateUpdateIsPause) return;
					break;
			}
			
			GetTrigger(stream)?.Notify(deltaTime);
		}

		private ReactiveEvent<float> GetTrigger(Streams stream)
		{
			if (_triggers.TryGetValue(stream, out ReactiveEvent<float> trigger))
				return trigger;
			trigger = new ReactiveEvent<float>();
			_triggers[stream] = trigger;
			return trigger;
		}
	}
}