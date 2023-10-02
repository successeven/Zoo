using Logic.Loaders.View;
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
            public BaseAnimalModel model;
            public ReactiveTrigger<int> death;
            public ReactiveEvent<int> showLabel;
        }
        
        protected ReactiveEvent<BaseAnimalModel> _tryEat;
        private AnimalCtx _ctx;
        protected Animal(AnimalCtx ctx)
        {
            _ctx = ctx;
            _tryEat = new ReactiveEvent<BaseAnimalModel>();
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
                // if (defenderModel.AnimalType == AnimalType.Prey)
                // {
                //     defenderModel.Alive.Value = false;
                // }
            }));
        }
    }
}