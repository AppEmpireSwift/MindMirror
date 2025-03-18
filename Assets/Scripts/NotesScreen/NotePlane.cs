using System;
using PhotoPicker;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NotesScreen
{
    public class NotePlane : MonoBehaviour
    {
        [SerializeField] private TMP_Text _typeText;
        [SerializeField] private ImagePlacer _imagePlacer;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private Button _openButton;

        public event Action<NotePlane> DataSelected;

        public bool IsActive { get; private set; }
        public NoteData NoteData { get; private set; }

        private void OnEnable()
        {
            _openButton.onClick.AddListener(OnButtonClicked);
        }

        private void OnDisable()
        {
            _openButton.onClick.RemoveListener(OnButtonClicked);
        }

        public void Enable(NoteData data)
        {
            NoteData = data ?? throw new ArgumentNullException(nameof(data));

            IsActive = true;
            gameObject.SetActive(IsActive);

            _typeText.text = NoteData.TagType.ToString();
            _imagePlacer.SetImage(NoteData.Photo);
            _titleText.text = NoteData.Title;
        }

        public void Disable()
        {
            IsActive = false;
            gameObject.SetActive(IsActive);

            _typeText.text = string.Empty;
            _imagePlacer.SetImage(null);
            _titleText.text = string.Empty;
            NoteData = null;
        }

        private void OnButtonClicked()
        {
            DataSelected?.Invoke(this);
        }
    }
}