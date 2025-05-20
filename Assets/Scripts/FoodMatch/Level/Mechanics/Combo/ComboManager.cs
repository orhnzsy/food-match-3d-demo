using Base.UI;
using FoodMatch.Game.Events;
using FoodMatch.Level.Controller;
using TMPro;
using UnityEngine;

namespace FoodMatch.Level.Mechanics.Combo
{
    public class ComboManager : InLevelManager
    {
        [SerializeField] private float comboDeactivationTime = 2f;
        [SerializeField] private TextMeshPro _comboText;
        [SerializeField] private SpriteProgressBar _comboProgressBar;

        public int CurrentCombo { get; private set; } = 1;
        public float ComboTime { get; private set; }

        private void Awake()
        {
            GameEvents.MatchOccured += OnMatchOccured;
        }

        private void OnDestroy()
        {
            GameEvents.MatchOccured -= OnMatchOccured;
        }

        private void OnMatchOccured()
        {
            IncreaseCombo();
        }

        public override void OnUpdate()
        {
            if (CurrentCombo <= 1)
            {
                return;
            }

            float comboEndTime = ComboTime + comboDeactivationTime;
            float remainingTime = (comboEndTime - Time.time) / comboDeactivationTime;
            _comboProgressBar.SetFillAmount(remainingTime);

            if (remainingTime < 0)
            {
                CurrentCombo = 1;
                UpdateComboText();
                _comboProgressBar.SetFillAmount(0);
            }
        }

        public void IncreaseCombo()
        {
            CurrentCombo++;
            UpdateComboText();
            _comboProgressBar.SetFillAmount(1f);
            ComboTime = Time.time;
        }

        public void UpdateComboText()
        {
            if (_comboText != null)
            {
                _comboText.text = $"x{CurrentCombo}";
            }
        }
    }
}