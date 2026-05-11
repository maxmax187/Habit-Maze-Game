using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DoorEventManager : MonoBehaviour
{
    [Header("Room")]
    public Transform roomCenter;

    [Header("Doors / Coin Event")]
    public float doorLockedCooldown = 5f;

    public float bufferDelayMinSecs = 1f;
    public float bufferDelayMaxSecs = 3f;

    public bool doorsLocked = false;
    public bool doorEventHasHappened = false;


    private bool transitioning = false;


    private Room room;

    private void Awake()
    {
        room = GetComponent<Room>();
    }

    public void Reset()
    {
        doorsLocked = false;
        doorEventHasHappened = false;
        transitioning = false;
        StopAllCoroutines();
    }

    public void OnDoorTriggered(DoorSlideAnimation triggeredDoor, GameObject player)
    {
        if (transitioning) return;
        Debug.Log("[TRIGGER] Player triggered door event");

        transitioning = true; //is only set to false again after exiting the room.

        // check if we should start coin spawn event or post-event door behaviour
        if (doorEventHasHappened == false)
        {
            doorEventHasHappened = true;
            // StartCoroutine(DoTransition(triggeredDoor, player));
            (Room.Directions dir, GameObject gameObject)? entryDoorTuple = GetDoorTuple(triggeredDoor);
            (Room.Directions dir, GameObject gameObject)? exitDoorTuple = GetExitDoor(triggeredDoor);

            StartCoroutine(DoCoinPresentation(entryDoorTuple, exitDoorTuple, player));
            // transitioning = false; //TODO temporary, will later happen automatically after player moved outside of door
        } else
        {
            //TODO thought bubble with locked door thought maybe??
            // if doors not locked, move player in and out through doors
            if (doorsLocked)
            {
                transitioning = false;
                Debug.Log("[LOCKED] Player attempted to move through Locked doors");
            }
            else
            {
                StartCoroutine(InAndOut(triggeredDoor, player));  
            }  
        }
    }

    // Coroutine for instructionpanel
    private IEnumerator WaitForDismissThenTrigger(DoorSlideAnimation triggeredDoor, GameObject player)
    {
        // Wait until the instruction panel is dismissed (timeScale returns to 1)
        yield return new WaitUntil(() => Time.timeScale == 1f);

        // Now fire the normal door logic
        transitioning = true;
        doorEventHasHappened = true;

        (Room.Directions dir, GameObject gameObject)? entryDoorTuple = GetDoorTuple(triggeredDoor);
        (Room.Directions dir, GameObject gameObject)? exitDoorTuple = GetExitDoor(triggeredDoor);
        StartCoroutine(DoCoinPresentation(entryDoorTuple, exitDoorTuple, player));
    }

    // Move player into room through entry door, then out through exit door
    private IEnumerator InAndOut(DoorSlideAnimation entryDoor, GameObject player)
    {
        (Room.Directions dir, GameObject gameObject)? exitDoorTuple = GetExitDoor(entryDoor);
        if (exitDoorTuple == null) yield break;
        Debug.Log("[DoorEventManager] Moving player in and out of room");
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

    // Returns the active door and its room direction as a tuple
    private (Room.Directions dir, GameObject gameObject)? GetDoorTuple(DoorSlideAnimation doorAnim)
    {
        Dictionary<Room.Directions, GameObject> activeDoors = room.GetActiveDoors();
        foreach (var kvp in activeDoors)
        {
            if (kvp.Value.GetComponent<DoorSlideAnimation>() == doorAnim)
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

        // 5. Close the door again
        door.Close();

        // 6. Only allow new door trigger to happen after having exited the room again
        if(toInside == false) transitioning = false;
    }


    private IEnumerator DoCoinPresentation((Room.Directions dir, GameObject gameObject)? entryDoorTuple, (Room.Directions dir, GameObject gameObject)? exitDoorTuple, GameObject player)
    {
        if (exitDoorTuple == null || entryDoorTuple == null || player == null) yield break;
        DoorSlideAnimation exitDoor = exitDoorTuple.Value.gameObject.GetComponent<DoorSlideAnimation>();
        Room.Directions exitDir = exitDoorTuple.Value.dir;

        DoorSlideAnimation entryDoor = entryDoorTuple.Value.gameObject.GetComponent<DoorSlideAnimation>();
        Room.Directions entryDir = entryDoorTuple.Value.dir;

        // 1. wait until player is moved inside room, do not re-enable movement yet.
        yield return StartCoroutine(DoTransition(entryDoor, player, reEnableMovement: false));
      
        // 2. Show thought bubble, and after buffer delay show gold/silver coin
        ThoughtBubble bubble = player.GetComponent<ThoughtBubble>();

        // NEW: Show inside-room instruction on round 1 of DoorPractice
        if (GameManager.Instance.GetCurrentGameState() == "DoorPractice" && GameManager.Instance.round == 1)
        {
            bool dismissed = false;
            InstructionManager.Instance.ShowInstruction(
                "You're now inside the security area!\n\n" +
                "You remember you have dropped a coin along the way. \n\n"+
                "Press SPACE to open the back door to go back and collect the coin.\n" +
                "OR press your movement key toward the exit to open the front door to continue without collecting it.",
                onDismiss: () => dismissed = true
            );

            yield return new WaitUntil(() => dismissed);

            dismissed = false;
            InstructionManager.Instance.ShowInstruction(
                "When pressing SPACE to go back for the coin, the security door will have a short cool down." +
                "You will have to wait a few seconds before going through the security area again.\n" +
                "When going forward to go towards the exit without collecting the coin,\n" +
                "the security door locks and you can no longer go back when you change your mind.\n\n" +
                "Coins do not reward any points yet in these practice rounds.",
                onDismiss: () => dismissed = true
                );


            // Wait for dismiss
            yield return new WaitUntil(() => dismissed);
        }

        // 2. Some amount of delay as specified by bufferDelay variable
        float bufferDelay = GetSeededDelay(GameManager.Instance.currentSeed);
        int currentCoinIdentity = GameManager.Instance.currentCoinIdentity;
        Debug.Log($"[DoorEventManager] Showing coin with identity: {currentCoinIdentity} after buffer delay of: {bufferDelay}");

        decimal thoughtBubbleTime = GameManager.Instance.dataController.time;
        bubble.Show(currentCoinIdentity, bufferDelay); //TODO adjust gold/silver based on the predetermined distribution
        yield return new WaitForSeconds(bufferDelay);
        decimal coinPresentTime = GameManager.Instance.dataController.time;


        // key press to room direction mappings
        var exitWASDKeys = new Dictionary<Room.Directions, KeyCode>
        {
            { Room.Directions.TOP,    KeyCode.W         },
            { Room.Directions.RIGHT,  KeyCode.D         },
            { Room.Directions.BOTTOM, KeyCode.S         },
            { Room.Directions.LEFT,   KeyCode.A         }
        };

        var exitArrowKeys = new Dictionary<Room.Directions, KeyCode>
        {
            { Room.Directions.TOP,    KeyCode.UpArrow    },
            { Room.Directions.RIGHT,  KeyCode.RightArrow },
            { Room.Directions.BOTTOM, KeyCode.DownArrow  },
            { Room.Directions.LEFT,   KeyCode.LeftArrow  }
        };

        // 3. await key press
        int receivedInput = -1;
        while (receivedInput == -1)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                receivedInput = 1;
            else if (Input.GetKeyDown(exitWASDKeys[exitDir]) || Input.GetKeyDown(exitArrowKeys[exitDir]))
                receivedInput = 0;
            yield return null;
        }

        decimal playerChoiceTime = GameManager.Instance.dataController.time;
        bubble.Hide(); // hide thought bubble and coin again
        bool wentBackForCoin = false;

        if (receivedInput == 1) // 4.1 received keycode SPACE for going backwards to the coin
        {
            wentBackForCoin = true;
            Debug.Log("received SPACE");
            yield return StartCoroutine(DoTransition(entryDoor, player, toInside: false, exitDir:entryDir));

            // spawn actual coin behind player in maze
            GameManager.Instance.coinController.SpawnCoin(currentCoinIdentity);
            // temporarily lock doors
            yield return StartCoroutine(LockDoorsTemporarily());
            // Refresh and Re-fire trigger for any player already standing in the previously locked doorway
            entryDoor.GetComponent<DoorTrigger>().Refresh();   
        }
        else if (receivedInput == 0) // 4.2 received keycode "FORWARD" MOVEMENT for continuing without the coin
        {
            Debug.Log("received MOVEMENT KEY FORWARD");
            yield return StartCoroutine(DoTransition(exitDoor, player, toInside:false, exitDir:exitDir));
            doorsLocked = true;

            Debug.Log("[DoorEventManager] permanently locked doors");
        };

        // pass datapoints on to gamemanager
        GameManager.Instance.thoughtBubbleTime = thoughtBubbleTime;
        GameManager.Instance.bufferDelay = bufferDelay;
        GameManager.Instance.coinPresentTime = coinPresentTime;
        GameManager.Instance.playerChoiceTime = playerChoiceTime;
        GameManager.Instance.wentBackForCoin = wentBackForCoin;
    }

    private IEnumerator LockDoorsTemporarily()
    {
        doorsLocked = true;
        Debug.Log($"[DoorEventManager] Doors temporarily locked for {doorLockedCooldown} seconds");

        yield return new WaitForSeconds(doorLockedCooldown);

        doorsLocked = false;
        Debug.Log("[DoorEventManager] Doors unlocked");
    }

    private float GetSeededDelay(int seed)
    {
        Random.InitState(seed);
        return Random.Range(bufferDelayMinSecs, bufferDelayMaxSecs);
    }
}