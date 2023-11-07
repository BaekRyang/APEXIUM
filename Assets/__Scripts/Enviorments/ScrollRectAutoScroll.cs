using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ScrollRectAutoScroll : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float scrollSpeed = 10f;
    private                  bool  _mouseOver;

    private readonly List<Selectable> _selectables = new();

    private ScrollRect _scrollRect;
    private Vector2    _nextScrollPosition = Vector2.up;

    private void OnEnable()
    {
        if (_scrollRect)
            _scrollRect.content.GetComponentsInChildren(_selectables);
    }

    private void Awake()
    {
        _scrollRect = GetComponent<ScrollRect>();
    }

    private void Start()
    {
        if (_scrollRect)
            _scrollRect.content.GetComponentsInChildren(_selectables);

        ScrollToSelected(true);
    }

    private void Update()
    {
        //스크롤 위치 얻어오기
        InputScroll();
        
        //스크롤 업데이트
        switch (_mouseOver)
        {
            case true:
                //해당 위치를 저장을 해둔다.
                _nextScrollPosition = _scrollRect.normalizedPosition;
                break;

            case false:
                //해당 위치로 Lerp 스크롤
                _scrollRect.normalizedPosition = Vector2.Lerp(_scrollRect.normalizedPosition, _nextScrollPosition, scrollSpeed * Time.deltaTime);
                break;
        }
    }

    void InputScroll()
    {
        
        if (_selectables.Count <= 0) return;
        
        //네비게이션 버튼이 눌렸을 때
        if (Input.GetButtonDown("Horizontal") || Input.GetButtonDown("Vertical") || Input.GetButton("Horizontal") || Input.GetButton("Vertical")) 
            ScrollToSelected(false);
    }

    void ScrollToSelected(bool _quickScroll)
    {
        int        _selectedIndex   = -1;
        
        //현재 선택된 요소가 있으면 그 요소의 Selectable을 가져온다.
        Selectable _selectedElement = EventSystem.current.currentSelectedGameObject ? EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>() : null;

        if (_selectedElement) //그리고 해당 컴포넌트의 인덱스를 가져온다.
            _selectedIndex = _selectables.IndexOf(_selectedElement);

        if (_selectedIndex <= -1) //선택된 요소가 없으면 return
            return;

        if (_quickScroll) //빠른 스크롤이면 바로 이동
        {
            _scrollRect.normalizedPosition = new Vector2(0, 1 - (_selectedIndex / ((float)_selectables.Count - 1)));
            _nextScrollPosition            = _scrollRect.normalizedPosition;
        }
        else //아니면 해당 위치를 저장해두고, Update에서 Lerp를 하도록 한다.
            _nextScrollPosition = new Vector2(0, 1 - _selectedIndex / ((float)_selectables.Count - 1));
    }

    // ReSharper disable once InconsistentNaming
    public void OnPointerEnter(PointerEventData eventData)
    {
        _mouseOver = true;
    }

    // ReSharper disable once InconsistentNaming
    public void OnPointerExit(PointerEventData eventData)
    {
        _mouseOver = false;
        ScrollToSelected(false);
    }
}