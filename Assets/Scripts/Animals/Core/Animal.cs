using Logic.Loaders.View;
using Logic.Scene.Animals.Snake;
using Tools.Extensions.Reactive;
using Tools.Framework;
using UniRx;
using UnityEngine;

namespace Logic.Scene.Animals.Core
{
    public abstract class Animal: BaseDisposable
    {
        public struct AnimalCtx
        {
            public Camera camera;
            public BaseAnimalModel model;
            public ReactiveTrigger<int> death;
            public ReactiveEvent<int> showLabel;
            public TimeStream timeStream;
            public GameObject view;
        }
        
        protected ReactiveEvent<BaseAnimalModel> _tryEat;
        protected AnimalView _view;
        private AnimalCtx _ctx;
        protected ReactiveEvent<ChangeDirectionInfo> _changeDirectionTrigger;
        protected Vector3 _currentDirection;
        protected bool _isInit;
        
        private float _checkDistance = 2;
        protected Animal(AnimalCtx ctx)
        {
            _ctx = ctx;
            _tryEat = new ReactiveEvent<BaseAnimalModel>();
            _changeDirectionTrigger = new ReactiveEvent<ChangeDirectionInfo>();
            _view = _ctx.view.GetComponent<AnimalView>();
            AddDispose(_ctx.model.Alive.Subscribe(value =>
            {
                if (!value)
                {
                    Debug.Log($"{_ctx.model.AnimalName} dead");
                    _ctx.death.Notify(_ctx.model.Id);
                }
            }));
            AddDispose(_tryEat.SubscribeWithSkip(defenderModel =>
            {
                if (!_ctx.model.Alive.Value)
                    return;
                
                if (_ctx.model.AnimalType == AnimalType.Prey)
                    return;
                
                defenderModel.Alive.Value = false;
                _ctx.showLabel.Notify(_ctx.model.Id);
            }));
            
            AddDispose(_ctx.timeStream.SubscribeToStream(TimeStream.Streams.UPDATE, delta =>
            {
                if(!_isInit)
                    return;
                CheckScreenPos();
                Move(delta);
            }));
            
        }
        private void CheckScreenPos()
        {
            Vector3 screenPoint = _ctx.camera.WorldToViewportPoint(_view.Rigidbody.position +_currentDirection * _checkDistance);
        
            bool onScreen = screenPoint is { z: > 0, x: > 0 and < 1, y: > 0 and < 1 };
            if (!onScreen)
                _changeDirectionTrigger.Notify(ChangeDirectionInfo.ReverseBoth);
        }


        protected virtual void Move(float deltaTime)
        {
            
        }
    }
}