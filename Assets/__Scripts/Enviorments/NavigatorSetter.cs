using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavigatorSetter : MonoBehaviour
{
    [SerializeField] private Transform    parentLabel;
    private                  Transform    left, right;
    private                  Selectable[] _leftSelectables;
    private                  Selectable[] _rightSelectables;

    private void Awake()
    {
        left  = transform.Find("Left");
        right = transform.Find("Right");

        _leftSelectables  = left.GetComponentsInChildren<Selectable>();
        _rightSelectables = right.GetComponentsInChildren<Selectable>();

        if (_leftSelectables.Length + _rightSelectables.Length == 0)
        {
            Debug.Log($"<color=yellow>NavigatorSetter</color> : {gameObject.name} has no selectables.");
        }

        //레이블 네비게이션의 오른쪽 방향을 요소로 설정
        Selectable _selectable = parentLabel.GetComponent<Selectable>();
        Navigation _nav        = _selectable.navigation;
        _nav.selectOnRight     = _leftSelectables.Length != 0 ? _leftSelectables[0] : null;
        _selectable.navigation = _nav;

        for (int _i = 0; _i < _leftSelectables.Length; _i++)
        {
            Selectable _targetSelectable = _leftSelectables[_i];
            _targetSelectable.navigation = new Navigation
                                           {
                                               mode          = Navigation.Mode.Explicit,
                                               selectOnLeft  = parentLabel.GetComponent<Selectable>(),
                                               selectOnUp    = _leftSelectables[(_i - 1 + _leftSelectables.Length) % _leftSelectables.Length],
                                               selectOnDown  = _leftSelectables[(_i     + 1)                       % _leftSelectables.Length],
                                               selectOnRight = _rightSelectables.Length != 0 ? _rightSelectables[0] : null //있으면 할당하고 없으면 비우기
                                           };
        }

        for (int _i = 0; _i < _rightSelectables.Length; _i++)
        {
            Selectable _targetSelectable = _rightSelectables[_i];
            _targetSelectable.navigation = new Navigation
                                           {
                                               mode          = Navigation.Mode.Explicit,
                                               selectOnLeft  = _leftSelectables.Length != 0 ? _leftSelectables[0] : null,
                                               selectOnUp    = _rightSelectables[(_i - 1 + _rightSelectables.Length) % _rightSelectables.Length],
                                               selectOnDown  = _rightSelectables[(_i     + 1)                        % _rightSelectables.Length],
                                               selectOnRight = null
                                           };
        }

        Debug.Log($"<color=yellow>NavigatorSetter</color> : {gameObject.name} is set. Left:{_leftSelectables.Length}, Right:{_rightSelectables.Length}");
    }
}