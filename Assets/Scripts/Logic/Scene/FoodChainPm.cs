using Logic.Loaders.View;
using Logic.Scene.Animals.Core;
using Tools.Extensions.Reactive;
using Tools.Framework;
using UniRx;

namespace Logic.Scene
{
    public class FoodChainPm: BaseDisposable
    {
        public struct Ctx
        {
            public IReadOnlyReactiveDictionary<int, AnimalInfo> animals;
            public IReadOnlyReactiveEvent<EatInfo> tryEat;
        }

        private Ctx _ctx;

        public FoodChainPm(Ctx ctx)
        {
            _ctx = ctx;
            AddDispose(_ctx.tryEat.SubscribeWithSkip(eatInfo =>
            {
                if (!_ctx.animals.TryGetValue(eatInfo.AttackerId, out var attackerInfo))
                {
                    log.Err($"Dot find animal with id {eatInfo.AttackerId}");
                    return;
                }
                
                if (attackerInfo.Model.AnimalType == AnimalType.Prey)
                    return;
                
                if (!_ctx.animals.TryGetValue(eatInfo.DefenderId, out var defenderInfo))
                {
                    log.Err($"Dot find animal with id {eatInfo.DefenderId}");
                    return;
                }
                
                log.Info($"Animal {eatInfo.AttackerId} eat  {eatInfo.DefenderId}");
                


            }));
        }
    }
}