using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HealthJournalScreen
{
    public class JournalPlane : MonoBehaviour
    {
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private TMP_Text _dateText;
        [SerializeField] private TMP_Text _moodText;
        [SerializeField] private Button _button;

        public event Action<JournalPlane> PlaneSelected;

        public bool IsActive { get; private set; }
        public JournalData JournalData { get; private set; }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnButtonClicked);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnButtonClicked);
        }

        public void Enable(JournalData data)
        {
            JournalData = data ?? throw new ArgumentNullException(nameof(data));

            IsActive = true;
            gameObject.SetActive(IsActive);

            _titleText.text = JournalData.Name;
            _descriptionText.text = JournalData.Description;
            _dateText.text = JournalData.Date.ToString("dd.MM.yyyy");
            _moodText.text = JournalData.EmotionType.ToString();
        }

        public void Disable()
        {
            IsActive = false;
            gameObject.SetActive(IsActive);

            _titleText.text = string.Empty;
            _descriptionText.text = string.Empty;
            _dateText.text = string.Empty;
            _moodText.text = string.Empty;
            JournalData = null;
        }

        private void OnButtonClicked()
        {
            PlaneSelected?.Invoke(this);
        }
    }
}