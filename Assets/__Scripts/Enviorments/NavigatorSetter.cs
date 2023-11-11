using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NavigatorSetter : MonoBehaviour
{
    [SerializeField] private Transform        parentLabel;
    private                  Transform        left, right;
    private                  List<Selectable> _leftSelectables;
    private                  List<Selectable> _rightSelectables;

    private void Awake()
    {
        left  = transform.Find("Left");
        right = transform.Find("Right");

        _leftSelectables  = left.GetComponentsInChildren<Selectable>().ToList();
        _rightSelectables = right.GetComponentsInChildren<Selectable>().ToList();

        List<Selectable> _removeLeft = _leftSelectables.FindAll(_selectable => _selectable is Slider);
        foreach (Selectable _targetSelectable in _removeLeft) _leftSelectables.Remove(_targetSelectable);

        List<Selectable> _removeRight = _rightSelectables.FindAll(_selectable => _selectable is Slider);
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

        //삭제한 슬라이더들의 인덱스가 기존의 버튼과 1:1 대응이 되기 때문에
        //해당 조건을 바탕으로 슬라이더의 위아래 네비게이션을 다른 슬라이더의 버튼으로 설정해준다.
        SetSliderNavigation(_removeLeft);
        SetSliderNavigation(_removeRight);

        Debug.Log($"<color=yellow>NavigatorSetter</color> : {gameObject.name} is set. Left:{_leftSelectables.Count}, Right:{_rightSelectables.Count}");
    }

    /// <summary>
    /// 슬라이더 전용 네비게이션 설정
    /// </summary>
    private void SetSliderNavigation(IReadOnlyList<Selectable> _sliderList)
    {
        for (int _i = 0; _i < _sliderList.Count; _i++)
        {
            Selectable _selectable = _sliderList[_i];
            _selectable.navigation = new Navigation
                                     {
                                         mode          = Navigation.Mode.Explicit,
                                         selectOnLeft  = null,
                                         selectOnUp    = _leftSelectables[(_i - 1 + _leftSelectables.Count) % _leftSelectables.Count],
                                         selectOnDown  = _leftSelectables[(_i     + 1)                      % _leftSelectables.Count],
                                         selectOnRight = null
                                     };
        }
    }
}