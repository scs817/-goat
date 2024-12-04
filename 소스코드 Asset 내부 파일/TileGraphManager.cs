using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TileGraphManager : MonoBehaviour
{
    public Tilemap tilemap; // Ÿ�ϸ��� ����
    public string tileTag = "Tile"; // �⺻ Ÿ�� �±�
    public string wallTag = "Wall"; // �� Ÿ�� �±�

    private HashSet<Vector3Int> walkableTiles;  // �̵� ������ Ÿ��
    public Dictionary<Vector3Int, int> tileToIndex; // Ÿ�� -> �ε��� ����
    private int[,] graph; // ���� ��� �׷���

    void Start()
    {
        // �̵� ������ Ÿ�� ����
        walkableTiles = GetWalkableTiles();

        // �׷��� ����
        graph = CreateGraph(walkableTiles);

        Debug.Log("�׷��� ���� �Ϸ�");
    }

    // �̵� ������ Ÿ�� ����
    private HashSet<Vector3Int> GetWalkableTiles()
    {
        HashSet<Vector3Int> tiles = new HashSet<Vector3Int>();
        BoundsInt bounds = tilemap.cellBounds;

        foreach (var pos in bounds.allPositionsWithin)
        {
            // Ÿ�� �±� Ȯ��
            if (tilemap.HasTile(pos) && tilemap.GetInstantiatedObject(pos)?.CompareTag(tileTag) == true)
            {
                tiles.Add(pos);
            }
        }

        // �� �±׸� ���� Ÿ�� ����
        foreach (var pos in bounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos) && tilemap.GetInstantiatedObject(pos)?.CompareTag(wallTag) == true)
            {
                tiles.Remove(pos);
            }
        }

        return tiles;
    }

    // �׷��� ���� (���� ���)
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
                    graph[from, to] = 1; // ����ġ: 1
                }
            }
        }

        return graph;
    }

    // �����¿� �̿� Ÿ�� ��ȯ
    private Vector3Int[] GetNeighbors(Vector3Int tile)
    {
        return new Vector3Int[]
        {
            tile + new Vector3Int(1, 0, 0),  // ������
            tile + new Vector3Int(-1, 0, 0), // ����
            tile + new Vector3Int(0, 1, 0),  // ��
            tile + new Vector3Int(0, -1, 0)  // �Ʒ�
        };
    }

    // �׷��� ��ȯ
    public int[,] GetGraph()
    {
        return graph;
    }

    // Ÿ�� �ε��� ��ȯ
    public int GetTileIndex(Vector3Int tile)
    {
        return tileToIndex.ContainsKey(tile) ? tileToIndex[tile] : -1;
    }
}
