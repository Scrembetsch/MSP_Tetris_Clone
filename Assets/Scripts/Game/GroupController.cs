using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupController : MonoBehaviour
{
    #region Enums
    public enum ControlKey
    {
        None = -1,
        LeftArrow,
        RightArrow,
        UpArrow,
        DownArrow,

        Num
    }
    #endregion

    #region Fields
    [HideInInspector]
    public Group CurrentGroup;

    private Playfield _Playfield;
    private ControlKey _CurrentButtonEvent;

    private Coroutine _HoldCoroutine;
    private float _LastFall = 0.0f;
    #endregion

    #region Mono
    private void Awake()
    {
        _Playfield = GetComponent<Playfield>();
    }

    private void Update()
    {
        UpdatePosition();
    }
    #endregion

    #region Controller
    private void UpdatePosition()
    {
        CheckInput(ControlKey.LeftArrow);
        CheckInput(ControlKey.RightArrow);
        CheckInput(ControlKey.UpArrow);
        CheckInput(ControlKey.DownArrow);

        if (Time.time - _LastFall >= 1)
        {
            DoAction(ControlKey.DownArrow);
        }
    }
    #endregion

    #region Input Helpers
    private void CheckInput(ControlKey controlKey)
    {
        KeyCode keycode;
        switch (controlKey)
        {
            case ControlKey.LeftArrow:
                keycode = KeyCode.LeftArrow;
                break;

            case ControlKey.RightArrow:
                keycode = KeyCode.RightArrow;
                break;

            case ControlKey.UpArrow:
                keycode = KeyCode.UpArrow;
                break;

            case ControlKey.DownArrow:
                keycode = KeyCode.DownArrow;
                break;

            default:
                return;
        }

        if(_CurrentButtonEvent == controlKey)
        {
            DoAction(_CurrentButtonEvent);
            _CurrentButtonEvent = ControlKey.None;
            return;
        }

        if (Input.GetKeyDown(keycode))
        {
            if(_HoldCoroutine != null)
            {
                StopCoroutine(_HoldCoroutine);
            }
            _HoldCoroutine = StartCoroutine(HoldRoutine(controlKey));
        }
        else if (Input.GetKeyUp(keycode))
        {
            if (_HoldCoroutine != null)
            {
                StopCoroutine(_HoldCoroutine);
            }
        }
    }

    public void DoAction(ControlKey controlKey)
    {
        switch (controlKey)
        {
            case ControlKey.LeftArrow:
                MoveInDirection(Vector3.left);
                break;

            case ControlKey.RightArrow:
                MoveInDirection(Vector3.right);
                break;

            case ControlKey.UpArrow:
                if(!CurrentGroup.BlockRotation)
                {
                    RotateGroup(Vector3.forward * 90);
                }
                break;

            case ControlKey.DownArrow:
                if (!MoveInDirection(Vector3.down))
                {
                    InsertCurrentGridPosition();
                    _Playfield.DeleteFullRows();
                    _Playfield.SpawnNext();
                }

                _LastFall = Time.time;
                break;

            default:
                break;
        }
    }

    private IEnumerator HoldRoutine(ControlKey controlKey)
    {
        DoAction(controlKey);
        yield return new WaitForSeconds(0.4f);
        while (true)
        {
            DoAction(controlKey);
            yield return new WaitForSeconds(0.1f);
        }
    }
    #endregion

    #region Transform Helpers
    private bool CheckIfMoveIsPossible(Vector3 direction)
    {
        CurrentGroup.gameObject.transform.position += direction;
        bool ret = IsValidGridPosition();
        CurrentGroup.gameObject.transform.position -= direction;
        return ret;
    }

    private bool CheckIfRotatePossible(Vector3 rotation)
    {
        CurrentGroup.gameObject.transform.Rotate(rotation);
        bool ret = IsValidGridPosition();
        CurrentGroup.gameObject.transform.Rotate(-rotation);
        return ret;
    }

    private bool MoveInDirection(Vector3 direction)
    {
        if (CheckIfMoveIsPossible(direction))
        {
            CurrentGroup.gameObject.transform.position += direction;
            return true;
        }
        return false;
    }

    private bool RotateGroup(Vector3 rotation)
    {
        if (CheckIfRotatePossible(rotation))
        {
            CurrentGroup.gameObject.transform.Rotate(rotation);
            return true;
        }
        return false;
    }
    #endregion

    #region Grid Helpers
    private bool IsValidGridPosition()
    {
        for(int i = 0; i < CurrentGroup.gameObject.transform.childCount; i++)
        {
            Transform child = CurrentGroup.gameObject.transform.GetChild(i);

            Vector2Int gridPosition = Playfield.ConvertPositionToGridPoint(child.position);

            if (!_Playfield.IsInsideBorder(gridPosition))
            {
                return false;
            }
            if (!_Playfield.IsGridPointFree(gridPosition))
            {
                return false;
            }
        }
        return true;
    }

    private void InsertCurrentGridPosition()
    {
        for (int i = 0; i < CurrentGroup.gameObject.transform.childCount; i++)
        {
            Transform child = CurrentGroup.gameObject.transform.GetChild(i);
            Vector2Int gridPosition = Playfield.ConvertPositionToGridPoint(child.position);
            _Playfield.Grid[gridPosition.x, gridPosition.y] = child.GetComponent<GroupPart>();
        }
    }
    #endregion

    #region Button Events
    public void LeftButtonClicked()
    {

    }
    #endregion
}
