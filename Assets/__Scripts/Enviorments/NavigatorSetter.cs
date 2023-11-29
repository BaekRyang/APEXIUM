using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 버튼 입력을 통한 네비게이션을 자동으로 설정해주는 스크립트
/// 버튼의 네비게이션 맵을 Explicit으로 전부 설정해주지 않고
/// 자동으로 주변의 요소를 파악하여 버튼을 이어준다.
/// </summary>
public class NavigatorSetter : MonoBehaviour
{
    [SerializeField] private Transform parentLabel;

    private void Awake()
    {
        var _leftSelectables  = transform.Find("Left").GetComponentsInChildren<Selectable>().ToList();
        var _rightSelectables = transform.Find("Right").GetComponentsInChildren<Selectable>().ToList();

        var _removeLeft = _leftSelectables.FindAll(_selectable => _selectable is Slider);
        foreach (Selectable _targetSelectable in _removeLeft) _leftSelectables.Remove(_targetSelectable);

        var _removeRight = _rightSelectables.FindAll(_selectable => _selectable is Slider);
        foreach (Selectable _targetSelectable in _removeRight) _rightSelectables.Remove(_targetSelectable);

        if (_leftSelectables.Count + _rightSelectables.Count == 0)
        {
            Debug.Log($"<color=yellow>NavigatorSetter</color> : {gameObject.name} has no selectables.");
        }

        //레이블 네비게이션의 오른쪽 방향을 요소로 설정
        {
            Selectable _selectable = parentLabel.GetComponent<Selectable>();
            Navigation _nav        = _selectable.navigation;
            _nav.selectOnRight     = _leftSelectables.Count != 0 ? _leftSelectables[0] : null;
            _selectable.navigation = _nav;
        }

        for (int _i = 0; _i < _leftSelectables.Count; _i++)
        {
            Selectable _targetSelectable = _leftSelectables[_i];
            _targetSelectable.navigation = new Navigation
                                           {
                                               mode          = Navigation.Mode.Explicit,
                                               selectOnLeft  = parentLabel.GetComponent<Selectable>(),
                                               selectOnUp    = _leftSelectables[(_i - 1 + _leftSelectables.Count) % _leftSelectables.Count],
                                               selectOnDown  = _leftSelectables[(_i     + 1)                      % _leftSelectables.Count],
                                               selectOnRight = _rightSelectables.Count != 0 ? _rightSelectables[0] : null //있으면 할당하고 없으면 비우기
                                           };
        }

        for (int _i = 0; _i < _rightSelectables.Count; _i++)
        {
            Selectable _selectable = _rightSelectables[_i];
            _selectable.navigation = new Navigation
                                           {
                                               mode          = Navigation.Mode.Explicit,
                                               selectOnLeft  = _leftSelectables.Count != 0 ? _leftSelectables[0] : null,
                                               selectOnUp    = _rightSelectables[(_i - 1 + _rightSelectables.Count) % _rightSelectables.Count],
                                               selectOnDown  = _rightSelectables[(_i     + 1)                       % _rightSelectables.Count],
                                               selectOnRight = null
                                           };
        }
        
        SetSliderNavigation(_removeLeft);
        SetSliderNavigation(_removeRight);
    }

    /// <summary>
    /// 슬라이더 전용 네비게이션 설정
    /// </summary>
    private void SetSliderNavigation(IReadOnlyList<Selectable> _sliderList)
    {
        foreach (Selectable _selectable in _sliderList)
        {
            Selectable _selectButton = _selectable.transform.parent.parent.GetComponent<Selectable>();
            
            _selectable.navigation = new Navigation
                            {
                                mode          = Navigation.Mode.Explicit,
                                selectOnLeft  = null,
                                selectOnUp    = _selectButton,
                                selectOnDown  = _selectButton,
                                selectOnRight = null
                            };
        }
    }
}