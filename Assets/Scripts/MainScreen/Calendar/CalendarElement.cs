using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainScreen.Calendar
{
    public class CalendarElement : MonoBehaviour
    {
        [SerializeField] private Color _selectedPlaneColor;
        [SerializeField] private Color _defaultPlaneColor;
        [SerializeField] private Color _selectedTextColor;
        [SerializeField] private Color _defaultTextColor;

        [SerializeField] private TMP_Text _dateNumberText;
        [SerializeField] private TMP_Text _dayNameText;
        [SerializeField] private Image _planeImage;

        public void Initialize(DateTime dateTime)
        {
            _dateNumberText.text = dateTime.Day.ToString();
            _dayNameText.text = dateTime.ToString("ddd");

            if (dateTime.Day == DateTime.Today.Day)
            {
                HandleTypeSelected();
                return;
            }

            HandleTypeNotSelected();
        }

        private void HandleTypeSelected()
        {
            _planeImage.color = _selectedPlaneColor;
            _dateNumberText.color = _selectedTextColor;
        }

        private void HandleTypeNotSelected()
        {
            _planeImage.color = _defaultPlaneColor;
            _dateNumberText.color = _defaultTextColor;
        }
    }
}