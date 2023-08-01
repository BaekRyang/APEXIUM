using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Resources : MonoBehaviour
{
    public static Resources Instance;
    
    private Image _resourceImage;
    private TMP_Text  _resourceValue;
    
    private Image _advancedResourceImage;
    private TMP_Text  _advancedResourceValue;

    public TMP_Text ResourceValue
    {
        get => _resourceValue;
        set => _resourceValue = value;
    }

    public TMP_Text AdvancedResourceValue
    {
        get => _advancedResourceValue;
        set => _advancedResourceValue = value;
    }

    private void Awake()
    {
        Instance ??= this;
        
        var _resource = transform.Find("Resource");
        _resourceImage = _resource.Find("Icon").GetComponent<Image>();
        ResourceValue = _resource.Find("Value").GetComponent<TMP_Text>();
        
        var _specialResource = transform.Find("AdvResource");
        _advancedResourceImage = _specialResource.Find("Icon").GetComponent<Image>();
        AdvancedResourceValue = _specialResource.Find("Value").GetComponent<TMP_Text>();
    }
}
