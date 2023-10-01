using System;
using Logic.Loaders.View;
using Logic.Scene.Animals.Core;
using Tools.Extensions.Reactive;
using Tools.Framework;
using UnityEngine;

namespace Logic.Scene.Animals.Snake
{
    public class AnimalView : BaseMonoBehaviour
    {
        public struct Ctx
        {
            public BaseAnimalModel model;
            public ReactiveEvent<EatInfo> tryEat;
            public ReactiveEvent<ChangeDirectionInfo> changeDirection;
        }

        [SerializeField] private Transform _asset;
        [SerializeField] private CapsuleCollider _collider;

        [SerializeField] private Rigidbody _rigidbody;

        [SerializeField] private Transform _pointForUi;

        public Transform PointForUi => _pointForUi;
        public BaseAnimalModel Model => _ctx.model;

        public CapsuleCollider Collider => _collider;

        public Rigidbody Rigidbody => _rigidbody;
        private Ctx _ctx;

        public void SetCtx(Ctx ctx)
        {
            _ctx = ctx;
        }
        //
        // private void OnCollisionStay(Collision other)
        // {
        //     if (other.gameObject.CompareTag("Wall"))
        //         _ctx.changeDirection.Notify();
        // }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Wall"))
            {
                
                var direction = other.GetContact(0).point - transform.position;
                if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
                    _ctx.changeDirection.Notify(ChangeDirectionInfo.ReverseX);
                else
                    _ctx.changeDirection.Notify(ChangeDirectionInfo.ReverseZ);
            }
            else
            {
                var otherAnimal = other.gameObject.GetComponent<AnimalView>();
                if (otherAnimal)
                {
                    _ctx.tryEat.Notify(new EatInfo
                    {
                        AttackerId = _ctx.model.Id,
                        DefenderId = otherAnimal.Model.Id
                    });
                    _ctx.changeDirection.Notify(ChangeDirectionInfo.NewWay);
                }
            }
        }
    }
}