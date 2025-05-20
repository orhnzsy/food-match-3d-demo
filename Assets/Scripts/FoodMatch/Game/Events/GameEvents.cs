using System;

namespace FoodMatch.Game.Events
{
    public static class GameEvents
    {
        public static Action ThereIsNotEnoughSpace;
        public static Action MatchOccured;
        public static Action TimerEnded;
        public static Action<string> ItemCollected;
        public static Action AllOrdersCompleted;
    }
}