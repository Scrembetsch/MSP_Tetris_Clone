using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Playfield : MonoBehaviour
{
    #region Constants
    private const int _DefaultHeight = 20;
    private const int _DefaultWidth = 10;
    #endregion

    #region Inspector
    [Header("Playfield")]
    public int Height;
    public int Width;

    [Header("HelpLines")]
    [SerializeField]
    private GameObject _VerticalHelpLine;
    [SerializeField]
    private GameObject _HorizontalHelpLine;
    [SerializeField]
    private GameObject _LeftBorder;
    [SerializeField]
    private GameObject _BottomBorder;

    [Header("Spawner")]
    [SerializeField]
    private GameObject[] _Groups;
    [SerializeField]
    private Camera _NextBlockCamera;

    [Header("Score")]
    [SerializeField]
    private TMPro.TextMeshPro _Score;
    #endregion

    #region Fields
    public GroupPart[,] Grid;
    private GroupController _GroupController;
    private Group _NextGroup;

    private int _CurrentScore;
    #endregion

    #region Mono
    private void Awake()
    {
        Grid = new GroupPart[Width, Height];
        _GroupController = GetComponent<GroupController>();
    }

    private void Start()
    {
        SpawnNext();
        SpawnHelpLines();
    }
    #endregion

    #region Spawner
    public void SpawnNext()
    {
        if(_NextGroup == null)
        {
            SpawnNextGroup();
        }

        Group currentGroup = _NextGroup;
        SpawnNextGroup();

        Vector3 spawnPosition = transform.position;
        spawnPosition.y += Height - 5;
        spawnPosition.x += Width / 2;

        currentGroup.transform.position = spawnPosition;
        _GroupController.CurrentGroup = currentGroup;
    }

    private void SpawnNextGroup()
    {
        int index = Random.Range(0, _Groups.Length);
        _NextGroup = Instantiate(_Groups[index]).GetComponent<Group>();

        Vector2 weightOffset = _NextGroup.GetWeightOffset();
        Vector3 spawnPosition = _NextBlockCamera.transform.position;
        spawnPosition.z = 0;
        spawnPosition.x -= weightOffset.x;
        spawnPosition.y -= weightOffset.y;

        _NextGroup.gameObject.transform.position = spawnPosition;
    }
    #endregion

    #region Group Checkers
    public bool IsInsideBorder(Vector2Int position)
    {
        return position.x >= 0
            && position.x < Width
            && position.y >= 0;
    }

    public bool IsGridPointFree(Vector2Int position)
    {
        return Grid[position.x, position.y] == null;
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
            Grid[x, y].Destroy();
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

                Grid[x, yLower].gameObject.transform.position += Vector3.down;
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
                DeleteRow(y);
                DecreaseAbove(y);
                y--;
            }
        }
    }
    #endregion

    #region Help Lines
    private void SpawnHelpLines()
    {
        float verticalSpacing = (float)_DefaultHeight / Height;
        float horizontalSpacing = (float)_DefaultWidth / Width;

        Transform horizontalParent = _HorizontalHelpLine.transform.parent;
        for(int y = 0; y < Height - 1; y++)
        {
            Instantiate(_HorizontalHelpLine,
                new Vector3(_HorizontalHelpLine.transform.position.x, _BottomBorder.transform.position.y + verticalSpacing + y * verticalSpacing, _HorizontalHelpLine.transform.position.z),
                _HorizontalHelpLine.transform.rotation,
                horizontalParent);
        }
        Destroy(_HorizontalHelpLine);

        Transform verticalParent = _VerticalHelpLine.transform.parent;
        for (int x = 0; x < Width - 1; x++)
        {
            Instantiate(_VerticalHelpLine,
                new Vector3(_LeftBorder.transform.position.x + horizontalSpacing + x * horizontalSpacing, _VerticalHelpLine.transform.position.y, _VerticalHelpLine.transform.position.z),
                _VerticalHelpLine.transform.rotation,
                verticalParent);
        }
        Destroy(_VerticalHelpLine);
    }
    #endregion

    #region Score
    private void AddScore()
    {

    }
    #endregion
}
