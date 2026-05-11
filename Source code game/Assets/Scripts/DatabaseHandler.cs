using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System;

public class DatabaseHandler : MonoBehaviour
{
    private bool isProd = false;

    private string GetHost()
    {
        Debug.Log("[DatabaseHandler] getting host");
        ////////////// TODO: Change to your api endpoint //////////////
        if (isProd) { return "/api"; }
        //////////////////////////////////////////////////////////////

        Debug.Log("[DatabaseHandler] isProd: false");
        Debug.Log("[DatabaseHandler] returning \"localhost:8080\" instead of \"/api\"");
        return "localhost:8080";
    }
    public void GetParticipantByEmail(string email, Action<Participant> onSuccess, Action<string> onError)
    {
        StartCoroutine(GetParticipantCoroutine(email, onSuccess, onError));
    }

    private IEnumerator GetParticipantCoroutine(string email, Action<Participant> onSuccess, Action<string> onError)
    {
        string url = GetHost() + "/getParticipant?email=" + UnityWebRequest.EscapeURL(email);

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    ParticipantRoot root = JsonConvert.DeserializeObject<ParticipantRoot>(request.downloadHandler.text);
                    onSuccess?.Invoke(root.participant);
                }
                catch (Exception ex)
                {
                    onError?.Invoke("Error parsing response: " + ex.Message);
                }
            }
            else
            {
                onError?.Invoke("Error fetching participant: " + request.error);
            }
        }
    }
    public void AddParticipant(Participant participant)
    {
        string jsonData = JsonConvert.SerializeObject(participant);
        StartCoroutine(SendDataToServer(GetHost() + "/addParticipant", jsonData));
    }

    public void AddHabitSurvey(HabitSurveyData surveyData, Action onSuccess = null, Action<string> onError = null)
    {
        string jsonData = JsonConvert.SerializeObject(surveyData);
        StartCoroutine(SendDataToServer(GetHost() + "/addHabitSurvey", jsonData, onSuccess, onError));
    }

    public void AddRoundData(RoundData roundData)
    {
        RoundData filteredRoundData = FilterRoundData(roundData);
        string jsonData = JsonConvert.SerializeObject(filteredRoundData);

        StartCoroutine(SendDataToServer(GetHost() + "/addRoundWithTransaction", jsonData));
    }

    private RoundData FilterRoundData(RoundData roundData)
    {
        int maxCheck = Mathf.Min(100, roundData.roundLogs.Count);

        for (int i = 0; i < maxCheck; i++)
        {
            if (roundData.roundLogs[i].d < 8)
            {
                roundData.roundLogs.RemoveAt(i);
                i--;
                maxCheck--;
            }
        }

        return roundData;
    }

    public void UpdateScore(ScoreUpdateData scoreData, Action onSuccess = null, Action<string> onError = null)
    {
        string jsonData = JsonConvert.SerializeObject(scoreData);

        StartCoroutine(
            SendDataToServer(
                GetHost() + "/updateScore",
                jsonData,
                onSuccess,
                onError
            )
        );
    }

    private IEnumerator SendDataToServer(string url, string jsonData, Action onSuccess = null, Action<string> onError = null)
    {
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Added: " + request.downloadHandler.text);
                onSuccess?.Invoke();
            }
            else
            {
                Debug.LogError("Error adding data: " + request.error);
                onError?.Invoke(request.error);
            }
        }
    }
}

public class ParticipantRoot
{
    public bool success { get; set; }
    public Participant participant { get; set; }
}