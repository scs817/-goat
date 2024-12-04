using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class WallPlacementManager : MonoBehaviour
{
    public GameObject wallPrefab; // 벽 프리팹
    public Tilemap tilemap; // 타일맵 객체
    public int maxWalls = 8; // 설치 가능한 벽의 최대 개수
    private int placedWalls = 0; // 현재 설치된 벽의 개수
    private bool gameStarted = false; // 게임 시작 여부
    public EnemySpawner enemySpawner; // 적 스포너 참조

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
                Debug.Log("타일이 아니므로 벽을 설치할 수 없습니다.");
            }
        }
    }

    private void PlaceWall(Vector3 position, Vector3Int tilePosition)
    {
        if (placedWalls >= maxWalls) return;

        Instantiate(wallPrefab, position, Quaternion.identity);
        tilemap.SetTile(tilePosition, null); // 타일맵에서 해당 타일을 제거하거나 변경 가능
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
        // 벽이 이미 설치된 타일인지 확인
        return tilemap.GetTile(tilePosition) == null;
    }
    private void StartGame()
    {

        Debug.Log("벽 설치 완료! 게임을 시작합니다.");
        gameStarted = true;

        if (enemySpawner != null)
        {
            enemySpawner.StartSpawner();
        }
        else
        {
            Debug.LogWarning("EnemySpawner가 할당되지 않았습니다!");
        }
    }
    private HashSet<Vector3Int> GetWallPositions()
    {
        HashSet<Vector3Int> wallPositions = new HashSet<Vector3Int>();

        BoundsInt bounds = tilemap.cellBounds;
        foreach (var pos in bounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos) && tilemap.GetTile(pos) == null) // 벽이 설치된 타일 확인
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
    //    if (tilemap.HasTile(pos)) // 타일이 있는지 확인
    //  {
    //    wallPositions.Add(pos);
    //}
    //}

    //        return wallPositions;
    //  }
}
