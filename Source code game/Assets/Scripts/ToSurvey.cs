using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Networking;

/// <summary>
/// This file should be redundant now
/// </summary>
public class ToSurvey : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(UpdateScoreAndLoadSurvey());
        }
    }

    IEnumerator UpdateScoreAndLoadSurvey()
    {
        // Create the request data
        var data = new ScoreUpdateData
        {
            email = GameManager.Instance.participantData.email,
            totalScore = GameManager.Instance.score
        };

        // To JSON
        string jsonData = JsonUtility.ToJson(data);

        ////////////// TODO: Change to your api endpoint //////////////
        string url = "/api/updateScore";
        //////////////////////////////////////////////////////////////
      
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error updating score: {request.error}\nResponse: {request.downloadHandler.text}");
            } else
            {
                Debug.Log("Score update successfully");
            }
        }

        // Load the surveys
         SceneManager.LoadScene("SurveyHabit");
    }
}