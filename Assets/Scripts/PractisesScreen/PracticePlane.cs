using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace PractisesScreen
{
    public class PracticePlane : MonoBehaviour
    {
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _timerText;
        [SerializeField] private TMP_Text _typeText;
        [SerializeField] private Button _openButton;

        public PracticeData Data;

        public event Action<PracticeData> OpenButtonClicked;

        public bool IsActive { get; private set; }

        private void OnEnable()
        {
            _openButton.onClick.AddListener(OnButtonClicked);
        }

        private void OnDisable()
        {
            _openButton.onClick.RemoveListener(OnButtonClicked);
        }

        private void Start()
        {
            if (!string.IsNullOrEmpty(Data.Type) && !string.IsNullOrEmpty(Data.Title) && Data.Minutes > 0)
            {
                UpdateUIElements();
                return;
            }

            Disable();
        }

        public void Enable(PracticeData data)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));

            IsActive = true;
            gameObject.SetActive(IsActive);

            UpdateUIElements();
        }

        public void Disable()
        {
            IsActive = false;
            gameObject.SetActive(IsActive);

            Data = null;
        }

        private void UpdateUIElements()
        {
            _titleText.text = Data.Title;
            _timerText.text = Data.Minutes + " min";
            _typeText.text = Data.Type;
        }

        private void OnButtonClicked()
        {
            OpenButtonClicked?.Invoke(Data);
        }
    }
}