using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class WallPlacementManager : MonoBehaviour
{
    public GameObject wallPrefab; // �� ������
    public Tilemap tilemap; // Ÿ�ϸ� ��ü
    public int maxWalls = 8; // ��ġ ������ ���� �ִ� ����
    private int placedWalls = 0; // ���� ��ġ�� ���� ����
    private bool gameStarted = false; // ���� ���� ����
    public EnemySpawner enemySpawner; // �� ������ ����

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (gameStarted) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 10f;
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
            Vector3Int tilePosition = tilemap.WorldToCell(worldPosition);

            if (tilemap.HasTile(tilePosition))
            {
                PlaceWall(tilemap.GetCellCenterWorld(tilePosition), tilePosition);
            }
            else
            {
                Debug.Log("Ÿ���� �ƴϹǷ� ���� ��ġ�� �� �����ϴ�.");
            }
        }
    }

    private void PlaceWall(Vector3 position, Vector3Int tilePosition)
    {
        if (placedWalls >= maxWalls) return;

        Instantiate(wallPrefab, position, Quaternion.identity);
        tilemap.SetTile(tilePosition, null); // Ÿ�ϸʿ��� �ش� Ÿ���� �����ϰų� ���� ����
        placedWalls++;

        if (enemySpawner != null)
        {
            var wallPositions = GetWallPositions();

        }

        if (placedWalls == maxWalls)
        {
            StartGame();
        }
    }
    private bool IsTileOccupied(Vector3Int tilePosition)
    {
        // ���� �̹� ��ġ�� Ÿ������ Ȯ��
        return tilemap.GetTile(tilePosition) == null;
    }
    private void StartGame()
    {

        Debug.Log("�� ��ġ �Ϸ�! ������ �����մϴ�.");
        gameStarted = true;

        if (enemySpawner != null)
        {
            enemySpawner.StartSpawner();
        }
        else
        {
            Debug.LogWarning("EnemySpawner�� �Ҵ���� �ʾҽ��ϴ�!");
        }
    }
    private HashSet<Vector3Int> GetWallPositions()
    {
        HashSet<Vector3Int> wallPositions = new HashSet<Vector3Int>();

        BoundsInt bounds = tilemap.cellBounds;
        foreach (var pos in bounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos) && tilemap.GetTile(pos) == null) // ���� ��ġ�� Ÿ�� Ȯ��
            {
                wallPositions.Add(pos);
            }
        }

        return wallPositions;
    }
    

    //private HashSet<Vector3Int> GetWallPositions(Tilemap tilemap)
    // {
    //   HashSet<Vector3Int> wallPositions = new HashSet<Vector3Int>();
    //
    // BoundsInt bounds = tilemap.cellBounds;
    // foreach (var pos in bounds.allPositionsWithin)
    // {
    //    if (tilemap.HasTile(pos)) // Ÿ���� �ִ��� Ȯ��
    //  {
    //    wallPositions.Add(pos);
    //}
    //}

    //        return wallPositions;
    //  }
}
