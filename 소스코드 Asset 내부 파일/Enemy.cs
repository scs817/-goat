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

        // ���ͽ�Ʈ�� �˰����� ����Ͽ� ��� ���
        path = DijkstraAlgorithm(start, end);

        if (path.Count > 0)
        {
            transform.position = tilemap.CellToWorld(path[currentIndex]);
            StartCoroutine(OnMove());
        }
        else
        {
            // ��ΰ� ���ٸ� ���� ��� �����ϰų� �ٸ� ó���� �� �� �ֽ��ϴ�.
            OnDie(EnemyDestroyType.Kill);
        }
    }

    private IEnumerator OnMove()
    {
        while (currentIndex < path.Count)
        {
            Vector3 targetPosition = tilemap.CellToWorld(path[currentIndex]);

            // ��ǥ �������� ȸ��
            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);

            // ��ǥ �������� �̵�
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                movement2D.MoveTo(direction);
                yield return null;
            }

            // ��ǥ�� �����ϸ� ���� ��������Ʈ�� �̵�
            currentIndex++;
        }

        // �������� �����ϸ�
        gold = 0;
        OnDie(EnemyDestroyType.Arrive);
    }

    public void OnDie(EnemyDestroyType type)
    {
        enemySpawner.DestroyEnemy(type, this, gold);
    }

    // Dijkstra �˰����� ����Ͽ� ��θ� ����ϴ� �޼���
    private List<Vector3Int> DijkstraAlgorithm(Vector3Int start, Vector3Int end)
    {
        Dictionary<Vector3Int, float> distances = new Dictionary<Vector3Int, float>();
        Dictionary<Vector3Int, Vector3Int> previousNodes = new Dictionary<Vector3Int, Vector3Int>();
        List<Vector3Int> unvisitedNodes = new List<Vector3Int>();

        // ��� Ÿ���� �̹湮 ���� �����ϰ�, ���� ���� 0���� ����
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
            // ���� ���� �Ÿ� ���� ������ ��� ����
            Vector3Int currentNode = GetNodeWithSmallestDistance(unvisitedNodes, distances);

            unvisitedNodes.Remove(currentNode);

            // ���� ��忡�� ������ ��带 Ž��
            if (currentNode == end)
                break;

            foreach (var neighbor in GetNeighbors(currentNode))
            {
                if (!unvisitedNodes.Contains(neighbor)) continue;

                float tentativeDistance = distances[currentNode] + 1; // ���� �������� �Ÿ��� 1�� ����

                if (tentativeDistance < distances[neighbor])
                {
                    distances[neighbor] = tentativeDistance;
                    previousNodes[neighbor] = currentNode;
                }
            }
        }

        // ��� ����
        List<Vector3Int> path = new List<Vector3Int>();
        Vector3Int current = end;
        while (previousNodes.ContainsKey(current))
        {
            path.Insert(0, current);
            current = previousNodes[current];
        }

        return path;
    }

    // ���� ���� �Ÿ��� ���� ��带 ã�� �޼���
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

    // ������ ��带 ��ȯ�ϴ� �޼��� (��, ��, ��, ��)
    private List<Vector3Int> GetNeighbors(Vector3Int node)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>
        {
            new Vector3Int(node.x + 1, node.y, node.z),
            new Vector3Int(node.x - 1, node.y, node.z),
            new Vector3Int(node.x, node.y + 1, node.z),
            new Vector3Int(node.x, node.y - 1, node.z)
        };

        // Ÿ�ϸ� ���� �ִ� ���� ��常 ��ȯ
        neighbors.RemoveAll(neighbor => !tilemap.HasTile(neighbor));

        return neighbors;
    }
}
