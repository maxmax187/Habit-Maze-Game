using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public enum Directions
    {
        TOP,
        RIGHT,
        BOTTOM,
        LEFT,
        NONE,
    }

    //walls
    [SerializeField]
    GameObject topWall;
    [SerializeField]
    GameObject rightWall;
    [SerializeField]
    GameObject bottomWall;
    [SerializeField]
    GameObject leftWall;

    //doors
    [SerializeField]
    GameObject topDoor;
    [SerializeField]
    GameObject rightDoor;
    [SerializeField]
    GameObject bottomDoor;
    [SerializeField]
    GameObject leftDoor;

    Dictionary<Directions, GameObject> walls =
      new Dictionary<Directions, GameObject>();
    Dictionary<Directions, GameObject> doors =
      new Dictionary<Directions, GameObject>();

    public Vector2Int Index
    {
        get;
        set;
    }

    public bool visited { get; set; } = false;

    Dictionary<Directions, bool> dirflags =
      new Dictionary<Directions, bool>();

    private void Start()
    {
        walls[Directions.TOP] = topWall;
        walls[Directions.RIGHT] = rightWall;
        walls[Directions.BOTTOM] = bottomWall;
        walls[Directions.LEFT] = leftWall;

        doors[Directions.TOP] = topDoor;
        doors[Directions.RIGHT] = rightDoor;
        doors[Directions.BOTTOM] = bottomDoor;
        doors[Directions.LEFT] = leftDoor;
    }

    private void SetActive(Directions dir, bool flag, string type = "wall")
    {
        if (type == "wall")
        {
            walls[dir].SetActive(flag);
        }
        else
        {
            doors[dir].SetActive(flag);
        }

    }

    public void SetDirFlag(Directions dir, bool flag, string type = "wall")
    {
        dirflags[dir] = flag;
        SetActive(dir, flag, type);
    }

    // Returns doors of this room that are currently active, alongside their directions
    // (should always return 2 doors during normal gameplay)
    public Dictionary<Directions, GameObject> GetActiveDoors()
    {
        Dictionary<Directions, GameObject> activeDoors = new Dictionary<Directions, GameObject>();
        foreach (var kvp in doors)
        {
            if (kvp.Value != null && kvp.Value.activeSelf)
                activeDoors.Add(kvp.Key, kvp.Value);
        }
        return activeDoors;
    }

    public Dictionary<Directions, GameObject> GetAllDoors()
    {
        return new Dictionary<Directions, GameObject>(doors);
    }

    public int GetActiveWallCount()
    {
        int count = 0;
        foreach (var kvp in walls)
            if (kvp.Value != null && kvp.Value.activeSelf) count++;
        return count;
    }
}
