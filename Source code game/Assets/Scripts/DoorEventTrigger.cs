using System.Collections;
using UnityEngine;

/// <summary>
///  Triggered when the player reaches the door.
///  Disables player movement and slides player to room center
/// </summary>
public class DoorTrigger : MonoBehaviour
{
    [Header("Room")]
    public Transform roomCenter;          // Assign the center point of THIS room

    [Header("Door Transition")]
    public float moveSpeed = 5f;          // Units per second to slide player
    public float arrivalThreshold = 0.05f; // How close = "arrived"

    [Header("Door New State")]
    public Vector3 doorNewPosition;       // Local or world position after transition
    public float doorNewRotation;         // Z rotation in degrees after transition
    public bool useLocalPosition = true;  // Usually true for child objects

    private bool triggered = false;

    private SlidingDoor slidingDoor;

    void Awake()
    {
        slidingDoor = GetComponent<SlidingDoor>();
    } 


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;
        StartCoroutine(DoTransition(other.gameObject));
    }

    private IEnumerator DoTransition(GameObject player)
    {
        // 1. Disable movement + zero velocity
        var movement = player.GetComponent<PlayerMovement>();
        if (movement != null) movement.movementEnabled = false;
        var rb = player.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        // 2. Open the door
        if (slidingDoor != null)
        {
            Debug.Log("Opening sliding door");
            slidingDoor.Open();
        }

        // 3. Wait for door to finish opening, then move player
        yield return new WaitForSeconds(slidingDoor != null ? slidingDoor.duration : 0f);

        // 4. Slide player to room center
        while (Vector2.Distance(player.transform.position, roomCenter.position) > arrivalThreshold)
        {
            player.transform.position = Vector2.MoveTowards(
                player.transform.position,
                roomCenter.position,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }
        player.transform.position = roomCenter.position;

        // 5. Re-enable movement
        if (movement != null) { movement.movementEnabled = true; } 

        // 6. Close the door behind them
        if (slidingDoor != null) { slidingDoor.Close(); } 


        // Optional: reset so it can trigger again if needed
        // triggered = false;
    }
}