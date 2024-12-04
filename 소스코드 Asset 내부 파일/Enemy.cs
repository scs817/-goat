using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum EnemyDestroyType { Kill = 0, Arrive }

public class Enemy : MonoBehaviour
{
    private int wayPointCount;
    private Transform[] wayPoints;
    private int currentIndex = 0;
    private Movement2d movement2D;
    private EnemySpawner enemySpawner;
    private Tilemap tilemap;
    private List<Vector3Int> path;

    [SerializeField]
    private float rotationSpeed = 10f;
    [SerializeField]
    private int gold = 10;

    public void Setup(EnemySpawner enemySpawner, Tilemap tilemap, Vector3Int start, Vector3Int end)
    {
        movement2D = GetComponent<Movement2d>();
        this.enemySpawner = enemySpawner;
        this.tilemap = tilemap;

        // 다익스트라 알고리즘을 사용하여 경로 계산
        path = DijkstraAlgorithm(start, end);

        if (path.Count > 0)
        {
            transform.position = tilemap.CellToWorld(path[currentIndex]);
            StartCoroutine(OnMove());
        }
        else
        {
            // 경로가 없다면 적을 즉시 제거하거나 다른 처리를 할 수 있습니다.
            OnDie(EnemyDestroyType.Kill);
        }
    }

    private IEnumerator OnMove()
    {
        while (currentIndex < path.Count)
        {
            Vector3 targetPosition = tilemap.CellToWorld(path[currentIndex]);

            // 목표 지점으로 회전
            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);

            // 목표 지점으로 이동
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                movement2D.MoveTo(direction);
                yield return null;
            }

            // 목표에 도달하면 다음 웨이포인트로 이동
            currentIndex++;
        }

        // 목적지에 도달하면
        gold = 0;
        OnDie(EnemyDestroyType.Arrive);
    }

    public void OnDie(EnemyDestroyType type)
    {
        enemySpawner.DestroyEnemy(type, this, gold);
    }

    // Dijkstra 알고리즘을 사용하여 경로를 계산하는 메서드
    private List<Vector3Int> DijkstraAlgorithm(Vector3Int start, Vector3Int end)
    {
        Dictionary<Vector3Int, float> distances = new Dictionary<Vector3Int, float>();
        Dictionary<Vector3Int, Vector3Int> previousNodes = new Dictionary<Vector3Int, Vector3Int>();
        List<Vector3Int> unvisitedNodes = new List<Vector3Int>();

        // 모든 타일을 미방문 노드로 설정하고, 시작 노드는 0으로 설정
        foreach (var position in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.HasTile(position))
            {
                unvisitedNodes.Add(position);
                distances[position] = float.MaxValue;
            }
        }

        distances[start] = 0;

        while (unvisitedNodes.Count > 0)
        {
            // 가장 작은 거리 값을 가지는 노드 선택
            Vector3Int currentNode = GetNodeWithSmallestDistance(unvisitedNodes, distances);

            unvisitedNodes.Remove(currentNode);

            // 현재 노드에서 인접한 노드를 탐색
            if (currentNode == end)
                break;

            foreach (var neighbor in GetNeighbors(currentNode))
            {
                if (!unvisitedNodes.Contains(neighbor)) continue;

                float tentativeDistance = distances[currentNode] + 1; // 인접 노드까지의 거리는 1로 설정

                if (tentativeDistance < distances[neighbor])
                {
                    distances[neighbor] = tentativeDistance;
                    previousNodes[neighbor] = currentNode;
                }
            }
        }

        // 경로 구성
        List<Vector3Int> path = new List<Vector3Int>();
        Vector3Int current = end;
        while (previousNodes.ContainsKey(current))
        {
            path.Insert(0, current);
            current = previousNodes[current];
        }

        return path;
    }

    // 가장 작은 거리를 가진 노드를 찾는 메서드
    private Vector3Int GetNodeWithSmallestDistance(List<Vector3Int> unvisitedNodes, Dictionary<Vector3Int, float> distances)
    {
        Vector3Int smallestNode = unvisitedNodes[0];
        foreach (var node in unvisitedNodes)
        {
            if (distances[node] < distances[smallestNode])
            {
                smallestNode = node;
            }
        }
        return smallestNode;
    }

    // 인접한 노드를 반환하는 메서드 (상, 하, 좌, 우)
    private List<Vector3Int> GetNeighbors(Vector3Int node)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>
        {
            new Vector3Int(node.x + 1, node.y, node.z),
            new Vector3Int(node.x - 1, node.y, node.z),
            new Vector3Int(node.x, node.y + 1, node.z),
            new Vector3Int(node.x, node.y - 1, node.z)
        };

        // 타일맵 내에 있는 인접 노드만 반환
        neighbors.RemoveAll(neighbor => !tilemap.HasTile(neighbor));

        return neighbors;
    }
}
