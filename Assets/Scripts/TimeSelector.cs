using System;
using DanielLochner.Assets.SimpleScrollSnap;
using TMPro;
using UnityEngine;

public class TimeSelector : MonoBehaviour
{
    [SerializeField] private Color _selectedColor;
    [SerializeField] private Color _unselectedColor;

    [SerializeField] private SimpleScrollSnap _minuteScrollSnap;
    [SerializeField] private SimpleScrollSnap _secondsScrollSnap;
    [SerializeField] private TMP_Text[] _minuteText;
    [SerializeField] private TMP_Text[] _secondsText;

    private string _minute;
    private string _seconds;

    public event Action<string> MinuteInputed;
    public event Action<string> SecondsInputed;

    private void OnEnable()
    {
        _minuteScrollSnap.OnPanelCentered.AddListener(SetMinute);
        _secondsScrollSnap.OnPanelCentered.AddListener(SetSeconds);
    }

    private void OnDisable()
    {
        _minuteScrollSnap.OnPanelCentered.RemoveListener(SetMinute);
        _secondsScrollSnap.OnPanelCentered.RemoveListener(SetSeconds);
    }

    private void Start()
    {
        InitializeTimeFields();
    }

    public void Enable()
    {
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        Reset();
        gameObject.SetActive(false);
    }

    private void SetMinute(int start, int end)
    {
        _minute = _minuteText[start].text;
        SetColorForSelected(_minuteText, start);
        MinuteInputed?.Invoke(_minute);
    }

    private void SetSeconds(int start, int end)
    {
        _seconds = _secondsText[start].text;
        SetColorForSelected(_secondsText, start);
        SecondsInputed?.Invoke(_seconds);
    }

    private void InitializeTimeFields()
    {
        PopulateMinutes();
        PopulateSeconds();
        SetColorForSelected(_minuteText, 0);
        SetColorForSelected(_secondsText, 0);
    }

    private void PopulateMinutes()
    {
        for (int i = 0; i < _minuteText.Length; i++)
        {
            _minuteText[i].text = i < 60 ? i.ToString("00") : "";
        }
    }

    private void PopulateSeconds()
    {
        for (int i = 0; i < _secondsText.Length; i++)
        {
            _secondsText[i].text = i < 60 ? i.ToString("00") : "";
        }
    }

    private void SetColorForSelected(TMP_Text[] texts, int selectedIndex)
    {
        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].color = i == selectedIndex ? _selectedColor : _unselectedColor;
        }
    }

    private void Reset()
    {
        _minuteScrollSnap.GoToPanel(0);
        _secondsScrollSnap.GoToPanel(0);

        _minute = string.Empty;
        _seconds = string.Empty;
    }
        
    public string GetTimeString()
    {
        if (string.IsNullOrEmpty(_minute) || string.IsNullOrEmpty(_seconds))
            return string.Empty;
                
        return $"{_minute}:{_seconds}";
    }
}