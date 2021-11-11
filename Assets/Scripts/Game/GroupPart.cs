using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupPart : MonoBehaviour
{
    #region Inspector
    [SerializeField]
    private Group _Group;
    #endregion

    #region Controller
    public void Destroy()
    {
        _Group.ChildDestroyed();
        Destroy(gameObject);
    }
    #endregion
}
