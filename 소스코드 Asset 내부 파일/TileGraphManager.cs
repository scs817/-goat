using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TileGraphManager : MonoBehaviour
{
    public Tilemap tilemap; // 타일맵을 연결
    public string tileTag = "Tile"; // 기본 타일 태그
    public string wallTag = "Wall"; // 벽 타일 태그

    private HashSet<Vector3Int> walkableTiles;  // 이동 가능한 타일
    public Dictionary<Vector3Int, int> tileToIndex; // 타일 -> 인덱스 매핑
    private int[,] graph; // 인접 행렬 그래프

    void Start()
    {
        // 이동 가능한 타일 추출
        walkableTiles = GetWalkableTiles();

        // 그래프 생성
        graph = CreateGraph(walkableTiles);

        Debug.Log("그래프 생성 완료");
    }

    // 이동 가능한 타일 추출
    private HashSet<Vector3Int> GetWalkableTiles()
    {
        HashSet<Vector3Int> tiles = new HashSet<Vector3Int>();
        BoundsInt bounds = tilemap.cellBounds;

        foreach (var pos in bounds.allPositionsWithin)
        {
            // 타일 태그 확인
            if (tilemap.HasTile(pos) && tilemap.GetInstantiatedObject(pos)?.CompareTag(tileTag) == true)
            {
                tiles.Add(pos);
            }
        }

        // 벽 태그를 가진 타일 제거
        foreach (var pos in bounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos) && tilemap.GetInstantiatedObject(pos)?.CompareTag(wallTag) == true)
            {
                tiles.Remove(pos);
            }
        }

        return tiles;
    }

    // 그래프 생성 (인접 행렬)
    private int[,] CreateGraph(HashSet<Vector3Int> tiles)
    {
        int tileCount = tiles.Count;
        int[,] graph = new int[tileCount, tileCount];

        tileToIndex = new Dictionary<Vector3Int, int>();
        int index = 0;

        foreach (var tile in tiles)
        {
            tileToIndex[tile] = index++;
        }

        foreach (var tile in tiles)
        {
            Vector3Int[] neighbors = GetNeighbors(tile);

            foreach (var neighbor in neighbors)
            {
                if (tiles.Contains(neighbor))
                {
                    int from = tileToIndex[tile];
                    int to = tileToIndex[neighbor];
                    graph[from, to] = 1; // 가중치: 1
                }
            }
        }

        return graph;
    }

    // 상하좌우 이웃 타일 반환
    private Vector3Int[] GetNeighbors(Vector3Int tile)
    {
        return new Vector3Int[]
        {
            tile + new Vector3Int(1, 0, 0),  // 오른쪽
            tile + new Vector3Int(-1, 0, 0), // 왼쪽
            tile + new Vector3Int(0, 1, 0),  // 위
            tile + new Vector3Int(0, -1, 0)  // 아래
        };
    }

    // 그래프 반환
    public int[,] GetGraph()
    {
        return graph;
    }

    // 타일 인덱스 반환
    public int GetTileIndex(Vector3Int tile)
    {
        return tileToIndex.ContainsKey(tile) ? tileToIndex[tile] : -1;
    }
}
