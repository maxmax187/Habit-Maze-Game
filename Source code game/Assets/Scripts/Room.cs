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

    walls[Directions.TOP] = topDoor;
    walls[Directions.RIGHT] = rightDoor;
    walls[Directions.BOTTOM] = bottomDoor;
    walls[Directions.LEFT] = leftDoor;
  }

  private void SetActive(Directions dir, bool flag)
  {
    walls[dir].SetActive(flag);
  }

  public void SetDirFlag(Directions dir, bool flag)
  {
    dirflags[dir] = flag;
    SetActive(dir, flag);
  }
}
