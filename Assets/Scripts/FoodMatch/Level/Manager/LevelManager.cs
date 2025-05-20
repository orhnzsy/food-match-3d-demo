using System.Collections.Generic;
using FoodMatch.Game;
using FoodMatch.Game.Events;
using FoodMatch.Game.Level;
using FoodMatch.Level.Data;
using UnityEngine;

namespace FoodMatch.Level.Controller
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private List<InLevelManager> _inLevelManagers;
        [SerializeField] private ItemDataRepo _itemDataRepo;
        [SerializeField] private GameObject _winScreen;
        [SerializeField] private GameObject _loseScreen;
        private LevelState LevelState { get; set; } = LevelState.Playing;

        private void Awake()
        {
            GameEvents.AllOrdersCompleted += OnAllOrdersCompleted;
            GameEvents.ThereIsNotEnoughSpace += OnThereIsNotEnoughSpace;
            GameEvents.TimerEnded += OnTimerEnded;
        }

        private void Start()
        {
            PrepareLevel(GameManager.Instance.CurrentLevelData);
        }

        private void PrepareLevel(LevelData levelData)
        {
            _inLevelManagers.ForEach(x => x.PrepareLevel(levelData, _itemDataRepo));
        }

        public void Update()
        {
            if (LevelState == LevelState.Paused)
            {
                return;
            }

            _inLevelManagers.ForEach(x => x.OnUpdate());
        }

        private void OnDestroy()
        {
            GameEvents.AllOrdersCompleted -= OnAllOrdersCompleted;
            GameEvents.ThereIsNotEnoughSpace -= OnThereIsNotEnoughSpace;
            GameEvents.TimerEnded -= OnTimerEnded;
        }

        private void OnTimerEnded()
        {
            LevelState = LevelState.Paused;
            _loseScreen.SetActive(true);
        }

        private void OnAllOrdersCompleted()
        {
            LevelState = LevelState.Paused;
            GameManager.Instance.IncreaseLevel();
            _winScreen.SetActive(true);
        }

        private void OnThereIsNotEnoughSpace()
        {
            LevelState = LevelState.Paused;
            _loseScreen.SetActive(true);
        }

        public void Click_ContinueButtonClicked()
        {
            GameManager.Instance.LoadHome();
        }

        public void Click_RestartButtonClicked()
        {
            _inLevelManagers.ForEach(x => x.Cleanup());
            _inLevelManagers.ForEach(x => x.PrepareLevel(GameManager.Instance.CurrentLevelData, _itemDataRepo));
            LevelState = LevelState.Playing;
        }
    }

    public enum LevelState
    {
        Playing,
        Paused,
    }
}