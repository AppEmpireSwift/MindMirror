using Emotions;
using Stress;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StressLevelScreen
{
    public class StressLevelHolder : MonoBehaviour
    {
        [SerializeField] private Color _selectedColor;
        [SerializeField] private Color _unselectedColor;
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _text;

        [field: SerializeField] public StressType StressType { get; private set; }

        public void SetSelected()
        {
            _image.color = _selectedColor;
            _text.gameObject.SetActive(true);
        }

        public void SetUnselected()
        {
            _image.color = _unselectedColor;
            _text.gameObject.SetActive(false);
        }
    }
}