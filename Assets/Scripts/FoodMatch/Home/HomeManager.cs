using System;
using FoodMatch.Game;
using TMPro;
using UnityEngine;

namespace FoodMatch.Home
{
    public class HomeManager : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _levelText;

        private void Awake()
        {
            _levelText.text = $"Level {(GameManager.Instance.GetCurrentLevel() + 1)}";
        }

        public void OnNextLevelButtonClicked()
        {
            GameManager.Instance.LoadNextLevel();
        }
    }
}