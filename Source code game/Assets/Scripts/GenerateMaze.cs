using Pathfinding;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GenerateMaze : MonoBehaviour
{
    [SerializeField]
    GameObject roomPrefab;

    GameObject roomsParent;

    [SerializeField]
    AIAgent pathfinding;

    [SerializeField]
    private bool isEngaging = false;

    // The grid.
    Room[,] rooms = null;


    [SerializeField]
    int numX = 10;
    [SerializeField]
    int numY = 10;

    // The room width and height.
    float roomWidth;
    float roomHeight;

    // The stack for backtracking.
    Stack<Room> stack = new Stack<Room>();

    bool generating = false;
    bool finished = false;

    // door generation
    [SerializeField]
    private Seeker startAISeeker;

    [SerializeField]
    private AIAgent finishAI;

    private float totalDistance;
    // public float spawnDistance;
    private bool isDistanceCalculated = false;
    public int spawnPercentage { get; private set; }

    [SerializeField]
    private int lowerPercentageLimit, upperPercentageLimit = 0;

    private Path _path;

    private void GetRoomSize()
    {
        SpriteRenderer[] spriteRenderers =
          roomPrefab.GetComponentsInChildren<SpriteRenderer>();

        Vector3 minBounds = Vector3.positiveInfinity;
        Vector3 maxBounds = Vector3.negativeInfinity;

        foreach (SpriteRenderer ren in spriteRenderers)
        {
            minBounds = Vector3.Min(
              minBounds,
              ren.bounds.min);

            maxBounds = Vector3.Max(
              maxBounds,
              ren.bounds.max);
        }

        roomWidth = maxBounds.x - minBounds.x;
        roomHeight = maxBounds.y - minBounds.y;
    }


    private void SetCamera()
    {
        Camera.main.transform.position = new Vector3(
          numX * (roomWidth - 1) / 2,
          numY * (roomHeight - 1) / 2,
          -100.0f);

        float min_value = Mathf.Min(numX * (roomWidth - 1), numY * (roomHeight - 1));
        Camera.main.orthographicSize = min_value * 0.75f;
    }

    private void Start()
    {
        GetRoomSize();

        rooms = new Room[numX, numY];
        roomsParent = new GameObject("Rooms");


        // Set seed 
        GameManager.Instance.GenerateSeed();

        for (int i = 0; i < numX; ++i)
        {
            for (int j = 0; j < numY; ++j)
            {
                GameObject room = Instantiate(roomPrefab,
                  new Vector3(i * roomWidth, j * roomHeight, 0.0f),
                  Quaternion.identity, roomsParent.transform);

                room.name = "Room_" + i.ToString() + "_" + j.ToString();
                rooms[i, j] = room.GetComponent<Room>();
                rooms[i, j].Index = new Vector2Int(i, j);
            }
        }

        roomsParent.transform.position = new Vector3((float)8.3, 0, 0);

        // SetCamera();
    }

    private void RemoveRoomWall(
      int x,
      int y,
      Room.Directions dir)
    {
        if (dir != Room.Directions.NONE)
        {
            rooms[x, y].SetDirFlag(dir, false);
        }

        Room.Directions opp = Room.Directions.NONE;
        switch (dir)
        {
            case Room.Directions.TOP:
                if (y < numY - 1)
                {
                    opp = Room.Directions.BOTTOM;
                    ++y;
                }
                break;
            case Room.Directions.RIGHT:
                if (x < numX - 1)
                {
                    opp = Room.Directions.LEFT;
                    ++x;
                }
                break;
            case Room.Directions.BOTTOM:
                if (y > 0)
                {
                    opp = Room.Directions.TOP;
                    --y;
                }
                break;
            case Room.Directions.LEFT:
                if (x > 0)
                {
                    opp = Room.Directions.RIGHT;
                    --x;
                }
                break;
        }
        if (opp != Room.Directions.NONE)
        {
            rooms[x, y].SetDirFlag(opp, false);
        }
    }

    public List<Tuple<Room.Directions, Room>> GetNeighboursNotVisited(
      int cx, int cy)
    {
        List<Tuple<Room.Directions, Room>> neighbours =
          new List<Tuple<Room.Directions, Room>>();

        foreach (Room.Directions dir in Enum.GetValues(
          typeof(Room.Directions)))
        {
            int x = cx;
            int y = cy;

            switch (dir)
            {
                case Room.Directions.TOP:
                    if (y < numY - 1)
                    {
                        ++y;
                        if (!rooms[x, y].visited)
                        {
                            neighbours.Add(new Tuple<Room.Directions, Room>(
                              Room.Directions.TOP,
                              rooms[x, y]));
                        }
                    }
                    break;
                case Room.Directions.RIGHT:
                    if (x < numX - 1)
                    {
                        ++x;
                        if (!rooms[x, y].visited)
                        {
                            neighbours.Add(new Tuple<Room.Directions, Room>(
                              Room.Directions.RIGHT,
                              rooms[x, y]));
                        }
                    }
                    break;
                case Room.Directions.BOTTOM:
                    if (y > 0)
                    {
                        --y;
                        if (!rooms[x, y].visited)
                        {
                            neighbours.Add(new Tuple<Room.Directions, Room>(
                              Room.Directions.BOTTOM,
                              rooms[x, y]));
                        }
                    }
                    break;
                case Room.Directions.LEFT:
                    if (x > 0)
                    {
                        --x;
                        if (!rooms[x, y].visited)
                        {
                            neighbours.Add(new Tuple<Room.Directions, Room>(
                              Room.Directions.LEFT,
                              rooms[x, y]));
                        }
                    }
                    break;
            }
        }
        return neighbours;
    }

    private bool GenerateStep()
    {
        if (stack.Count == 0) return true;

        Room r = stack.Peek();
        var neighbours = GetNeighboursNotVisited(r.Index.x, r.Index.y);

        if (neighbours.Count != 0)
        {
            var index = 0;
            if (neighbours.Count > 1)
            {
                index = UnityEngine.Random.Range(0, neighbours.Count);
            }

            var item = neighbours[index];
            Room neighbour = item.Item2;
            neighbour.visited = true;
            RemoveRoomWall(r.Index.x, r.Index.y, item.Item1);

            stack.Push(neighbour);
        }
        else
        {
            stack.Pop();
        }

        return false;
    }

    public void CreateMaze()
    {
        if (generating)
        {
            return;
        }

        Reset();

        RemoveRoomWall(0, 0, Room.Directions.BOTTOM);

        RemoveRoomWall(numX - 1, numY - 1, Room.Directions.RIGHT);

        rooms[0, 0].visited = true; // prevent creating multiple paths from start to exit
        stack.Push(rooms[0, 0]);

        // StartCoroutine(Coroutine_Generate());
        Generate();

        AstarPath.active.Scan();

        //Wait for path to be ready, then genenrate door
        startAISeeker.StartPath(
            startAISeeker.transform.position,
            finishAI.transform.position,
            OnFinishPathComplete
        );
    }
    private void OnFinishPathComplete(Path p)
    {
        Debug.Log($"Start pos: {startAISeeker.transform.position}");
        Debug.Log($"End pos: {finishAI.transform.position}");
        Debug.Log($"Path node count: {p.path?.Count}");

        if (p.error)
        {
            Debug.LogError("Path error: " + p.errorLog);
            return;
        }

        _path = p;
        CheckTotalDistance();
        GenerateDoor(p);
        // StartCoroutine(ABC(p));
    }

    //Gets ordered list of room objects based on Astar path to maze exit
    private List<Room> GetOrderedRoomPath(Path path)
    {
        //var currentPath = startAISeeker.GetCurrentPath(); 

        Debug.Log($"Current path: {path}");
        Debug.Log($"Current path nodes: {path?.path?.Count}");

        // Return empty list if path isn't calculated yet
        if (path == null || path.path == null)
            return new List<Room>();

        //List<GraphNode> nodes = currentPath.path;
        List<Room> roomPath = new List<Room>();

        foreach (GraphNode node in path.path)
        {
            Room room = GetRoomAtNode(node);
            if (room != null && !roomPath.Contains(room)) // avoid duplicates{
            {
                roomPath.Add(room);
            }
        }

        // roomPath.Reverse(); // now ordered furthest from goal → closest to goal
        return roomPath;
    }

    // Get room object that overlaps with graphnode
    private Room GetRoomAtNode(GraphNode node)
    {
        float roomsize = 10f;

        Vector3 nodePos = (Vector3)node.position;
        Room roomatNode = null;
        foreach (Room room in rooms)
        {
            float distance = math.distance(nodePos, room.transform.position);
            if (distance < roomsize)
            {
                roomsize = distance;
                roomatNode = room;
            }
        }

        if (roomatNode == null)
        {
            Debug.LogError($"[GenerateMaze.cs] GetRoomAtNode No room found near node at position {nodePos}");
        }

        Debug.Log($"Found suitable room for door placement at position: {roomatNode.transform.position}");

        return roomatNode;
    }

    void GenerateDoor(Path calculatedPath)
    {
        if (GameManager.Instance.GetCurrentGameState() == "Practice") { return; } //! TODO 
        Debug.Log("Gen Door function reached");

        List<Room> rooms = GetOrderedRoomPath(calculatedPath);

        // Guard against empty or null path
        if (rooms == null || rooms.Count == 0)
        {
            Debug.LogWarning("DoorController: Room path is empty, cannot generate door.");
            return;
        }

        // pick a random room inbetween the percentage range
        int percentage = GetSpawnPercentage();
        int roomIdx = Mathf.Clamp(
            (int)Math.Floor((double)(percentage * rooms.Count) / 100),
            0,
            rooms.Count - 1
        );
        Room room = rooms[roomIdx];
        SetDoorDirection(room, rooms[roomIdx - 1]); //Set entrance door (the door between the room before and the room)
        SetDoorDirection(room, rooms[roomIdx + 1]); //Set exit door (the door between the room after and the room)

        //NOTE Amber: Don't think you will need this anymore as now only the entrance and exit door get spawned
        // set doors active (//! temporarily ALL doors TODO: only 2 correct doors)
        /*  foreach (Room.Directions dir in Enum.GetValues(
            typeof(Room.Directions)))
          {
              if (dir != Room.Directions.NONE)
              {
                  room.SetDirFlag(dir, true, "door");
              }
          }*/
    }

    private void SetDoorDirection(Room room, Room otherRoom)
    {
        Vector2 roomPosition = room.transform.position;
        Vector2 otherRoomPosition = otherRoom.transform.position;
        Vector2 direction = (otherRoomPosition - roomPosition).normalized;

        Room.Directions roomDirection = direction switch
        {
            { x: 1, y: 0 } => Room.Directions.RIGHT,
            { x: -1, y: 0 } => Room.Directions.LEFT,
            { x: 0, y: 1 } => Room.Directions.TOP,
            { x: 0, y: -1 } => Room.Directions.BOTTOM,
            _ => Room.Directions.NONE
        };

        if (roomDirection == Room.Directions.NONE)
        {
            Debug.LogWarning($"GetDoorDirection direction of door between room {room.name} and {otherRoom.name} not found");
            return;
        }

        room.SetDirFlag(roomDirection, true, "door");
    }

    private int GetSpawnPercentage()
    {
        return UnityEngine.Random.Range(lowerPercentageLimit, upperPercentageLimit);
    }

    private int maxTries = 50;
    // IEnumerator CheckTotalDistanceCoroutine()
    private void CheckTotalDistance()
    {
        int attempts = 0;
        float distance = 0;

        if (finishAI.path == null)
        {
            Debug.Log("Condition not met yet.");
            // attempts++;
            // yield return new WaitForSecondsRealtime(0.05f);
        }
        distance = finishAI.path.remainingDistance;

        if (distance > 50f && !float.IsInfinity(distance))
        {
            totalDistance = distance;
            // isDistanceCalculated = true;
        }
        spawnPercentage = GetSpawnPercentage();
        Debug.Log("Total distance has been checked");
    }

    void Generate()
    {
        generating = true;
        bool flag = false;
        while (!flag)
        {
            flag = GenerateStep();
        }

        generating = false;
        finished = true;
    }

    private void Reset()
    {
        for (int i = 0; i < numX; ++i)
        {
            for (int j = 0; j < numY; ++j)
            {
                rooms[i, j].SetDirFlag(Room.Directions.TOP, true);
                rooms[i, j].SetDirFlag(Room.Directions.RIGHT, true);
                rooms[i, j].SetDirFlag(Room.Directions.BOTTOM, true);
                rooms[i, j].SetDirFlag(Room.Directions.LEFT, true);
                rooms[i, j].visited = false;
            }
        }
    }

    private void Update()
    {
        if (!generating && !finished)
        {
            // Create maze for round 1
            // CreateMaze();
            GameManager.Instance.NextRound(true, isEngaging, true);
        }
    }
}
