using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UiButton : Button
{
    #region Events
    public delegate void Event_OnButtonDown();
    public delegate void Event_OnButtonUp();
    #endregion

    #region Fields
    private bool _IsDown;
    public bool IsDown => _IsDown;

    public Event_OnButtonDown OnButtonDown;
    public Event_OnButtonDown OnButtonUp;
    #endregion

    #region Overrides
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        _IsDown = true;
        if(OnButtonDown != null)
        {
            OnButtonDown.Invoke();
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        _IsDown = false;
        if (OnButtonUp != null)
        {
            OnButtonUp.Invoke();
        }
    }
    #endregion
}
