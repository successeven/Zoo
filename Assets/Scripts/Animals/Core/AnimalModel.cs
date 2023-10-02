using UniRx;

namespace Logic.Scene.Animals.Core
{
    public class BaseAnimalModel
    {
        public int Id;
        public AnimalType AnimalType;
        public AnimalNames AnimalName;

        public ReactiveProperty<bool> Alive { get; }
        public ReactiveProperty<float> Speed { get; }

        public BaseAnimalModel()
        {
            Alive = new ReactiveProperty<bool>(true);
            Speed = new ReactiveProperty<float>();
        }
    }
}