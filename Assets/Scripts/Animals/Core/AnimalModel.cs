using UniRx;

namespace Logic.Scene.Animals.Core
{
    public class BaseAnimalModel
    {
        public int Id;
        public AnimalType AnimalType;
        public ReactiveProperty<float> Speed { get; }

        public BaseAnimalModel()
        {
            Speed = new ReactiveProperty<float>();
        }
    }
}