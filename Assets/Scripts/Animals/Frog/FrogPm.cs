using System.Collections.Generic;
using Logic.Loaders.View;
using Logic.Scene;
using Logic.Scene.Animals.Core;
using Logic.Scene.Animals.Snake;
using Tools;
using Tools.Extensions.Reactive;
using UniRx;
using UnityEngine;

namespace Animals.Frog
{
    public class FrogPm : Animal
    {
        public struct Ctx
        {
            public TimeStream timeStream;
            public Camera camera;
            public GameObject view;
            public FrogModel model;
            public ReactiveTrigger<int> death;
            public ReactiveEvent<int> showLabel;
        }

        private Ctx _ctx;

        public FrogPm(Ctx ctx) : base(new AnimalCtx
        {
            model = ctx.model,
            death = ctx.death,
            showLabel = ctx.showLabel,
            camera = ctx.camera,
            timeStream = ctx.timeStream,
            view = ctx.view
        })
        {
            _ctx = ctx;
            _view = _ctx.view.GetComponent<AnimalView>();
            var reflectVelocity = new ReactiveTrigger<Vector3>();

            AddDispose(_changeDirectionTrigger.Subscribe(newWay =>
            {
                switch (newWay)
                {
                    case ChangeDirectionInfo.NewWay:
                        _currentDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
                        _view.Rigidbody.velocity =  _ctx.model.JumpHeight.Value * Vector3.up + _currentDirection * _ctx.model.Speed.Value;
                        break;
                    case ChangeDirectionInfo.ReverseZ:
                        _currentDirection.z *= -1;
                        break;
                    case ChangeDirectionInfo.ReverseX:
                        _currentDirection.x *= -1;
                        break;
                    case ChangeDirectionInfo.ReverseBoth:
                        _currentDirection.x *= -1;
                        _currentDirection.z *= -1;
                        break;
                }
            }));

            _view.SetCtx(new AnimalView.Ctx
            {
                tryEat = _tryEat,
                changeDirection = _changeDirectionTrigger,
                model = _ctx.model,
                reflectVelocity = reflectVelocity
            });

            AddDispose(reflectVelocity.Subscribe(normalVector =>
            {
                _currentDirection = Vector3.Reflect(_currentDirection, normalVector);
            }));
            
            _isInit = true;
        }

        protected override void Move(float deltaTime)
        {
            _view.Rigidbody.MovePosition(_view.Rigidbody.position + 
                                         _ctx.model.Speed.Value * deltaTime * _currentDirection);
            
            var isGround = Physics.Raycast(_view.Rigidbody.position, Vector3.down, .15f, SceneLayers.FloorMask);
            if (isGround)
            {
                _changeDirectionTrigger.Notify(ChangeDirectionInfo.NewWay);
            }
        }
    }
}