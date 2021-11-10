using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupController : MonoBehaviour
{
    #region Fields
    [HideInInspector]
    public Playfield Playfield;
    private float _LastFall = 0.0f;
    #endregion

    #region Update
    private void Update()
    {
        UpdatePosition();
    }
    #endregion

    #region Controller
    private void UpdatePosition()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.position += Vector3.left;

            if (IsValidGridPosition())
            {
                UpdateGrid();
            }
            else
            {
                transform.position += Vector3.right;
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.position += Vector3.right;

            if (IsValidGridPosition())
            {
                UpdateGrid();
            }
            else
            {
                transform.position += Vector3.left;
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            transform.Rotate(Vector3.forward * 90);

            if (IsValidGridPosition())
            {
                UpdateGrid();
            }
            else
            {
                transform.Rotate(Vector3.back * 90);
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow)
            || Time.time - _LastFall >= 1)
        {
            transform.position += Vector3.down;

            if (IsValidGridPosition())
            {
                UpdateGrid();
            }
            else
            {
                transform.position += new Vector3(0, 1, 0);

                Playfield.DeleteFullRows();

                Playfield.SpawnNext();

                enabled = false;
            }

            _LastFall = Time.time;
        }
    }
    #endregion

    #region Helper
    private bool IsValidGridPosition()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);

            Vector2Int gridPosition = Playfield.ConvertPositionToGridPoint(child.position);

            if (!Playfield.IsInsideBorder(gridPosition))
            {
                return false;
            }
            if (!Playfield.IsGridPointFree(gridPosition, transform))
            {
                return false;
            }
        }
        return true;
    }

    private void UpdateGrid()
    {
        for(int y = 0; y < Playfield.Height; y++)
        {
            for(int x = 0; x < Playfield.Width; x++)
            {
                if (Playfield.Grid[x, y] != null
                    && Playfield.Grid[x, y].parent == transform)
                {
                    Playfield.Grid[x, y] = null;
                }
            }
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            Vector2Int gridPosition = Playfield.ConvertPositionToGridPoint(child.position);
            Playfield.Grid[gridPosition.x, gridPosition.y] = child;
        }
    }
    #endregion
}
