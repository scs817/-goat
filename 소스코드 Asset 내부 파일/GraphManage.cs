using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphManager : MonoBehaviour
{
    public int gridSize = 16; // 16x16 격자
    private int[,] graph;

    private void Start()
    {
        InitializeGraph();
    }

    // 그래프 초기화
    private void InitializeGraph()
    {
        int totalNodes = gridSize * gridSize;
        graph = new int[totalNodes, totalNodes];

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                int currentNode = GetNodeIndex(i, j);

                // 상하좌우 연결
                ConnectNodes(currentNode, i - 1, j); // 위쪽
                ConnectNodes(currentNode, i + 1, j); // 아래쪽
                ConnectNodes(currentNode, i, j - 1); // 왼쪽
                ConnectNodes(currentNode, i, j + 1); // 오른쪽
            }
        }
    }

    // 특정 노드를 다른 노드와 연결
    private void ConnectNodes(int currentNode, int x, int y)
    {
        if (x >= 0 && x < gridSize && y >= 0 && y < gridSize)
        {
            int neighborNode = GetNodeIndex(x, y);
            graph[currentNode, neighborNode] = 1; // 기본 가중치 1
        }
    }

    // 2D 좌표를 1D 노드 인덱스로 변환
    private int GetNodeIndex(int x, int y)
    {
        return x * gridSize + y;
    }

    // 특정 위치에 장애물 추가 (가중치 무한대 설정)
    public void AddObstacle(Vector2Int position)
    {
        int obstacleNode = GetNodeIndex(position.x, position.y);

        for (int i = 0; i < graph.GetLength(0); i++)
        {
            graph[obstacleNode, i] = int.MaxValue;
            graph[i, obstacleNode] = int.MaxValue;
        }

        Debug.Log($"장애물 추가: ({position.x}, {position.y})");
    }

    // 그래프 반환 (다익스트라에 전달용)
    public int[,] GetGraph()
    {
        return graph;
    }
}

