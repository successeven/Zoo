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
            public ReactiveEvent<BaseAnimalModel> tryEat;
            public ReactiveEvent<ChangeDirectionInfo> changeDirection;
            public ReactiveTrigger<Vector3> reflectVelocity;
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

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Wall"))
            {
                _ctx.reflectVelocity.Notify(other.GetContact(0).normal);
            }
            else if (other.gameObject.CompareTag("Animal"))
            {
                var otherAnimal = other.gameObject.GetComponent<AnimalView>();
                if (otherAnimal)
                {
//                    Debug.Log($"{_ctx.model.AnimalName} try eat {otherAnimal.Model.AnimalName}");
                    _ctx.tryEat.Notify(otherAnimal.Model);
                    _ctx.changeDirection.Notify(ChangeDirectionInfo.NewWay);
                }
            }
        }
    }
}