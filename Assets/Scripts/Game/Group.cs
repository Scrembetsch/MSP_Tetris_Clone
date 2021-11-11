using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Group : MonoBehaviour
{
    #region Inspector
    [SerializeField]
    public bool BlockRotation;
    #endregion

    #region Fields
    private int _ChildCount;
    #endregion

    #region Align Helpers
    public Vector2 GetWeightOffset()
    {
        Vector2 meanPosition = Vector2.zero;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            meanPosition += (Vector2)child.transform.localPosition;
        }
        meanPosition /= transform.childCount;
        return meanPosition;
    }
    #endregion

    #region Mono
    private void Awake()
    {
        _ChildCount = transform.childCount;
    }
    #endregion

    #region Controller
    public void ChildDestroyed()
    {
        _ChildCount--;
        if(_ChildCount <= 0)
        {
            Destroy(gameObject);
        }
    }
    #endregion
}
