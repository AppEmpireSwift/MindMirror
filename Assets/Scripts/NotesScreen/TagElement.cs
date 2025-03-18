using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NotesScreen
{
    public class TagElement : MonoBehaviour
    {
        [SerializeField] private Color _selectedTextColor;
        [SerializeField] private Color _unselectedTextColor;

        [SerializeField] private Color _selectedPlaneColor;
        [SerializeField] private Color _unselectedPlaneColor;

        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _text;

        public event Action<TagElement> TagSelected;

        [field: SerializeField] public TagType TagType { get; private set; }

        private void OnEnable()
        {
            _button.onClick.AddListener(Selected);

            _text.text = TagType.ToString();
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(Selected);
        }

        public void Selected()
        {
            TagSelected?.Invoke(this);
        }

        public void Select()
        {
            _button.image.color = _selectedPlaneColor;
            _text.color = _selectedTextColor;
        }

        public void Unselect()
        {
            _button.image.color = _unselectedPlaneColor;
            _text.color = _unselectedTextColor;
        }

        public void SetSelected(bool isSelected)
        {
            if (isSelected)
            {
                Selected();
            }
            else
            {
                Unselect();
            }
        }
    }

    public enum TagType
    {
        Health,
        Meditation,
        Selfcare,
        Stress,
        Psychology,
        Nutrition,
        Inspiration,
        None
    }
}