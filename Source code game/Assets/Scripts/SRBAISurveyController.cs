using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the SRBAI Likert-scale survey shown between the Training and Test phases.
///
/// HIERARCHY EXPECTED:
///   UIScreens > StartUI > SRBAISurveyUI > StartUI          (assign to surveyPanel)
///   UIScreens > StartUI > SRBAISurveyUI > ContinueButton   (assign to continueButton)
///
///   Each question lives at:
///   SRBAISurveyUI > StartUI > ItemGroup > Question(N) > Likert
///   The Likert object must have a ToggleGroup component attached.
///   Each child Toggle must be named "1" through "7" (used to read the scale value).
///   Assign the 4 ToggleGroup references in the Inspector.
///
/// USAGE:
///   Call Show() to display the survey — e.g. from UIManager.ShowSRBAISurvey().
///   When the participant clicks Continue, survey data is submitted to the API and
///   GameManager.Instance.AdvanceFromSurveyToTest() is called to start the Test phase.
/// </summary>
public class SRBAISurveyController : MonoBehaviour
{
    [Header("Panel References")]
    [Tooltip("The SRBAISurveyUI > StartUI GameObject that contains the survey content.")]
    [SerializeField] private GameObject surveyPanel;

    [Tooltip("The ContinueButton. Only becomes active when all 4 questions are answered.")]
    [SerializeField] private Button continueButton;

    // ---------------------------------------------------------------
    // One ToggleGroup per question.
    // The ToggleGroup component lives on the Likert object under each question.
    // Its child Toggles must be named "1" through "7".
    // ---------------------------------------------------------------
    [Header("Question Toggle Groups")]
    [Tooltip("ItemGroup > Question1 > Likert (ToggleGroup component)")]
    [SerializeField] private ToggleGroup question1Group;

    [Tooltip("ItemGroup > Question2 > Likert (ToggleGroup component)")]
    [SerializeField] private ToggleGroup question2Group;

    [Tooltip("ItemGroup > Question3 > Likert (ToggleGroup component)")]
    [SerializeField] private ToggleGroup question3Group;

    [Tooltip("ItemGroup > Question4 > Likert (ToggleGroup component)")]
    [SerializeField] private ToggleGroup question4Group;

    // Submission is routed through DataController > DatabaseHandler,
    // which handles host selection (isProd flag) and JSON serialization.

    // Stores the selected scale value (1-7) for each question; 0 = unanswered.
    private readonly int[] answers = new int[4];

    private ToggleGroup[] allGroups;

    // ---------------------------------------------------------------
    // Unity lifecycle
    // ---------------------------------------------------------------

    private void Start()
    {
        allGroups = new ToggleGroup[] { question1Group, question2Group, question3Group, question4Group };

        surveyPanel.SetActive(false);
        continueButton.gameObject.SetActive(false);

        // Register listeners on every toggle in every group.
        for (int q = 0; q < allGroups.Length; q++)
        {
            int capturedQ = q; // capture for closure
            foreach (Toggle toggle in allGroups[q].GetComponentsInChildren<Toggle>())
            {
                Toggle capturedToggle = toggle; // capture for closure
                toggle.onValueChanged.AddListener((isOn) =>
                {
                    if (isOn)
                        OnToggleChanged(capturedQ, capturedToggle);
                });
            }
        }

        continueButton.onClick.AddListener(OnContinueClicked);
    }

    private void OnDestroy()
    {
        continueButton.onClick.RemoveListener(OnContinueClicked);
    }

    // ---------------------------------------------------------------
    // Public API
    // ---------------------------------------------------------------

    /// <summary>
    /// Show the survey panel and reset all previous answers.
    /// Call this from UIManager.ShowSRBAISurvey().
    /// </summary>
    public void Show()
    {
        ResetSurvey();
        surveyPanel.SetActive(true);
    }

    /// <summary>
    /// Hide the survey panel. Called automatically after Continue is clicked.
    /// </summary>
    public void Hide()
    {
        surveyPanel.SetActive(false);
    }

    // ---------------------------------------------------------------
    // Internal logic
    // ---------------------------------------------------------------

    /// <summary>
    /// Called whenever a toggle is switched on.
    /// Reads the scale value from the toggle's GameObject name (must be "1"-"7").
    /// </summary>
    private void OnToggleChanged(int questionIndex, Toggle toggle)
    {
        if (int.TryParse(toggle.gameObject.name, out int value))
        {
            answers[questionIndex] = value;
        }
        else
        {
            Debug.LogWarning($"[SRBAISurvey] Toggle '{toggle.gameObject.name}' in question {questionIndex + 1} " +
                             "could not be parsed as an integer. Name your toggles '1' through '7'.");
        }

        EvaluateContinueButton();
    }

    /// <summary>
    /// Shows the Continue button only when every question has a non-zero answer.
    /// </summary>
    private void EvaluateContinueButton()
    {
        foreach (int answer in answers)
        {
            if (answer == 0)
            {
                continueButton.gameObject.SetActive(false);
                return;
            }
        }
        continueButton.gameObject.SetActive(true);
        continueButton.interactable = true;
    }

    /// <summary>
    /// Deactivates all toggles without firing their callbacks, then clears answers.
    /// </summary>
    private void ResetSurvey()
    {
        for (int i = 0; i < answers.Length; i++)
            answers[i] = 0;

        foreach (ToggleGroup group in allGroups)
        {
            foreach (Toggle t in group.GetComponentsInChildren<Toggle>())
                t.SetIsOnWithoutNotify(false);
        }

        continueButton.gameObject.SetActive(false);
        continueButton.interactable = true;
    }

    /// <summary>
    /// Called when the participant clicks Continue.
    /// Submits data to the API, then advances the game regardless of network result.
    /// </summary>
    private void OnContinueClicked()
    {
        // Disable button immediately to prevent double-clicks during submission.
        continueButton.gameObject.SetActive(false);
        Hide();
        SubmitAndAdvance();
    }

    /// <summary>
    /// Submits survey data via DatabaseHandler (which respects the isProd host flag),
    /// then advances to the Test phase regardless of network result.
    /// </summary>
    private void SubmitAndAdvance()
    {
        HabitSurveyData surveyData = BuildSurveyData();

        // Grab the DatabaseHandler from the same object that holds DataController,
        // mirroring how DataController.Start() resolves it.
        DatabaseHandler db = GameManager.Instance.dataController.GetComponent<DatabaseHandler>();

        db.AddHabitSurvey(
            surveyData,
            onSuccess: () =>
            {
                Debug.Log("[SRBAISurvey] Survey submitted successfully.");
                GameManager.Instance.AdvanceFromSurveyToTest();
            },
            onError: (error) =>
            {
                Debug.LogError($"[SRBAISurvey] Submission failed: {error}. Advancing anyway.");
                GameManager.Instance.AdvanceFromSurveyToTest();
            }
        );
    }

    /// <summary>
    /// Packages the collected answers into a HabitSurveyData object.
    /// </summary>
    private HabitSurveyData BuildSurveyData()
    {
        HabitSurveyData data = new HabitSurveyData
        {
            participantEmail = PlayerPrefs.GetString("ParticipantEmail"),
            day = PlayerPrefs.GetInt("Day"),
            srbai1 = answers[0],
            srbai2 = answers[1],
            srbai3 = answers[2],
            srbai4 = answers[3]
        };
        return data;
    }
}