using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AutoScrollDropdown : TMP_Dropdown
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        if (eventData.selectedObject.GetComponentInChildren<Scrollbar>() != null || !IsActive() || !IsInteractable())
            return;

        Scrollbar scrollbar = gameObject.GetComponentInChildren<ScrollRect>()?.verticalScrollbar;
        if (options.Count > 1 && scrollbar != null)
        {
            if (scrollbar.direction == Scrollbar.Direction.TopToBottom)
                scrollbar.value = Mathf.Max(0.001f, value / (float)(options.Count - 1));
            else
                scrollbar.value = Mathf.Max(0.001f, 1.0f - value / (float)(options.Count - 1));
        }
    }

    private float   mouseMoveTime;
    private Vector3 preMousePos;

    void Update()
    {
        if (preMousePos != Input.mousePosition)
        {
            mouseMoveTime = Time.time;
            preMousePos   = Input.mousePosition;
        }
    }

    public void MoveScrollToTarget(BaseEventData eventData)
    {
        if (Time.time - mouseMoveTime < 0.1f)
            return;

        var sbIdx = eventData.selectedObject.transform.GetSiblingIndex();

        Scrollbar scrollbar = gameObject.GetComponentInChildren<ScrollRect>()?.verticalScrollbar;
        if (options.Count > 1 && scrollbar != null)
        {
            if (scrollbar.direction == Scrollbar.Direction.TopToBottom)
                scrollbar.value = Mathf.Max(0.001f, sbIdx / (float)(options.Count - 1));
            else
                scrollbar.value = Mathf.Max(0.001f, 1.0f - sbIdx / (float)(options.Count - 1));
        }
    }
}