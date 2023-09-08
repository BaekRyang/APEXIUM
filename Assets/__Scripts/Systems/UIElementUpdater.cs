using System;
using System.Text;
using AssetKits.ParticleImage.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIElementUpdater : MonoBehaviour
{
    [SerializeField] private string indexFormat    = "0.0";
    [SerializeField] private string indexSeparator = "/";
    [SerializeField] private string indexSuffix    = "";

    private object _value;
    private object _maxValue;

    [SerializeField] private Slider   _slider;
    [SerializeField] private TMP_Text _text;
    
    public event EventHandler OnUpdateValue;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        TryGetComponent(out _slider);
        transform.Find("Value")?.TryGetComponent(out _text);

        this.AssignToGlobal();
    }

    public void UpdateValue(object _currentValue, object _newMaxValue)
    {
        _value    = _currentValue;
        _maxValue = _newMaxValue;
        UpdateUI();
        OnUpdateValue?.Invoke(this, EventArgs.Empty);
    }

    private void UpdateUI()
    {
        float _parsedValue    = Convert.ToSingle(_value);
        float _parsedMaxValue = _maxValue != null ? Convert.ToSingle(_maxValue) : 0;

        ValueUpdate(_parsedValue, _parsedMaxValue);
        TextUpdate(_parsedValue, _parsedMaxValue);
    }

    private void TextUpdate(float _current, float _max)
    {
        if (_text == null) return;

        StringBuilder _stringBuilder = new();
        
        _stringBuilder.Append(_current.ToString(indexFormat));

        if (indexSeparator != "")
            _stringBuilder.Append(indexSeparator + _max.ToString(indexFormat));

        if (indexSuffix != "")
            _stringBuilder.Append(indexSuffix);

        _text.text = _stringBuilder.ToString();
    }

    private void ValueUpdate(float _current, float _max)
    {
        if (_maxValue == null || _slider == null) return;

        _slider.maxValue = _max;
        _slider.value    = _current;
    }
}