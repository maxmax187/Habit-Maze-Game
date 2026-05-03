using UnityEngine;

public class CountdownTimer : MonoBehaviour
{
    [SerializeField]
    private float totalTime = 60;
    public float timeRemaining { get; private set; }
    private bool timerIsRunning = false;

    void Start()
    {
        timeRemaining = totalTime;
        timerIsRunning = true;
    }

    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > Mathf.Epsilon) // Mathf.Epsilon = floating point zero
            {
                timeRemaining -= Time.unscaledDeltaTime;
                GameManager.Instance.uiManager.SetTimer(timeRemaining);
            }
            else
            {
                Debug.Log("Time has run out!");
                timeRemaining = 0;
                timerIsRunning = false;
                GameManager.Instance.FinishRound(false);
            }
        }
    }

    public void Reset()
    {
        timeRemaining = totalTime;
    }

    public void StartTimer() { 
        timerIsRunning = true;
    }

    public void StopTimer()
    {
        timerIsRunning = false;
    }
}
