using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.Rendering;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public class UI_InputPad : MonoBehaviour
{


    public RectTransform Knob;

    public CanvasGroup joystickCanvas;

    public RectTransform dragArea;
    public float draggingMaxLength = 100;


    public RectTransform RectTransform
    {
        get=>this.transform as RectTransform;
    }

    protected  void OnEnable()
    {
        
        EnhancedTouchSupport.Enable();

        ETouch.Touch.onFingerDown += HandleFingerDown;
        ETouch.Touch.onFingerUp += HandleLoseFinger ;
   //     ETouch.Touch.onFingerMove += HandleFingerMove;
        
    }
   
    void OnDisable()
    {
        EnhancedTouchSupport.Disable();
        ETouch.Touch.onFingerDown -= HandleFingerDown;
        ETouch.Touch.onFingerUp -= HandleLoseFinger;
     //   ETouch.Touch.onFingerMove -= HandleFingerMove;

    }


    private void HandleLoseFinger(Finger finger)
    {
        if(finger == movementFinger)
        {
            movementFinger = null;
            Knob.anchoredPosition = Vector2.zero;
        
            movementAmount = Vector2.zero;
            joystickCanvas.alpha = 0.5f;
            joystickCanvas.interactable = false;
        }
    }

    Finger movementFinger;
    public Vector2 movementAmount;

    private void HandleFingerDown(Finger TouchedFinger)
    {
        var touchedScreenPos = TouchedFinger.screenPosition;

        if (movementFinger == null && IsOnDragArea(touchedScreenPos))
        {
            movementFinger = TouchedFinger;
            movementAmount = Vector2.zero;

            RectTransform.anchoredPosition = touchedScreenPos;// ClampStartPosition(touchedScreenPos);
            joystickCanvas.alpha = 1;
            joystickCanvas.interactable = true;
        }

    }

    private bool IsOnDragArea(Vector2 touchedScreenPos)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(dragArea, touchedScreenPos, null, out localPoint);
        return dragArea.rect.Contains(localPoint);
    }

    private Vector2 ClampStartPosition(Vector2 startPosition)
    {
        var joystickSize= RectTransform.sizeDelta;
    
        if(startPosition.x < joystickSize.x/2) {
            startPosition.x= joystickSize.x/2;
        }

        if(startPosition.y < joystickSize.y/2)
        {
            startPosition.y= joystickSize.y/2;
        }else if(startPosition.y > Screen.height - joystickSize.y/2)
        {            
            startPosition.y= Screen.height - joystickSize.y/2;        
        }

        return startPosition;


    }


}
