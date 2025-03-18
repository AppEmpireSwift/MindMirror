using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using HealthJournalScreen;
using NotesScreen.CreateNote;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NotesScreen
{
    [RequireComponent(typeof(ScreenVisabilityHandler))]
    public class HealthNotesScreenController : MonoBehaviour
    {
        [SerializeField] private Button _addButton;
        [SerializeField] private TagElement[] _tagElements;
        [SerializeField] private TMP_InputField _searchInput;
        [SerializeField] private List<NotePlane> _notePlanes;
        [SerializeField] private CreateNoteScreen _createNoteScreen;
        [SerializeField] private OpenNoteScreen _openNoteScreen;

        [Header("Animation Settings")] [SerializeField]
        private float _animationDuration = 0.3f;

        [SerializeField] private Ease _easeType = Ease.OutBack;

        private ScreenVisabilityHandler _screenVisabilityHandler;
        private List<NoteData> _noteDataList = new List<NoteData>();
        private TagType _currentFilterTag = TagType.None;
        private NoteData _editingNoteData;

        private void Awake()
        {
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();

            if (_notePlanes == null)
                _notePlanes = new List<NotePlane>();

            InitializeTagElements();

            LoadSavedNotes();
        }

        private void OnEnable()
        {
            _addButton.onClick.AddListener(OnAddNoteClicked);
            _searchInput.onValueChanged.AddListener(OnSearchInputChanged);

            foreach (TagElement tagElement in _tagElements)
            {
                tagElement.TagSelected += OnTagFilterSelected;
            }

            if (_createNoteScreen != null)
            {
                _createNoteScreen.BackClicked += OnCreateNoteBack;
                _createNoteScreen.NoteCreated += OnNoteCreated;
                _createNoteScreen.NoteUpdated += OnNoteUpdated;
            }

            if (_openNoteScreen != null)
            {
                _openNoteScreen.BackClicked += OnOpenNoteBack;
                _openNoteScreen.EditClicked += OnEditNoteClicked;
            }

            foreach (NotePlane notePlane in _notePlanes)
            {
                notePlane.DataSelected += OnNoteClicked;
            }
        }

        private void OnDisable()
        {
            _addButton.onClick.RemoveListener(OnAddNoteClicked);
            _searchInput.onValueChanged.RemoveListener(OnSearchInputChanged);

            foreach (TagElement tagElement in _tagElements)
            {
                tagElement.TagSelected -= OnTagFilterSelected;
            }

            if (_createNoteScreen != null)
            {
                _createNoteScreen.BackClicked -= OnCreateNoteBack;
                _createNoteScreen.NoteCreated -= OnNoteCreated;
                _createNoteScreen.NoteUpdated -= OnNoteUpdated;
            }

            if (_openNoteScreen != null)
            {
                _openNoteScreen.BackClicked -= OnOpenNoteBack;
                _openNoteScreen.EditClicked -= OnEditNoteClicked;
            }

            foreach (NotePlane notePlane in _notePlanes)
            {
                notePlane.DataSelected -= OnNoteClicked;
            }

            DOTween.Kill(this);
        }

        private void Start()
        {
            _screenVisabilityHandler.DisableScreen();
        }

        private void LoadSavedNotes()
        {
            _noteDataList = NotesDataManager.LoadNotes();
        }

        public void Enable()
        {
            _screenVisabilityHandler.EnableScreen();
            RefreshNotesDisplay();
            AnimateUIElements(true);
        }

        public void Disable()
        {
            AnimateUIElements(false, () => _screenVisabilityHandler.DisableScreen());
        }

        private void OnNoteUpdated(NoteData updatedNoteData)
        {
            for (int i = 0; i < _noteDataList.Count; i++)
            {
                if (_noteDataList[i].Title == _editingNoteData.Title &&
                    _noteDataList[i].Description == _editingNoteData.Description)
                {
                    _noteDataList[i] = updatedNoteData;

                    NotesDataManager.UpdateNote(_editingNoteData, updatedNoteData);

                    break;
                }
            }

            _editingNoteData = null;
            RefreshNotesDisplay();
        }

        private void AnimateUIElements(bool show, Action onComplete = null)
        {
            Sequence sequence = DOTween.Sequence().SetId(this);

            if (_searchInput != null)
            {
                RectTransform searchRect = _searchInput.transform as RectTransform;
                if (show)
                {
                    searchRect.localScale = Vector3.zero;
                    sequence.Append(searchRect.DOScale(1f, _animationDuration).SetEase(_easeType));
                }
                else
                {
                    sequence.Append(searchRect.DOScale(0f, _animationDuration).SetEase(Ease.InBack));
                }
            }

            if (_addButton != null)
            {
                RectTransform addButtonRect = _addButton.transform as RectTransform;
                if (show)
                {
                    addButtonRect.localScale = Vector3.zero;
                    sequence.Join(addButtonRect.DOScale(1f, _animationDuration).SetEase(_easeType));
                }
                else
                {
                    sequence.Join(addButtonRect.DOScale(0f, _animationDuration).SetEase(Ease.InBack));
                }
            }

            for (int i = 0; i < _tagElements.Length; i++)
            {
                RectTransform tagRect = _tagElements[i].transform as RectTransform;
                tagRect.localScale = show ? Vector3.one : Vector3.zero;
            }

            if (onComplete != null)
            {
                sequence.OnComplete(() => onComplete.Invoke());
            }

            sequence.Play();
        }

        private void EnablePlane(NoteData data)
        {
            NotePlane availablePlane = _notePlanes.FirstOrDefault(p => !p.IsActive);

            if (availablePlane != null)
            {
                availablePlane.Enable(data);
                AnimateNewPlane(availablePlane);
            }
        }

        private void AnimateNewPlane(NotePlane plane)
        {
            RectTransform planeRect = plane.transform as RectTransform;

            if (planeRect != null)
            {
                planeRect.localScale = Vector3.zero;
                planeRect.DOScale(1f, _animationDuration).SetEase(_easeType);
            }
        }

        private void InitializeTagElements()
        {
            foreach (TagElement tagElement in _tagElements)
            {
                tagElement.SetSelected(false);
            }

            _currentFilterTag = TagType.None;
        }

        private void DisableAllPlanes()
        {
            foreach (NotePlane notePlane in _notePlanes)
            {
                notePlane.Disable();
            }
        }

        private void RefreshNotesDisplay()
        {
            DisableAllPlanes();

            string searchText = _searchInput.text.ToLower();

            var filteredNotes = _noteDataList
                .Where(note =>
                    (string.IsNullOrEmpty(searchText) ||
                     note.Title.ToLower().Contains(searchText)) &&
                    (_currentFilterTag == TagType.None || note.TagType == _currentFilterTag))
                .ToList();

            for (int i = 0; i < Math.Min(filteredNotes.Count, _notePlanes.Count); i++)
            {
                _notePlanes[i].Enable(filteredNotes[i]);
            }
        }

        #region Event Handlers

        private void OnAddNoteClicked()
        {
            AnimateButtonClick(_addButton);
            _createNoteScreen.Enable();
            Disable();
        }

        private void OnSearchInputChanged(string searchText)
        {
            RefreshNotesDisplay();
        }

        private void OnTagFilterSelected(TagElement tagElement)
        {
            _currentFilterTag = tagElement.TagType;
            foreach (TagElement element in _tagElements)
            {
                element.Unselect();
            }

            tagElement.Select();

            RefreshNotesDisplay();
        }

        private void OnNoteCreated(NoteData noteData)
        {
            Enable();
            _noteDataList.Add(noteData);

            NotesDataManager.AddNote(noteData);

            RefreshNotesDisplay();
        }

        private void OnCreateNoteBack()
        {
            Enable();
        }

        private void OnNoteClicked(NotePlane notePlane)
        {
            if (_openNoteScreen != null)
            {
                _openNoteScreen.Enable(notePlane);
            }

            Disable();
        }

        private void OnOpenNoteBack()
        {
            Enable();
        }

        private void OnEditNoteClicked(NotePlane notePlane)
        {
            if (_createNoteScreen != null && notePlane != null && notePlane.NoteData != null)
            {
                _editingNoteData = notePlane.NoteData;
                _createNoteScreen.EnableEditMode(notePlane.NoteData);
            }
        }

        private void AnimateButtonClick(Button button)
        {
            if (button == null)
                return;

            Sequence clickSequence = DOTween.Sequence();
            clickSequence.Append(button.transform.DOScale(0.9f, 0.1f).SetEase(Ease.OutQuad));
            clickSequence.Append(button.transform.DOScale(1f, 0.1f).SetEase(Ease.OutBack));
            clickSequence.Play();
        }

        #endregion
    }
}