using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    [SerializeField] Vector2Int startCoordinates;//시작 노드 좌표
    public Vector2Int StartCoorinates { get { return startCoordinates; } }

    [SerializeField] Vector2Int destinationCoordinates;//목적지 노드 좌표
    public Vector2Int DestinationCoordinates { get { return destinationCoordinates; } }

    Node startNode;//시작노드
    Node destinationNode;//목적지 노드
    Node currentSearchNode;//현재 찾은 노드


    Queue<Node> frontier = new Queue<Node>(); //현재 위치(경계) 큐 (아래 ExploreNeighbors 에서 찾은 순서대로 노드를 큐에 넣어줌)
    Dictionary<Vector2Int, Node> reached = new Dictionary<Vector2Int, Node>(); //이미 찾은 노드의 딕셔너리

    Vector2Int[] directions = { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };//4방향 배열 (C언어 알고리즘)

    GridManager gridManager;//그리드 매니저 (총 노드의 크기가 설정되어 있음)
    Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();//그리드 딕셔너리

    void Awake()
    {
        gridManager = FindAnyObjectByType<GridManager>();//그리드 매니저 오브젝트를 찾고
        if(gridManager != null )//그리드 오브젝트 매니저가 있다면
        {
            grid = gridManager.Grid;//그리드 매니저의 그리드를 패스파인더 그리드에 저장
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

        bool isRunning = true;//현재 운행중인지 확인하는 조건 변수

        frontier.Enqueue(startNode);//현재 경계에 시작 노드를 넣고
        reached.Add(startCoordinates, startNode);//이미 찾은 시작노드의 좌표와 노드를 키와 값으로 연결

        while (frontier.Count > 0 && isRunning)//큐에 뭐가 있고 운행중이면
        {
            currentSearchNode = frontier.Dequeue();//현재 찾은 노드에 저장되어 있는 큐로 저장
            currentSearchNode.isExplored = true;//그노드를 탐험가능으로 설정하고
            ExploreNeighbors();//주위탐색메서드 실행

            if (currentSearchNode.coordinate == destinationCoordinates)//만일 현재 노드의 좌표가 목적지 노드좌표면 
            {
                isRunning = false;//운행종료
            }
        }
    }

    void ExploreNeighbors()
    {
        List<Node> neighbors = new List<Node>();//새로운 인근 노드들 저장하기 위한 리스트

        foreach(Vector2Int direction in directions)//각 방향마다 실행
        {
            Vector2Int neighborCords = currentSearchNode.coordinate + direction;//현재 노드에서 방향별로 설정한 순서대로 이동

            if (grid.ContainsKey(neighborCords))//주위에 이동한 좌표의 값이 있다면
            {
                neighbors.Add(grid[neighborCords]);//그 좌표들을 인근 노드 리스트에 추가
            }
        }

        foreach(Node neighbor in neighbors)//주위노드가 모두 저장되면 각 노드마다
        {
            if(!reached.ContainsKey(neighbor.coordinate) && neighbor.isWalkable)//이미 찾은 노드 딕셔너리에 좌표가 없고 노드가 갈수 있다면
            {
                neighbor.connectedTo = currentSearchNode;
                reached.Add(neighbor.coordinate, neighbor);//이미 찾은 노드딕셔너리에 저당하고
                frontier.Enqueue(neighbor);//현재 끝노드에 큐로 저장
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
