using System;
using UniRx;

// ReSharper disable All

namespace Tools.Extensions.Reactive
{
  public static class ReactiveExtensions
  {
    public static IDisposable DelayedCall(float delaySec, Action action)
    {
      if (delaySec <= 0)
      {
        action?.Invoke();
        return null;
      }
      return Observable.Timer(TimeSpan.FromSeconds(delaySec)).Take(1).Subscribe(_ => action?.Invoke());
    }
    
    public static IDisposable DelayedCallTimeIgnoreTimeScale(float delaySec, Action action)
    {
      if (delaySec <= 0)
      {
        action?.Invoke();
        return null;
      }
      return Observable.Timer(TimeSpan.FromSeconds(delaySec), Scheduler.MainThreadIgnoreTimeScale).Take(1).Subscribe(_ => action?.Invoke());
    }

    public static IDisposable Timer(int sec, Action everyTick, Action finallyAction)
    {
      everyTick.Invoke();
      return Observable.Interval(TimeSpan.FromSeconds(1)).Take(sec).Finally(finallyAction).Subscribe(_ =>
      {
        everyTick.Invoke();
      });
    }

    public static IDisposable DelayedFinallyCall(float delaySec, Action action)
    {
      if (delaySec <= 0)
      {
        action?.Invoke();
        return null;
      }
      bool executed = false;
      return Observable.Timer(TimeSpan.FromSeconds(delaySec)).Take(1).Finally(() =>
      {
        if (executed)
          return;
        action?.Invoke();
      }).Subscribe(_ =>
      {
        executed = true;
        action?.Invoke();
      });
    }

    // for testing
    public static IDisposable StartUpdate(Action onIteration)
    {
      return Observable.EveryUpdate().Subscribe(_ => { onIteration?.Invoke(); });
    }
    // for testing
    public static IDisposable StartLateUpdate(Action onIteration)
    {
      return Observable.EveryLateUpdate().Subscribe(_ => { onIteration?.Invoke(); });
    }

    // for testing
    public static IDisposable StartFixedUpdate(Action onIteration)
    {
      return Observable.EveryFixedUpdate().Subscribe(_ => { onIteration?.Invoke(); });
    }

    public static IDisposable DelayFrame(int countFrame, Action action)
    {
      return Observable.TimerFrame(countFrame).Subscribe(_ => action?.Invoke());
    }

    public static IDisposable DoOnMainThread(Action action)
    {
      return Scheduler.MainThread.Schedule(() =>
      {
        action?.Invoke();
      });
    }
  }
}