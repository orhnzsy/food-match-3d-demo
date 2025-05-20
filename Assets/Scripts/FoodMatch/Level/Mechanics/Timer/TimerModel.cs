namespace FoodMatch.Level.Mechanics.Timer
{
    public class TimerModel
    {
        public float InitialTime { get; private set; }
        public float RemainingTime { get; private set; }
        public bool Done => RemainingTime <= 0;

        public TimerModel(float initialTime)
        {
            RemainingTime = initialTime;
        }

        public void OnTick(float deltaTime)
        {
            RemainingTime -= deltaTime;
            if (RemainingTime <= 0)
            {
                RemainingTime = 0;
            }
        }
    }
}