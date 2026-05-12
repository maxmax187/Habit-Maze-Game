using UnityEngine;
using System.Collections;

public class DevTools : MonoBehaviour
{
    public GameObject player;
    public GameObject finish;
    [Header("Dev Tool Settings")]
    public bool enableDevTools = false;
    public float devToolKeyPressDuration = 1f;
    [Header("Level Skip")]
    public KeyCode levelSkipKey = KeyCode.Return;
    [Header("All Levels Same Seed")] // logic handled in GameManager.cs
    public bool enableAllLevelsSameSeed = false;
    public int seed = 0;
    [Header("All Levels Same Seed")] // logic handled in GameManager.cs
    [SerializeField] private bool capFPS = false;
    [SerializeField] private int targetFrameRate = 10;

    private int originalTargetFrameRate;
    private int originalVSyncCount;

    void Start()
    {
        originalTargetFrameRate = Application.targetFrameRate;
        originalVSyncCount = QualitySettings.vSyncCount;

        if (enabled && capFPS)
        {
            // cap FPS (used to simulate slow laptop)
            Application.targetFrameRate = targetFrameRate; 
            QualitySettings.vSyncCount = 0;
        }
    }

    void OnDestroy()
    {
        Application.targetFrameRate = originalTargetFrameRate;
        QualitySettings.vSyncCount = originalVSyncCount;
    }

    void OnDisable()
    {
        Application.targetFrameRate = originalTargetFrameRate;
        QualitySettings.vSyncCount = originalVSyncCount;
    }

    void Update()
    {
        if(!enableDevTools) return;
        LevelSkip();
    }


    private bool delayElapsed = false;
    private Coroutine delayCounterCoroutine = null;

    // Teleports player character to a zone that triggers the finish round code, triggers after holding levelSkipKey key (ENTER by default)
    private void LevelSkip()
    {
        if (Input.GetKeyDown(levelSkipKey)) //first frame that key is pressed => start counting
        {
            // Guard against stacked coroutines on key bounce
            if (delayCounterCoroutine != null) StopCoroutine(delayCounterCoroutine);
            delayCounterCoroutine = StartCoroutine(DelayTimer(devToolKeyPressDuration));
        }
        else if (Input.GetKey(levelSkipKey) && delayElapsed) //key has been pressed for the duration of devToolKeyPressDuration
        {
            delayElapsed = false;       // Reset so it only fires once per hold
            delayCounterCoroutine = null;

            if(player == null || finish == null)
            {
                Debug.LogWarning("[DEVTOOLS] attempted to trigger LevelSkip but failed");
                return;
            }
            
            player.transform.position = finish.transform.position;
            Debug.Log("[DEVTOOLS] Level skipped");
        }
        else if (Input.GetKeyUp(levelSkipKey))
        {
            // Key released before duration elapsed — cancel and reset
            if (delayCounterCoroutine != null)
            {
                StopCoroutine(delayCounterCoroutine);
                delayCounterCoroutine = null;
            }
            delayElapsed = false;
        }
    }

    private IEnumerator DelayTimer(float seconds)
    {
        delayElapsed = false;
        yield return new WaitForSeconds(seconds);
        delayElapsed = true;
    }

}