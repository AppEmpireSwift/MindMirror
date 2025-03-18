using System;
using Emotions;
using UnityEngine;
using UnityEngine.UI;

namespace HealthJournalScreen.CreateJournalScreen
{
    public class EmotionButton : MonoBehaviour
    {
        [SerializeField] private Color _selectedColor;
        [SerializeField] private Color _unselectedColor;

        [SerializeField] private EmotionType _emotionType;
        [SerializeField] private Button _button;

        public event Action<EmotionButton> EmotionSelected;

        public EmotionType EmotionType => _emotionType;

        private void OnEnable()
        {
            _button.onClick.AddListener(OnButtonClicked);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnButtonClicked);
        }

        public void SetSelected()
        {
            _button.image.color = _selectedColor;
        }

        public void SetUnselected()
        {
            _button.image.color = _unselectedColor;
        }

        private void OnButtonClicked()
        {
            EmotionSelected?.Invoke(this);
            SetSelected();
        }
    }
}