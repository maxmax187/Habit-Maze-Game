using System.Collections;
using UnityEngine;

public class DoorEventManager : MonoBehaviour
{
    [Header("Room")]
    public Transform roomCenter;

    [Header("Doors")]
    public DoorSlideAnimation[] doors;  // TODO possibly remove, don't think this is a required/used feature. Maybe for if we want to open all doors though, once the player has collected the coin??

    private bool transitioning = false;

    public void OnDoorTriggered(DoorSlideAnimation triggeredDoor, GameObject player)
    {
        if (transitioning) return;
        transitioning = true;
        StartCoroutine(DoTransition(triggeredDoor, player));
    }

    private IEnumerator DoTransition(DoorSlideAnimation triggeredDoor, GameObject player)
    {
        // 1. Disable player movement immediately
        var movement = player.GetComponent<PlayerMovement>();
        if (movement != null) movement.movementEnabled = false;

        var rb = player.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        // 2. Open the triggered door, wait for it
        triggeredDoor.Open();
        yield return new WaitForSeconds(triggeredDoor.Duration);

        // 3. Slide player to room center
        var playerSlide = player.GetComponent<PlayerSlideAnimation>();
        if (playerSlide != null)
            yield return StartCoroutine(playerSlide.SlideToTarget(roomCenter.position));

        // 4. Re-enable movement
        if (movement != null) movement.movementEnabled = true;

        // 5. Close the door behind them
        triggeredDoor.Close();

        transitioning = false;
    }
}