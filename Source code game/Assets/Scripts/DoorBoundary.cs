using UnityEngine;
/// <summary>
/// physical boundary for a room's doors. Enable its box collider to prevent user walking through doors when locked.
/// Disabled by default
/// </summary>
public class DoorBoundary : MonoBehaviour
{
    private BoxCollider2D col;

    private void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        col.enabled = false;
    }

    public void Enable() => col.enabled = true;
    public void Disable() => col.enabled = false;

}