using System;
using Stress;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainScreen
{
    public class StressPlane : MonoBehaviour
    {
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private TMP_Text _typeText;
        [SerializeField] private Button _openButton;

        public event Action OpenButtonClicked;

        private void OnEnable()
        {
            _openButton.onClick.AddListener(OnButtonClicked);
        }

        private void OnDisable()
        {
            _openButton.onClick.RemoveListener(OnButtonClicked);
        }

        public void SetData(StressData data)
        {
            _levelText.text = "Level " + data.Number;
            _typeText.text = GetTypeText(data.StressType);
        }

        private string GetTypeText(StressType type)
        {
            switch (type)
            {
                case StressType.Overwhelmed:
                    return "Overwhelmed";
                case StressType.HighlyStressed:
                    return "Highly Stressed";
                case StressType.MildlyTense:
                    return "Mildly Tense";
                case StressType.ModeratelyStressed:
                    return "Moderately Stressed";
                case StressType.RelaxedStressed:
                    return "Relaxed Stressed";
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private void OnButtonClicked()
        {
            OpenButtonClicked?.Invoke();
        }
    }
}