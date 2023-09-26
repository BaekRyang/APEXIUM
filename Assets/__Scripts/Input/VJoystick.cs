using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private GameObject movementArea;

    [SerializeField] private GameObject movementKnob;
    [DoNotSerialize] private Transform  knobTransform;

    [SerializeField] private GameObject movementArrow;
    [DoNotSerialize] private Transform  arrowTransform;
    
    private float _movementAreaLeft;
    private float _movementAreaRight;
    
    private Vector3 _lastPressedPosition;

    public bool _isPressed;
    public bool _isDraging;

    public static int MovementDirection { get; private set; } = 0;

    private void Awake()
    {
        movementArea   = gameObject;
        knobTransform  = movementKnob.GetComponent<Transform>();
        arrowTransform = movementArrow.GetComponent<Transform>();
        
        _movementAreaLeft  = movementArea.GetComponent<RectTransform>().rect.xMin;
        _movementAreaRight = movementArea.GetComponent<RectTransform>().rect.xMax;
    }

    private void Update()
    {
        if (_isPressed && !_isDraging) //처음 눌렀을 때
            knobTransform.position = _lastPressedPosition = Input.mousePosition;

        if (_isDraging) //드래그 중일 때
        {
            Vector2 _mousePosition = Input.mousePosition;
            arrowTransform.position = new Vector2(
                Mathf.Clamp(_mousePosition.x, _movementAreaLeft, _movementAreaRight),
                _lastPressedPosition.y
                ); //화살표 위치를 화면 안으로 제한

            arrowTransform.rotation = Quaternion.Euler(0, 0, //마지막 누른 위치 기준으로 화살표를 회전
                                                       _mousePosition.x > _lastPressedPosition.x ?
                                                           270 : 90);
            
            MovementDirection = _mousePosition.x > _lastPressedPosition.x ? 1 : -1; //처음 누른 위치 기준으로 방향 설정
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isPressed = true;
        movementKnob.SetActive(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_isPressed)
        {
            _isDraging = true;
            movementArrow.SetActive(true);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        MovementDirection = 0;
        _isPressed = false;
        _isDraging = false;
        movementKnob.SetActive(false);
        movementArrow.SetActive(false);
    }
}