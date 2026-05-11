using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataController : MonoBehaviour
{
    private float intervalInMilliseconds = 20f; // Set the interval in milliseconds
    private float intervalInSeconds;
    private bool isMeasuring = false;
    [SerializeField] AIAgent finishAI;

    private List<decimal[]> timeDistanceFinish = new List<decimal[]>();
    private Coroutine currentCoroutine;

    public List<RoundLog> roundLogs = new List<RoundLog>();

    [SerializeField]
    private bool logDistance = false;

    DatabaseHandler databaseHandler;

    void Start()
    {
        databaseHandler = GetComponent<DatabaseHandler>();
        SetIntervalInSeconds();
    }

    private void SetIntervalInSeconds()
    {
        intervalInSeconds = intervalInMilliseconds / 1000f;
    }

    public void Reset()
    {
        SetIntervalInSeconds();
        timeDistanceFinish.Clear();
        roundLogs.Clear();
        time = 0;
    }

    public void Begin()
    {
        isMeasuring = true;
        StartCoroutine(PerformActionEveryInterval());
    }

    public void Stop()
    {
        isMeasuring = false;
    }

    public decimal time = 0;
    
    IEnumerator PerformActionEveryInterval()
    {
        while (isMeasuring && !AstarPath.active.isScanning)
        {
            float remainingDistance = finishAI.path.remainingDistance;

            if (float.IsInfinity(remainingDistance)) { continue; }


            //decimal elapsedTimeDecimal = Math.Round((decimal)(Time.time - startTime), 2);
            decimal elapsedTimeDecimal = time;
            decimal remainingDistanceDecimal = Math.Round((decimal)remainingDistance, 2);

            // Perform your action
            timeDistanceFinish.Add(new decimal[] { elapsedTimeDecimal, remainingDistanceDecimal });
            RoundLog roundLog = new RoundLog();
            roundLog.t = elapsedTimeDecimal;
            roundLog.d = remainingDistanceDecimal;
            roundLogs.Add(roundLog);

            time += 0.02M;

            if (logDistance) { Debug.Log($"Stored: Time = {elapsedTimeDecimal}, Value = {remainingDistanceDecimal}"); }

            // Wait for the next interval
            yield return new WaitForSeconds(intervalInSeconds);
        }

        string filePath = Path.Combine(Application.persistentDataPath, "data_" + GameManager.Instance.round + ".csv");
        SaveToCSV(filePath);
    }

    void SaveToCSV(string filePath)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            // Write the header
            writer.WriteLine("ElapsedTime,DistanceToFinish");

            // Write the data
            foreach (var entry in timeDistanceFinish)
            {
                writer.WriteLine($"{entry[0]},{entry[1]}");
            }
        }
    }

    public void AddParticipant()
    {
        databaseHandler.GetParticipantByEmail(GameManager.Instance.participantData.email,
                participant =>
                {
                    Debug.Log("Participant retrieved: " + participant.email);
                    GameManager.Instance.participantData = participant;
                    GameManager.Instance.score = participant.totalScore;
                    GameManager.Instance.uiManager.SetScore(participant.totalScore);

                },
                error => databaseHandler.AddParticipant(GameManager.Instance.participantData));
    }

    public void LoadParticipant(string email)
    {
        databaseHandler.GetParticipantByEmail(email,
                participant =>
                {
                    Debug.Log("Participant retrieved: " + participant.email);
                    GameManager.Instance.participantData = participant;
                    GameManager.Instance.score = participant.totalScore;
                    GameManager.Instance.uiManager.SetScore(participant.totalScore);
                    GameManager.Instance.playerAnimationController.SetAnimator(participant.characterSelect);
                },
                error => databaseHandler.AddParticipant(GameManager.Instance.participantData));
    }

    internal void InsertRoundDB(RoundData roundData)
    {
        databaseHandler.AddRoundData(roundData);
    }

    internal void InsertScoreDB(int score)
    {
        var data = new ScoreUpdateData
        {
            email = GameManager.Instance.participantData.email,
            totalScore = score
        };

        databaseHandler.UpdateScore(
            data,
            () =>
            {
                Debug.Log("[DataController] Score updated successfully");
            },
            error =>
            {
                Debug.LogError("[DataController] Error updating score: " + error);
            }
        );
    }
}
