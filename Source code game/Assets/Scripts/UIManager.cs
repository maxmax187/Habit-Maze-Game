using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    [SerializeField]
    private TMP_Text roundText, scoreText, timerText, timesUpText;
    [SerializeField]
    private GameObject roundFinishedTimeUI, roundFinishedUI, practiceFinishedUI, doorPracticeFinishedUI, trainingFinishedUI, testFinishedUI;
    [SerializeField] private SRBAISurveyController srbaISurveyController;

    public void ShowSRBAISurvey()
    {
        srbaISurveyController.Show();
    }
    
    public void SetRound(int round)
    {
        roundText.text = "Floor: " + round.ToString() + "/" + GameManager.Instance.GetCurrentTotalRounds();
    }

    public void SetScore(int score)
    {
        scoreText.text = "Score: " + score.ToString();
    }

    public void SetTimer(int timeLeft)
    {
        timerText.text = "Timer: " + timeLeft.ToString();
    }

    public void SetTimer(float timeToDisplay)
    {
        timeToDisplay += 1; // Adjust for display purposes
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timerText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
    }

    public void ShowRoundFinished(bool reachedFinish)
    {
        if (reachedFinish)
        {
            roundFinishedUI.SetActive(true);
            return;
        }
        roundFinishedTimeUI.SetActive(true);

    }

    public void ShowFinishedPractice()
    {
        practiceFinishedUI.SetActive(true);
    }

    public void ShowFinishedDoorPractice()
    {
        doorPracticeFinishedUI.SetActive(true);
    }


    public void ShowFinishedTraining()
    {
       trainingFinishedUI.SetActive(true);
    }

    public void ShowFinishedTest()
    {
        testFinishedUI.SetActive(true);
    }

    public void HideUI()
    {
        roundFinishedTimeUI.SetActive(false);
        roundFinishedUI.SetActive(false);
        practiceFinishedUI.SetActive(false);
        doorPracticeFinishedUI.SetActive(false);
        trainingFinishedUI.SetActive(false);
        testFinishedUI.SetActive(false);
    }
}
