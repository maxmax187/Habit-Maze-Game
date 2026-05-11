using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class GameManager : MonoBehaviour
{
    private enum GameState
    {
        Introduction,
        Practice, // No reward yet, familiarilizing with controls
        DoorPractice, // No reward yet, familiarizing with mechanics door
        Training, // Training phase, with reward.
        SRBAISurvey,
        DevalueIntroduction,
        Test,
        Debrief
    }

    private GameState gameState = GameState.Practice;

    [SerializeField]
    GenerateMaze generateMazeController;
    [SerializeField]
    CountdownTimer countdownTimer;
    [SerializeField]
    public PlayerMovement player;
    [SerializeField]
    public CoinController coinController;
    [SerializeField]
    public DataController dataController;
    [SerializeField]
    public AIAgent finishAi;

    public static GameManager Instance { get; private set; }

    [Header("Level configurations")]
    public int practiceRounds = 3;
    public int doorPracticeRounds = 3;
    public int trainingRounds = 5;
    public int trainingGoldCoinAmt = 2;
    public int testRounds = 5;
    public int testGoldCoinAmt = 2;

    public int round { get; private set; } = 1;
    public int totalRound { get; private set; } = 0;
    public int score { get; set; } = 0;

    [Header("Coin Valuation & Score")]
    public bool isCoinDevalued = false;
    public int goldCoinValue = 2;
    public int silverCoinValue = 1;
    public int mazeCompletionValue = 2;

    private int[] practiceLevels;
    private int[] doorPracticeLevels;
    private int[] trainingLevels;
    private int[] testLevels;

    public int currentSeed;
    public int currentCoinIdentity;
    private bool pickedUpCoinIsGold = true; // true = gold, false = silver. This is bad code but I can't be bothered to fix it.
    private int[] coinOrderTraining;
    private int[] coinOrderTest;

    public float currentTotalDistance = 0;

    // UI
    public UIManager uiManager;

    // Database data
    public Participant participantData = new Participant();

    private string currentDate;
    public bool pickedUpCoin = false;
    private decimal coinPickupTime;

    public int day = 1;

    public decimal thoughtBubbleTime = 0;
    public float bufferDelay = 0f;
    public decimal coinPresentTime = 0;
    public decimal playerChoiceTime = 0;
    public decimal reactionTime = 0;
    public bool wentBackForCoin = false;

    [SerializeField] private TMP_Text dayText;

    [SerializeField]
    public CharacterAnimationController playerAnimationController;

    [SerializeField] private GameObject sceneControllerDay1;
    [SerializeField] private GameObject sceneControllerDay2;
    [SerializeField] private GameObject sceneControllerDay3;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayerPrefs.SetInt("Day", day);
        dayText.text = "Day " + day;
        participantData.email = PlayerPrefs.GetString("ParticipantEmail");
        currentDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        if (day == 1)
        {
            sceneControllerDay1.SetActive(true);
            GeneratePracticeLevels();
            GenerateDoorPracticeLevels();
        }

        if (day == 2 || day == 3)
        {
            gameState = GameState.Training;
            StartCoroutine(WaitAndLoadParticipant());
        }

        if (day == 2)
        {
            sceneControllerDay2.SetActive(true);
        }

        if (day == 3)
        {
            sceneControllerDay3.SetActive(true);
        }

        GenerateLevelOrder();
    }


    IEnumerator WaitAndLoadParticipant()
    {
        yield return new WaitForSeconds(1);
        dataController.LoadParticipant(participantData.email);
    }

    private void GeneratePracticeLevels()
    {
        int startValue = 900; // Starting value for seeds
        practiceLevels = new int[practiceRounds]; // Create an array with the size of testRounds

        for (int i = 0; i < practiceRounds; i++)
        {
            practiceLevels[i] = startValue + i; // Increment the start value by 1 for each element creating consistent seeds e.g. 900, 901, 902...
        }

        Debug.Log("Pratice Levels: " + string.Join(", ", practiceLevels));
    }

    private void GenerateDoorPracticeLevels()
    {
        int startValue = 800; //Different range from practice
        doorPracticeLevels = new int[doorPracticeRounds];
        for (int i = 0; i < doorPracticeRounds; i++)
        {
            doorPracticeLevels[i] = startValue + i;
        }
    }
    private void GenerateLevelOrder()
    {
        System.DateTime now = System.DateTime.Now;
        int minutes = now.Minute;
        int seconds = now.Second;
        int milliseconds = now.Millisecond;
        int seed = minutes * 100000 + seconds * 1000 + milliseconds;

        UnityEngine.Random.InitState(seed);
        Debug.Log(seed);

        int totalRounds = trainingRounds + testRounds;

        // Create an array with numbers from 1 to 100
        int[] allLevels = new int[totalRounds];
        for (int i = 0; i < totalRounds; i++)
        {
            allLevels[i] = i + 1;
        }

        // Shuffle the array
        for (int i = allLevels.Length - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1); // Get a random index between 0 and i
            // Swap numbers[i] and numbers[j]
            int temp = allLevels[i];
            allLevels[i] = allLevels[j];
            allLevels[j] = temp;
        }

        DevTools devTools = GetComponent<DevTools>();
        if(devTools.enableDevTools && devTools.enableAllLevelsSameSeed)
        {
            //overwrite all levels to be the same seed
            int overrideSeed = GetComponent<DevTools>().seed;
            for (int i = 0; i < allLevels.Length; i++)
            {
                allLevels[i] = overrideSeed;
            }
            Debug.Log($"[DEVTOOLS / GAMEMANAGER] overwritten all level seeds to be: {overrideSeed}");
        }

        Debug.Log("Training Levels: " + string.Join(", ", allLevels));

        trainingLevels = new int[trainingRounds];
        testLevels = new int[testRounds];

        //generate here the order of gold vs silver coins (rounds, amt_gold, seed)
        coinOrderTraining = CoinOrderGenerator.GenerateRandomCoinOrder(trainingRounds, trainingGoldCoinAmt, seed);
        coinOrderTest = CoinOrderGenerator.GenerateRandomCoinOrder(testRounds, testGoldCoinAmt, seed);
        Debug.Log($"Generated coin order for Training rounds of length: {trainingRounds}");
        Debug.Log("Coin order: " + String.Join(", ", coinOrderTraining));

        Array.Copy(allLevels, 0, trainingLevels, 0, trainingRounds);
        Array.Copy(allLevels, trainingRounds, testLevels, 0, testRounds);

        Debug.Log("Training Levels: " + string.Join(", ", trainingLevels));
        Debug.Log("Test Levels: " + string.Join(", ", testLevels));

        participantData.trainingSeeds = string.Join(", ", trainingLevels);
        participantData.testSeeds = string.Join(", ", testLevels);
    }

    public void FinishRound(bool reachedFinish)
    {
        Debug.Log("FinishRound reached");
        dataController.Stop();
        player.GetComponent<PlayerSlideAnimation>()?.StopSliding(); // terminate slide animation in case player got stuck with bad timing after door event
        player.GetComponent<ThoughtBubble>().Hide();
        player.GetComponent<PlayerMovement>().movementEnabled = true; // re-enable in case it was locked mid-transition
        player.enabled = false;

        Rigidbody2D rbPlayer = player.gameObject.GetComponent<Rigidbody2D>();
        rbPlayer.linearVelocity = Vector2.zero;
        player.moveVelocity = Vector2.zero;

        countdownTimer.StopTimer();


        // Prepare DB data
        Round roundInfo = new Round();

        roundInfo.participantEmail = participantData.email;
        roundInfo.seed = currentSeed;
        roundInfo.round = round;
        // roundInfo.didCoinSpawn = coinController.hasSpawned;
        roundInfo.pickedUpCoin = pickedUpCoin;
        roundInfo.finished = reachedFinish;
        roundInfo.phase = gameState.ToString();
        roundInfo.date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        // roundInfo.totalDistance = Math.Round((decimal)coinController.totalDistance, 2); 
        // roundInfo.distanceCoinSpawn = Math.Round((decimal)coinController.spawnDistance, 2); 
        roundInfo.remainingTime = Math.Round((decimal)countdownTimer.timeRemaining, 2);
        roundInfo.totalRoundsFinished = totalRound;
        roundInfo.day = day;
        // roundInfo.coinPickupTime = coinPickupTime; 
        // roundInfo.coinSpawnTime = coinController.spawnTime; 
        roundInfo.thoughtBubbleTime = thoughtBubbleTime;
        roundInfo.bufferDelay = (decimal)bufferDelay;
        roundInfo.coinPresentTime = coinPresentTime; 
        roundInfo.playerChoiceTime = playerChoiceTime;
        roundInfo.reactionTime = reactionTime; 
        roundInfo.wentBackForCoin = wentBackForCoin; 
        roundInfo.coinIdentity = currentCoinIdentity;  // is not reset between rounds, probably best to keep this way

        RoundData roundData = new RoundData();
        roundData.roundLogs = dataController.roundLogs;
        roundData.round = roundInfo;

        dataController.InsertRoundDB(roundData);
        ResetDataCollectionPoints();


        if (reachedFinish && gameState != GameState.Practice && gameState != GameState.DoorPractice)
        {
            if(pickedUpCoin && pickedUpCoinIsGold)
            {
                score += goldCoinValue;
            }else if (pickedUpCoin)
            {
                score += silverCoinValue;
            }
            score += mazeCompletionValue;
            uiManager.SetScore(score);
        }

        // Check if finished practice
        if (gameState == GameState.Practice && round == practiceRounds)
        {
            gameState = GameState.DoorPractice;
            uiManager.ShowFinishedPractice();
            round = 1;
            score = 0;
            return;
        }

        if (gameState == GameState.DoorPractice && round == doorPracticeRounds)
        {
            gameState = GameState.Training;
            uiManager.ShowFinishedDoorPractice();
            round = 1;
            score = 0;
            return;
        }

        // Check if finished training and is day 2 so should not continue to testing
        if (gameState == GameState.Training && round == trainingRounds && day == 2)
        {
            gameState = GameState.Debrief;
            uiManager.ShowFinishedTest();
            round = 1;
            return;
        }


        // Check if finished training
        // if (gameState == GameState.Training && round == trainingRounds)
        // {
        //     isCoinDevalued = true;
        //     gameState = GameState.Test;
        //     uiManager.ShowFinishedTraining();
        //     round = 1;
        //     return;
        // }
        if (gameState == GameState.Training && round == trainingRounds)
        {
            isCoinDevalued = true;
            gameState = GameState.SRBAISurvey; // wait here, not Test yet
            uiManager.ShowSRBAISurvey();       // show survey via UIManager
            round = 1;
            return;
        }

        // Check if finished test
        if (gameState == GameState.Test && round == testRounds)
        {
            gameState = GameState.Debrief;
            uiManager.ShowFinishedTest();
            round = 1;
            // TODO score -> send score data to server
            return;
        }

        // Debug.Log("All checks in FinishRound done");
        // Debug.Log(reachedFinish);
        uiManager.ShowRoundFinished(reachedFinish);
    }

    // from SRBAI to next round (first test round)
    public void AdvanceFromSurveyToTest()
    {
        isCoinDevalued = true;
        round = 1;
        gameState = GameState.Test;
        uiManager.ShowFinishedTraining();
    }

    public void PickUpCoin(bool isGold)
    {
        pickedUpCoin = true;
        pickedUpCoinIsGold = isGold;
        coinPickupTime = Math.Round((decimal)countdownTimer.timeRemaining, 2);
    }

    private void ResetDataCollectionPoints()
    {
        pickedUpCoin = false;
        thoughtBubbleTime = 0;
        bufferDelay = 0f;
        coinPresentTime = 0;
        playerChoiceTime = 0;
        reactionTime = 0;
        wentBackForCoin = false;
    }

    public void NextRound(bool isNewPhase, bool isEngaging, bool isFirst)
    {
        uiManager.HideUI();
        if (!isNewPhase) { round++; }
        totalRound++;
        GenerateSeed();

        coinPickupTime = 0;
        uiManager.SetRound(round);
        generateMazeController.CreateMaze();
        player.transform.position = new Vector2(9, 0);
        finishAi.StartPathFinding();

        //StartCoroutine(CheckTotalDistanceCoroutine());

        coinController.Reset();

        countdownTimer.Reset();
        countdownTimer.StartTimer();
        dataController.Reset();
        
        if (!isFirst)
        {
            dataController.Begin();
        }

        if (!isEngaging)
        {
            player.enabled = true;
        }
    }

    private int maxTries = 50;

    IEnumerator CheckTotalDistanceCoroutine()
    {
        int attempts = 0;
        float distance = 0;

        while (attempts < maxTries)
        {
            distance = finishAi.path.remainingDistance;
            // Check the condition
            if (distance > 30f && !float.IsInfinity(distance))
            {
                currentTotalDistance = finishAi.path.remainingDistance;
                break;
            }

            // If not met, wait for the interval and try again
            Debug.Log($"Attempt {attempts + 1}: Condition not met yet.");
            attempts++;
            yield return new WaitForSecondsRealtime(0.05f);
        }

        if (attempts >= maxTries)
        {
            Debug.Log("Condition not met after max attempts.");
        }
        // Continue with the next part of your logic
        Debug.Log("Total distance: " + distance);
    }

    public string GetCurrentGameState()
    {
        switch (gameState)
        {
            case GameState.Introduction:
                return "Introduction";
            case GameState.Practice:
                return "Practice";
            case GameState.DoorPractice:
                return "DoorPractice";
            case GameState.Training:
                return "Training";
            case GameState.DevalueIntroduction:
                return "DevalueIntroduction";
            case GameState.Test:
                return "Test";
            case GameState.Debrief:
                return "Debrief";
            default:
                return "UNKOWN STATE";

        }
    }

    public void StartEngagingGame()
    {
        player.enabled = true;
        countdownTimer.gameObject.SetActive(true);
        dataController.Begin();
    }

    public void GenerateSeed()
    {
        if (gameState == GameState.Practice)
        {
            currentSeed = practiceLevels[round - 1];
            UnityEngine.Random.InitState(currentSeed);
        }

        if (gameState == GameState.DoorPractice)
        {
            currentSeed = doorPracticeLevels[round - 1];
            UnityEngine.Random.InitState(currentSeed);
        }

        if (gameState == GameState.Training)
        {
            currentSeed = trainingLevels[round - 1];
            currentCoinIdentity = coinOrderTraining[round - 1];
            UnityEngine.Random.InitState(currentSeed);
        }

        if (gameState == GameState.Test)
        {
            currentSeed = testLevels[round - 1];
            currentCoinIdentity = coinOrderTest[round - 1];
            UnityEngine.Random.InitState(currentSeed);
        }
    }

    public void PauseTimer()
    {
        countdownTimer.StopTimer();
    }

    public void ResumeTimer()
    {
        countdownTimer.StartTimer();
    }

    public string GetCurrentTotalRounds()
    {
        switch (gameState)
        {
            case GameState.Practice:
                return practiceRounds.ToString();
            case GameState.DoorPractice:
                return doorPracticeRounds.ToString();
            case GameState.Training:
                return trainingRounds.ToString();
            case GameState.Test:
                return testRounds.ToString();
            default:
                return "";

        }
    }
}
