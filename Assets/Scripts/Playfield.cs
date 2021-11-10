using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Playfield : MonoBehaviour
{
    #region Inspector
    public int Height;
    public int Width;
    [SerializeField]
    private GameObject[] _Groups;
    #endregion

    #region Fields
    public Transform[,] Grid;
    #endregion

    #region Mono
    private void Awake()
    {
        Grid = new Transform[Width, Height];
    }

    private void Start()
    {
        SpawnNext();
    }
    #endregion

    #region Spawner
    public void SpawnNext()
    {
        int index = Random.Range(0, _Groups.Length);

        Vector3 spawnPosition = transform.position;
        spawnPosition.y += Height - 5;
        spawnPosition.x += Width / 2;
        GroupController group = Instantiate(_Groups[index], spawnPosition, Quaternion.identity).GetComponent<GroupController>();
        group.Playfield = this;
    }
    #endregion

    #region Group Checkers
    public bool IsInsideBorder(Vector2Int position)
    {
        return position.x >= 0
            && position.x < Width
            && position.y >= 0;
    }

    public bool IsGridPointFree(Vector2Int position, Transform parent)
    {
        return Grid[position.x, position.y] == null
            || Grid[position.x, position.y].parent == parent;
    }

    public static Vector2Int ConvertPositionToGridPoint(Vector3 position)
    {
        Vector2Int ret = Vector2Int.zero;
        ret.x = Mathf.RoundToInt(position.x);
        ret.y = Mathf.RoundToInt(position.y);
        return ret;
    }
    #endregion

    #region Grid Controller
    private void DeleteRow(int y)
    {
        for (int x = 0; x < Width; x++)
        {
            Destroy(Grid[x, y].gameObject);
            Grid[x, y] = null;
        }
    }

    private void DecreaseRow(int y)
    {
        int yLower = y - 1;
        for (int x = 0; x < Width; x++)
        {
            if (Grid[x, y] != null)
            {
                Grid[x, yLower] = Grid[x, y];
                Grid[x, y] = null;

                Grid[x, yLower].position += Vector3.down;
            }
        }
    }

    private void DecreaseAbove(int y)
    {
        y++;
        for(; y < Height; y++)
        {
            DecreaseRow(y);
        }
    }

    private bool IsRowFull(int y)
    {
        for(int x = 0; x < Width; x++)
        {
            if(Grid[x, y] == null)
            {
                return false;
            }
        }
        return true;
    }

    public void DeleteFullRows()
    {
        for(int y = 0; y < Height; y++)
        {
            if (IsRowFull(y))
            {
                Debug.Log("Deleted");
                DeleteRow(y);
                DecreaseAbove(y);
                y--;
            }
        }
    }
    #endregion

    #region Helper
    public void PrintGrid()
    {
        StringBuilder t = new StringBuilder();
        t.Append(" \n ");
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                t.Append(Grid[x, y] == null ? "o" : "x");
            }
            t.Append(" \n ");
        }
        Debug.Log(t.ToString());
    }
    #endregion
}
