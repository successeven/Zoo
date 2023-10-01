using Logic.Scene.Animals.Core;
using UniRx;

namespace Animals.Frog
{
    public class FrogModel : BaseAnimalModel
    {
        public ReactiveProperty<float> JumpHeight { get; }
        public ReactiveProperty<float> JumpDistance { get; }
        public ReactiveProperty<float> JumpCooldown { get; }

        public FrogModel() :base()
        {
            JumpDistance = new ReactiveProperty<float>();
            JumpHeight = new ReactiveProperty<float>();
            JumpCooldown = new ReactiveProperty<float>();
        }
    }
}