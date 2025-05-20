using FoodMatch.Game.Events;
using FoodMatch.Game.Level;
using FoodMatch.Level.Controller;
using FoodMatch.Level.Data;
using TMPro;
using UnityEngine;

namespace FoodMatch.Level.Mechanics.Timer
{
    public class TimerManager : InLevelManager
    {
        [SerializeField] private TextMeshPro _timerText;

        private TimerModel TimerModel { get; set; }

        public override void PrepareLevel(LevelData levelData, ItemDataRepo itemDataRepo)
        {
            base.PrepareLevel(levelData, itemDataRepo);
            TimerModel = new TimerModel(levelData.TimeLimit);
        }

        public override void OnUpdate()
        {
            if (TimerModel.Done)
            {
                return;
            }

            TimerModel.OnTick(Time.deltaTime);

            if (TimerModel.Done)
            {
                GameEvents.TimerEnded?.Invoke();
            }

            var minutes = Mathf.FloorToInt(TimerModel.RemainingTime / 60F);
            var seconds = Mathf.FloorToInt(TimerModel.RemainingTime - minutes * 60);
            _timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
        }
    }
}