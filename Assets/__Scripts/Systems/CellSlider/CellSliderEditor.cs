using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(CellSlider))]
public class CellSliderEditor : Editor
{
    private void OnEnable()
    {
        CellSlider _cellSlider = (CellSlider)target;

        if (_cellSlider.firstLoad) return;

        _cellSlider.firstLoad = true;
        ApplySetting(_cellSlider);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        CellSlider _cellSlider = (CellSlider)target;

        //Value라는 이름의 Slider를 만들어준다.
        _cellSlider.value = (int)EditorGUILayout.Slider("Value", _cellSlider.value, _cellSlider.minValue, _cellSlider.maxValue);

        //ApplySetting 버튼을 만든다.
        if (GUILayout.Button("ApplySetting"))
            ApplySetting(_cellSlider);

        //FillRect의 Anchor Max X를 Value값에 맞춰준다.
        _cellSlider.fillRect.anchorMax = new((float)_cellSlider.value / _cellSlider.maxValue, 1);
    }

    private void ApplySetting(CellSlider p_cellSlider)
    {
        if (p_cellSlider.cellObject == null)
        {
            //프리팹을 만들어서 저장한다.
            p_cellSlider.cellObject = new("CellObject(!MakePrefab!)", typeof(Image));
            p_cellSlider.cellObject.GetComponent<RectTransform>().anchorMin =
                p_cellSlider.cellObject.GetComponent<RectTransform>().anchorMax = new(0, 0);
        }

        if (p_cellSlider.backGroundRect != null) DestroyImmediate(p_cellSlider.backGroundRect.gameObject);
        if (p_cellSlider.fillRect       != null) DestroyImmediate(p_cellSlider.fillRect.gameObject);

        var _backGroundRect = new GameObject("BackGroundRect").AddComponent<RectTransform>();
        _backGroundRect.SetParent(p_cellSlider.transform);
        p_cellSlider.backGroundRect = _backGroundRect;
        _backGroundRect.localScale  = _backGroundRect.anchorMax = Vector2.one;
        _backGroundRect.anchorMin   = _backGroundRect.offsetMin = _backGroundRect.offsetMax = new(0, 0);

        HorizontalLayoutGroup layoutGroup = _backGroundRect.AddComponent<HorizontalLayoutGroup>();

        layoutGroup.childControlHeight = layoutGroup.childControlWidth = layoutGroup.childForceExpandHeight = layoutGroup.childControlWidth = true;

        //자식으로 CellObject를 maxValue만큼 생성해준다.
        for (int i = 0; i < p_cellSlider.maxValue; i++)
        {
            var _cell = Instantiate(p_cellSlider.cellObject, _backGroundRect.transform);
            _cell.name                        = $"Cell {i + 1}";
            _cell.transform.localScale        = Vector3.one;
            _cell.GetComponent<Image>().color = p_cellSlider.disableColor;
        }

        var _fillRect = new GameObject("FillRect").AddComponent<RectTransform>();
        _fillRect.SetParent(p_cellSlider.transform);
        p_cellSlider.fillRect = _fillRect;

        _fillRect.localScale = _fillRect.anchorMax = Vector2.one;
        _fillRect.anchorMin  = _fillRect.offsetMin = _fillRect.offsetMax = new(0, 0);

        _fillRect.AddComponent<Mask>();
        _fillRect.AddComponent<Image>().color = new(0, 0, 0, 0.01f);

        //자식으로 CellObject를 maxValue만큼 생성해준다.
        for (int i = 0; i < p_cellSlider.maxValue; i++)
        {
            GameObject _cell = Instantiate(p_cellSlider.cellObject, _fillRect.transform);
            _cell.name                        = $"Cell {i + 1}";
            _cell.transform.localScale        = Vector3.one;
            _cell.GetComponent<Image>().color = p_cellSlider.enableColor;

            RectTransform cellRect = _cell.GetComponent<RectTransform>();
            cellRect.sizeDelta = new(p_cellSlider.backGroundRect.rect.width / p_cellSlider.maxValue, p_cellSlider.backGroundRect.rect.height);
            Vector2 cellSize = cellRect.sizeDelta;

            cellRect.pivot            = new(.5f, .5f);
            cellRect.anchoredPosition = new(i * cellSize.x + cellSize.x / 2, cellSize.y / 2);
        }
    }
}