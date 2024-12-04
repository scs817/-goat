using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphManager : MonoBehaviour
{
    public int gridSize = 16; // 16x16 ����
    private int[,] graph;

    private void Start()
    {
        InitializeGraph();
    }

    // �׷��� �ʱ�ȭ
    private void InitializeGraph()
    {
        int totalNodes = gridSize * gridSize;
        graph = new int[totalNodes, totalNodes];

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                int currentNode = GetNodeIndex(i, j);

                // �����¿� ����
                ConnectNodes(currentNode, i - 1, j); // ����
                ConnectNodes(currentNode, i + 1, j); // �Ʒ���
                ConnectNodes(currentNode, i, j - 1); // ����
                ConnectNodes(currentNode, i, j + 1); // ������
            }
        }
    }

    // Ư�� ��带 �ٸ� ���� ����
    private void ConnectNodes(int currentNode, int x, int y)
    {
        if (x >= 0 && x < gridSize && y >= 0 && y < gridSize)
        {
            int neighborNode = GetNodeIndex(x, y);
            graph[currentNode, neighborNode] = 1; // �⺻ ����ġ 1
        }
    }

    // 2D ��ǥ�� 1D ��� �ε����� ��ȯ
    private int GetNodeIndex(int x, int y)
    {
        return x * gridSize + y;
    }

    // Ư�� ��ġ�� ��ֹ� �߰� (����ġ ���Ѵ� ����)
    public void AddObstacle(Vector2Int position)
    {
        int obstacleNode = GetNodeIndex(position.x, position.y);

        for (int i = 0; i < graph.GetLength(0); i++)
        {
            graph[obstacleNode, i] = int.MaxValue;
            graph[i, obstacleNode] = int.MaxValue;
        }

        Debug.Log($"��ֹ� �߰�: ({position.x}, {position.y})");
    }

    // �׷��� ��ȯ (���ͽ�Ʈ�� ���޿�)
    public int[,] GetGraph()
    {
        return graph;
    }
}

