using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainScreen
{
    public class JournalPlane : MonoBehaviour
    {
        [SerializeField] private Button _openButton;
        [SerializeField] private TMP_Text _journalText;

        public event Action ButtonClicked;

        private void OnEnable()
        {
            _openButton.onClick.AddListener(OnButtonClicked);
        }

        private void OnDisable()
        {
            _openButton.onClick.RemoveListener(OnButtonClicked);
        }

        public void UpdateText(int count)
        {
            _journalText.text = count + "/365";
        }

        private void OnButtonClicked()
        {
            ButtonClicked?.Invoke();
        }
    }
}