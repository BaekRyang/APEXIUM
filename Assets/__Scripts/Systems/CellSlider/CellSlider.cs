using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CellSlider : MonoBehaviour
{
    [SerializeField]  public int           minValue = 0;
    [SerializeField]  public int           maxValue = 10;
    [SerializeField]  public RectTransform backGroundRect;
    [SerializeField]  public RectTransform fillRect;
    [SerializeField]  public GameObject    cellObject;
    [SerializeField]  public Color         enableColor  = new(1, 1, 1, 1);
    [SerializeField]  public Color         disableColor = new(.25f, .25f, .25f, 1);
    [HideInInspector] public int           value;
    [HideInInspector] public int           lastValue;
    [HideInInspector] public bool          firstLoad;

    public GameObject[] ActiveCellObjects
    {
        get
        {
            //fillRect의 자식들을 가져온다.
            GameObject[] _cellObjects = new GameObject[fillRect.childCount];
            for (int i = 0; i < fillRect.childCount; i++)
                _cellObjects[i] = fillRect.GetChild(i).gameObject;
            
            return _cellObjects;
        }
    }
    
    public GameObject[] DisableCellObjects
    {
        get
        {
            //fillRect의 자식들을 가져온다.
            GameObject[] _cellObjects = new GameObject[backGroundRect.childCount];
            for (int i = 0; i < backGroundRect.childCount; i++)
                _cellObjects[i] = backGroundRect.GetChild(i).gameObject;
            
            return _cellObjects;
        }
    }

    //value의 값이 바뀌였을때
    private void Update()
    {
        if (lastValue != value)
        {
            lastValue          = value;
            fillRect.anchorMax = new((float)value / maxValue, 1);
        }
    }
    
    public void ApplySetting()
    {
        if (backGroundRect != null) DestroyImmediate(backGroundRect.gameObject);
        if (fillRect       != null) DestroyImmediate(fillRect.gameObject);

        var _backGroundRect = new GameObject("BackGroundRect").AddComponent<RectTransform>();
        _backGroundRect.SetParent(transform);
        backGroundRect = _backGroundRect;
        _backGroundRect.localScale  = _backGroundRect.anchorMax = Vector2.one;
        _backGroundRect.anchorMin   = _backGroundRect.offsetMin = _backGroundRect.offsetMax = new(0, 0);

        HorizontalLayoutGroup layoutGroup = _backGroundRect.AddComponent<HorizontalLayoutGroup>();

        layoutGroup.childControlHeight = layoutGroup.childControlWidth = layoutGroup.childForceExpandHeight = layoutGroup.childControlWidth = true;

        //자식으로 CellObject를 maxValue만큼 생성해준다.
        for (int i = 0; i < maxValue; i++)
        {
            var _cell = Instantiate(cellObject, _backGroundRect.transform);
            _cell.name                        = $"Cell {i + 1}";
            _cell.transform.localScale        = Vector3.one;
            _cell.GetComponent<Image>().color = disableColor;
        }

        var _fillRect = new GameObject("FillRect").AddComponent<RectTransform>();
        _fillRect.SetParent(transform);
        fillRect = _fillRect;

        _fillRect.localScale = _fillRect.anchorMax = Vector2.one;
        _fillRect.anchorMin  = _fillRect.offsetMin = _fillRect.offsetMax = new(0, 0);

        _fillRect.AddComponent<Mask>();
        _fillRect.AddComponent<Image>().color = new(0, 0, 0, 0.01f);

        //자식으로 CellObject를 maxValue만큼 생성해준다.
        for (int i = 0; i < maxValue; i++)
        {
            GameObject _cell = Instantiate(cellObject, _fillRect.transform);
            _cell.name                        = $"Cell {i + 1}";
            _cell.transform.localScale        = Vector3.one;
            _cell.GetComponent<Image>().color = enableColor;

            RectTransform cellRect = _cell.GetComponent<RectTransform>();
            cellRect.sizeDelta = new(backGroundRect.rect.width / maxValue, backGroundRect.rect.height);
            Vector2 cellSize = cellRect.sizeDelta;

            cellRect.pivot            = new(.5f, .5f);
            cellRect.anchoredPosition = new(i * cellSize.x + cellSize.x / 2, cellSize.y / 2);
        }
    }
}