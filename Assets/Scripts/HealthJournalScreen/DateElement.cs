using System;
using Emotions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HealthJournalScreen
{
    public class DateElement : MonoBehaviour
    {
        [SerializeField] private Color _negativeColor;
        [SerializeField] private Color _neutralColor;
        [SerializeField] private Color _positiveColor;
        [SerializeField] private Color _unactiveColor;
        [SerializeField] private Color _activeColor;

        [SerializeField] private Color _activeMonthTextColor;
        [SerializeField] private Color _unactiveMonthTextColor;

        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Image _image;

        public DateTime HoldingDate { get; private set; }

        public event Action<DateTime> DateSelected;

        private void OnEnable()
        {
            _button.onClick.AddListener(OnButtonClicked);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnButtonClicked);
        }

        public void SetNagativeColor()
        {
            _image.color = _negativeColor;
            _text.color = _activeMonthTextColor;
        }

        public void SetNeutralColor()
        {
            _image.color = _negativeColor;
            _text.color = _activeMonthTextColor;
        }

        public void SetPositiveColor()
        {
            _image.color = _positiveColor;
            _text.color = _activeMonthTextColor;
        }

        public void UnSelect()
        {
            _image.color = _unactiveColor;
            _text.color = _unactiveMonthTextColor;
        }

        public void Select()
        {
            _image.color = _activeColor;
            _text.color = _activeMonthTextColor;
        }

        public void SetDate(DateTime dateTime)
        {
            HoldingDate = dateTime;
            _text.text = HoldingDate.Date.Day.ToString();
        }

        private void OnButtonClicked()
        {
            DateSelected?.Invoke(HoldingDate);
        }
    }
}