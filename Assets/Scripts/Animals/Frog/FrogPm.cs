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
            public ReactiveEvent<EatInfo> tryEat;
            public Camera camera;
            public GameObject view;
            public FrogModel model;
        }

        private Ctx _ctx;
        private AnimalView _view;
        private Vector3 _currentDirection;
        private List<Vector3> _jumpPoints;
        private int _pointIndex;
        private ReactiveEvent<ChangeDirectionInfo> _changeDirectionTrigger;
        private ReactiveProperty<bool> _onGround;
        private float _timerOnGround;

        public FrogPm(Ctx ctx)
        {
            _ctx = ctx;
            _view = _ctx.view.GetComponent<AnimalView>();
            _jumpPoints = new List<Vector3>();
            _onGround = new ReactiveProperty<bool>();
            _changeDirectionTrigger = new ReactiveEvent<ChangeDirectionInfo>();

            AddDispose(_changeDirectionTrigger.Subscribe(_ =>
            {
                
                
                bool haveDirWall = true;
                while (haveDirWall)
                {
                    _currentDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
                    Collider[] walls = new Collider[2];
                    if (Physics.OverlapSphereNonAlloc(_view.Rigidbody.position, _view.Collider.radius + .5f, walls,
                            SceneLayers.WallMask) > 0)
                    {
                        for (int i = 0; i < walls.Length - 1; i++)
                        {
                            var wall = walls[i];
                            Vector3 closestPoint = Physics.ClosestPoint(_view.Rigidbody.position, wall,
                                wall.transform.position, wall.transform.rotation);
                            Vector3 dirToTarget = (closestPoint - _view.Rigidbody.position).normalized;
                            dirToTarget.y = 0;
                            var angle = Vector3.Angle(_currentDirection, dirToTarget);
                            log.Info(angle);
                            
                            Debug.DrawLine(_view.Rigidbody.position, dirToTarget * 2, Color.red, .2f);
                            if (angle > 90f)
                            {
                                haveDirWall = false;
                                break;
                            }
                        }
                    }
                    else
                        haveDirWall = false;
                }

                _pointIndex = 0;
                Bezier.GetCurve(ref _jumpPoints, _view.Rigidbody.position,
                    _currentDirection * _ctx.model.JumpDistance.Value, _ctx.model.JumpHeight.Value);
            }));

            _view.SetCtx(new AnimalView.Ctx
            {
                tryEat = _ctx.tryEat,
                changeDirection = _changeDirectionTrigger,
                model = _ctx.model
            });
            _changeDirectionTrigger.Notify(ChangeDirectionInfo.NewWay);

            AddDispose(_ctx.timeStream.SubscribeToStream(TimeStream.Streams.UPDATE, delta =>
            {
                CheckGroundPos(delta);
                CheckScreenPos();
            }));

            AddDispose(_ctx.timeStream.SubscribeToStream(TimeStream.Streams.PHYSICS, CheckGround));
            AddDispose(_onGround.Subscribe(value =>
            {
                if (!value)
                    return;

                _changeDirectionTrigger.Notify(ChangeDirectionInfo.NewWay);
            }));
        }


        private void CheckScreenPos()
        {
            Vector3 screenPoint = _ctx.camera.WorldToViewportPoint(_view.transform.position +
                                                                   _currentDirection *
                                                                   _ctx.model.JumpDistance.Value);

            bool onScreen = screenPoint is { z: > 0, x: > 0 and < 1, y: > 0 and < 1 };
            if (!onScreen)
                _changeDirectionTrigger.Notify(ChangeDirectionInfo.NewWay);
        }

        private void CheckGroundPos(float deltatime)
        {
            if (!_onGround.Value)
                return;

            _timerOnGround += deltatime;
            if (_timerOnGround < _ctx.model.JumpCooldown.Value)
                return;

            _changeDirectionTrigger.Notify(ChangeDirectionInfo.NewWay);
        }

        private void CheckGround(float deltaTime)
        {
            if (_pointIndex < _jumpPoints.Count - 1 && _view.Rigidbody.transform.position == _jumpPoints[_pointIndex])
                _pointIndex++;
            else if (_pointIndex == _jumpPoints.Count - 1 &&
                     _view.Rigidbody.transform.position == _jumpPoints[_pointIndex])
            {
                _onGround.Value = true;
                return;
            }

            Vector3 newPos = Vector3.MoveTowards(_view.Rigidbody.position, _jumpPoints[_pointIndex],
                _ctx.model.Speed.Value * deltaTime);
            _view.transform.position = newPos;
            _onGround.Value = false;
        }
    }
}