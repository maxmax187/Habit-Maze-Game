using UnityEngine;

public class DoorCollision : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        
        Debug.Log("door event");
    }
}
