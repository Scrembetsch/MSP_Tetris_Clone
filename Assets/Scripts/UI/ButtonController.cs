using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GroupController;

public class ButtonController : MonoBehaviour
{
    #region Inspector
    [SerializeField]
    private GroupController _GroupController;
    [SerializeField]
    private ControlKey _ControlKey;
    #endregion

    #region Fields
    private UiButton _Button;
    private Coroutine _HoldCoroutine;
    #endregion

    #region Mono
    private void Awake()
    {
        _Button = GetComponent<UiButton>();
        _Button.OnButtonDown = OnButtonClicked;
        _Button.OnButtonUp = OnButtonReleased;

    }
    #endregion

    #region Controller
    private void OnButtonClicked()
    {
        _HoldCoroutine = StartCoroutine(HoldRoutine());
    }

    private void OnButtonReleased()
    {
        StopCoroutine(_HoldCoroutine);
    }

    private IEnumerator HoldRoutine()
    {
        _GroupController.DoAction(_ControlKey);
        yield return new WaitForSeconds(0.4f);
        while(true)
        {
            _GroupController.DoAction(_ControlKey);
            yield return new WaitForSeconds(0.1f);
        }
    }
    #endregion
}
