using System;
using Emotions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainScreen
{
    public class EmotionPlane : MonoBehaviour
    {
        [SerializeField] private TMP_Text _dailyTypeText;
        [SerializeField] private TMP_Text _streakText;
        [SerializeField] private Button _openButton;

        public event Action Opened;

        public EmotionData EmotionData { get; private set; }

        private void OnEnable()
        {
            _openButton.onClick.AddListener(OnButtonClicked);
        }

        private void OnDisable()
        {
            _openButton.onClick.RemoveListener(OnButtonClicked);
        }

        public void SetEmotionData(EmotionData data, int streak)
        {
            EmotionData = data ?? throw new ArgumentNullException(nameof(data));

            _dailyTypeText.text = EmotionData.EmotionType.ToString();
            _streakText.text = streak.ToString();
        }

        private void OnButtonClicked()
        {
            Opened?.Invoke();
        }
    }
}