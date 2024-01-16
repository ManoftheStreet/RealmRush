using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    [SerializeField] Vector2Int startCoordinates;//���� ��� ��ǥ
    public Vector2Int StartCoorinates { get { return startCoordinates; } }

    [SerializeField] Vector2Int destinationCoordinates;//������ ��� ��ǥ
    public Vector2Int DestinationCoordinates { get { return destinationCoordinates; } }

    Node startNode;//���۳��
    Node destinationNode;//������ ���
    Node currentSearchNode;//���� ã�� ���


    Queue<Node> frontier = new Queue<Node>(); //���� ��ġ(���) ť (�Ʒ� ExploreNeighbors ���� ã�� ������� ��带 ť�� �־���)
    Dictionary<Vector2Int, Node> reached = new Dictionary<Vector2Int, Node>(); //�̹� ã�� ����� ��ųʸ�

    Vector2Int[] directions = { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };//4���� �迭 (C��� �˰���)

    GridManager gridManager;//�׸��� �Ŵ��� (�� ����� ũ�Ⱑ �����Ǿ� ����)
    Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();//�׸��� ��ųʸ�

    void Awake()
    {
        gridManager = FindAnyObjectByType<GridManager>();//�׸��� �Ŵ��� ������Ʈ�� ã��
        if(gridManager != null )//�׸��� ������Ʈ �Ŵ����� �ִٸ�
        {
            grid = gridManager.Grid;//�׸��� �Ŵ����� �׸��带 �н����δ� �׸��忡 ����
            startNode = grid[startCoordinates];
            destinationNode = grid[destinationCoordinates];
        }
    }

    void Start()
    {
        GetNewPath();
    }

    public List<Node> GetNewPath()
    {
        gridManager.ResetNode();
        BreadthFerstSearch();
        return BuildPath();
    }

    void BreadthFerstSearch()
    {
        startNode.isWalkable = true;
        destinationNode.isWalkable = true;

        frontier.Clear();
        reached.Clear();

        bool isRunning = true;//���� ���������� Ȯ���ϴ� ���� ����

        frontier.Enqueue(startNode);//���� ��迡 ���� ��带 �ְ�
        reached.Add(startCoordinates, startNode);//�̹� ã�� ���۳���� ��ǥ�� ��带 Ű�� ������ ����

        while (frontier.Count > 0 && isRunning)//ť�� ���� �ְ� �������̸�
        {
            currentSearchNode = frontier.Dequeue();//���� ã�� ��忡 ����Ǿ� �ִ� ť�� ����
            currentSearchNode.isExplored = true;//�׳�带 Ž�谡������ �����ϰ�
            ExploreNeighbors();//����Ž���޼��� ����

            if (currentSearchNode.coordinate == destinationCoordinates)//���� ���� ����� ��ǥ�� ������ �����ǥ�� 
            {
                isRunning = false;//��������
            }
        }
    }

    void ExploreNeighbors()
    {
        List<Node> neighbors = new List<Node>();//���ο� �α� ���� �����ϱ� ���� ����Ʈ

        foreach(Vector2Int direction in directions)//�� ���⸶�� ����
        {
            Vector2Int neighborCords = currentSearchNode.coordinate + direction;//���� ��忡�� ���⺰�� ������ ������� �̵�

            if (grid.ContainsKey(neighborCords))//������ �̵��� ��ǥ�� ���� �ִٸ�
            {
                neighbors.Add(grid[neighborCords]);//�� ��ǥ���� �α� ��� ����Ʈ�� �߰�
            }
        }

        foreach(Node neighbor in neighbors)//������尡 ��� ����Ǹ� �� ��帶��
        {
            if(!reached.ContainsKey(neighbor.coordinate) && neighbor.isWalkable)//�̹� ã�� ��� ��ųʸ��� ��ǥ�� ���� ��尡 ���� �ִٸ�
            {
                neighbor.connectedTo = currentSearchNode;
                reached.Add(neighbor.coordinate, neighbor);//�̹� ã�� ����ųʸ��� �����ϰ�
                frontier.Enqueue(neighbor);//���� ����忡 ť�� ����
            }
        }
    }
    
    List<Node> BuildPath()
    {
        List<Node> path = new List<Node>();
        Node currentNode = destinationNode;

        path.Add(currentNode);
        currentNode.isPath = true;

        while(currentNode.connectedTo != null)
        {
            currentNode = currentNode.connectedTo;
            path.Add(currentNode);
            currentNode.isPath = true;
        }

        path.Reverse();

        return path;
    }

    public bool WillBlockPath(Vector2Int coordinates)
    {
        if (grid.ContainsKey(coordinates))
        {
            bool previousState = grid[coordinates].isWalkable;

            grid[coordinates].isWalkable = false;
            List<Node> newPath = GetNewPath();
            grid[coordinates].isWalkable = previousState;

            if(newPath.Count <= 1)
            {
                GetNewPath();
                return true;
            }
        }

        return false;
    }

    public void NotifyReceivers()
    {
        BroadcastMessage("RecalculatePath", SendMessageOptions.DontRequireReceiver);
    }
}
