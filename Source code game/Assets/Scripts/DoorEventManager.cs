using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DoorEventManager : MonoBehaviour
{
    [Header("Room")]
    public Transform roomCenter;
    public bool doorEventHasHappened = false;

    [Header("Doors")]
    public DoorSlideAnimation[] doors;  // TODO possibly remove, don't think this is a required/used feature. Maybe for if we want to open all doors though, once the player has collected the coin??
    public float doorLockedCooldown = 5f;
    public float bufferDelay = 3f; // TODO make this a range instead of single variable

    private bool transitioning = false;


    private Room room;
    private Vector3 roomScale;

    // set room script and transform component of gameObject this script is attached to
    private void Awake()
    {
        room = GetComponent<Room>();
        // roomCenter = GetComponent<Transform>(); // experimental
        // roomScale = roomCenter.localScale;
    }

    public void OnDoorTriggered(DoorSlideAnimation triggeredDoor, GameObject player)
    {
        if (transitioning) return;
        transitioning = true; //is only set to false again after exiting the room.

        // check if we should start coin spawn event or post-event door behaviour
        if (doorEventHasHappened == false)
        {
            doorEventHasHappened = true;
            // StartCoroutine(DoTransition(triggeredDoor, player));
            //DoCoinPresentation(BackDir, ForwardDir)
            transitioning = false; //TODO temporary
        } else
        {
            //TODO check if the player is allowed to go through the door yet, or whether it should be locked
            // For now: simply moves player in through one door and out through the other
            // perform some action here on the other active door that is not triggerdDoor
            StartCoroutine(InAndOut(triggeredDoor, player));
        }
    }

    // Move player into room through entry door, then out through exit door
    private IEnumerator InAndOut(DoorSlideAnimation entryDoor, GameObject player)
    {
        (Room.Directions dir, GameObject gameObject)? exitDoorTuple = GetExitDoor(entryDoor);
        if (exitDoorTuple == null) yield break;

        //move player into room
        yield return StartCoroutine(DoTransition(entryDoor, player, reEnableMovement: false));

        // move player out of room
        DoorSlideAnimation exitDoor = exitDoorTuple.Value.gameObject.GetComponent<DoorSlideAnimation>();
        Room.Directions exitDir = exitDoorTuple.Value.dir;
        yield return StartCoroutine(DoTransition(exitDoor, player, toInside: false, exitDir: exitDir));
    }

    // Returns the active door that is not the entry door, alongside its direction
    private (Room.Directions dir, GameObject gameObject)? GetExitDoor(DoorSlideAnimation entryDoor)
    {
        Dictionary<Room.Directions, GameObject> activeDoors = room.GetActiveDoors();
        foreach (var kvp in activeDoors)
        {
            if (kvp.Value.GetComponent<DoorSlideAnimation>() != entryDoor)
                return (kvp.Key, kvp.Value);
        }
        return null;
    }

    // all the steps to moving player inside or outside of room and opening sliding doors
    private IEnumerator DoTransition(DoorSlideAnimation door, GameObject player, bool toInside = true, bool reEnableMovement = true, Room.Directions exitDir = Room.Directions.NONE)
    {
        // 1. Disable player movement immediately
        var movement = player.GetComponent<PlayerMovement>();
        if (movement != null) movement.movementEnabled = false;

        var rb = player.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        // 2. Open the relevant door, wait for it
        door.Open();
        yield return new WaitForSeconds(door.Duration);

        if (toInside == true){
            //3.1 Slide player to room center
            var playerSlide = player.GetComponent<PlayerSlideAnimation>();
            if (playerSlide != null) 
                yield return StartCoroutine(playerSlide.SlideToTarget(roomCenter.position));
        }else{
            // 3.2 Slide player to outside the exit door

            // determine slide distance offset from room center per direction
            var exitOffsets = new Dictionary<Room.Directions, Vector2>
            {
                { Room.Directions.TOP,    Vector2.up    * 6f },
                { Room.Directions.RIGHT,  Vector2.right * 6f },
                { Room.Directions.BOTTOM, Vector2.down  * 6f },
                { Room.Directions.LEFT,   Vector2.left  * 6f }
            };
            Vector2 exitTarget = (Vector2)roomCenter.position + exitOffsets[exitDir];

            // Set character facing rotation based on the same offset vectors
            Animator animator = player.GetComponent<Animator>();
            if (animator != null)
            {
                Vector2 animDir = exitOffsets[exitDir];
                animator.SetFloat("Horizontal", animDir.x);
                animator.SetFloat("Vertical", animDir.y);
            }

            // start sliding player
            var playerSlide = player.GetComponent<PlayerSlideAnimation>();
            if (playerSlide != null)
                yield return StartCoroutine(playerSlide.SlideToTarget(exitTarget));
        }         

        // 4. Re-enable movement (unless specified otherwise)
        if (movement != null && reEnableMovement) movement.movementEnabled = true;

        // 5. Close the door behind them
        door.Close();

        // 6. Only allow new door trigger to happen after having exited the room again
        if(toInside == false) transitioning = false;
    }


    private IEnumerator DoCoinPresentation(DoorSlideAnimation triggeredDoor, GameObject player)
    {
        // 1. wait until player is moved inside room, do not re-enable movement yet.
        yield return StartCoroutine(DoTransition(triggeredDoor, player, reEnableMovement: false));

        // 2. Some amount of delay as specified by bufferDelay variable
        yield return new WaitForSeconds(bufferDelay);
        // 3. Enable coin bubble GameObject with the silver sprite or gold sprite

        // 4. await key press


        // user spacebar:
        // 5.1 Spawn coin (CoinController.cs)
        
        // 6.1 DoTransition (outside, back)

        // 7.1 Disable door trigger for some length of time



        // user "forward" movement
        // 5.2 DoTransition (outside, forward)

        // 6.2 Disable door permanently
    }
}