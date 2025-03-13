using System;
using UnityEngine;

namespace MainScreen.Calendar
{
    public class Calendar : MonoBehaviour
    {
        [SerializeField] private CalendarElement[] _elements;

        private CalendarElement _currentSelectedElement;
        private DateTime[] _dates;

        private void Start()
        {
            InitializeDates();
            InitializeElements();
        }

        private void InitializeDates()
        {
            _dates = new DateTime[7];
            DateTime currentDate = DateTime.Now;

            int diff = (int)currentDate.DayOfWeek - (int)DayOfWeek.Sunday;
            if (diff < 0)
                diff += 7;

            DateTime sunday = currentDate.AddDays(-diff);

            for (int i = 0; i < 7; i++)
            {
                _dates[i] = sunday.AddDays(i);
            }
        }

        private void InitializeElements()
        {
            for (int i = 0; i < _elements.Length; i++)
            {
                _elements[i].Initialize(_dates[i]);
            }
        }
    }
}