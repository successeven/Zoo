using System;
using Logic.Loaders.ResourcesLoader;
using Logic.Loaders.View;
using Logic.Scene.Animals.Core;
using Tools.Extensions.Reactive;
using Tools.Framework;
using Tools.Pool;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Logic.Scene.Animals.Snake
{
    public class SnakePm : Animal
    {
        public struct Ctx
        {
            public TimeStream timeStream;
            public Camera camera;
            public GameObject view;
            public BaseAnimalModel model;
            public ReactiveTrigger<int> death;
            public ReactiveEvent<int> showLabel;
        }

        private Ctx _ctx;
        private AnimalView _view;
        private Vector3 _currentDirection;
        
        private float _checkDistance = 2;
        
        public SnakePm(Ctx ctx) : base(new AnimalCtx
        {
            model = ctx.model,
            death = ctx.death,
            showLabel = ctx.showLabel
        })
        {
            _ctx = ctx;
            var reflectVelocity = new ReactiveTrigger<Vector3>();
            _view = _ctx.view.GetComponent<AnimalView>();
            var changeDirectionTrigger = new ReactiveEvent<ChangeDirectionInfo>();
            
            AddDispose(changeDirectionTrigger.Subscribe(newWay =>
            {
                switch (newWay)
                {
                    case ChangeDirectionInfo.NewWay:
                        _currentDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
                        break;
                    case ChangeDirectionInfo.ReverseZ:
                        _currentDirection.z *= -1;
                        break;
                    case ChangeDirectionInfo.ReverseX:
                        _currentDirection.x *= -1;
                        break;
                }
            }));
            
            _view.SetCtx(new AnimalView.Ctx
            {
                tryEat = _tryEat,
                changeDirection = changeDirectionTrigger,
                model = _ctx.model,
                reflectVelocity = reflectVelocity
            });

            AddDispose(reflectVelocity.Subscribe(normalVector =>
            {
                _currentDirection = Vector3.Reflect(_currentDirection, normalVector);
            }));
            
            AddDispose(_ctx.timeStream.SubscribeToStream(TimeStream.Streams.UPDATE, _ =>
            {
                Vector3 screenPoint = _ctx.camera.WorldToViewportPoint(_view.transform.position + 
                                                                       _currentDirection * _checkDistance);

                bool onScreen = screenPoint is { z: > 0, x: > 0 and < 1, y: > 0 and < 1 };
                if (!onScreen)
                    changeDirectionTrigger.Notify(ChangeDirectionInfo.NewWay);
            }));
            
            AddDispose(_ctx.timeStream.SubscribeToStream(TimeStream.Streams.UPDATE, Move));
        }

        private void Move(float deltaTime)
        {
            _view.Rigidbody.MovePosition(_view.Rigidbody.position + 
                                         _ctx.model.Speed.Value * deltaTime * _currentDirection);
        }
    }
}