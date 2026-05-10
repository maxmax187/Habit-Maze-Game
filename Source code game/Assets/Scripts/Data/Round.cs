[System.Serializable]
public class Round
{
    public int id;
    public string participantEmail;
    public int logId; //idk what this is
    public int seed;
    public int round;
    // public bool didCoinSpawn;
    public bool pickedUpCoin; // did player pick up the coin
    public bool finished; // did player finish the maze
    public string phase; // practice vs door practice vs training vs testing
    public string date; // real-time date
    // public decimal totalDistance;
    // public decimal distanceCoinSpawn;
    public decimal remainingTime;
    public int totalRoundsFinished;
    public int day; // day of the study (1,2 or 3)
    // public decimal coinPickupTime;
    // public decimal coinSpawnTime;
    public decimal thoughBubbleTime; // thought bubble pop-up time
    public decimal bufferDelay; // buffer delay between thought bubble pop-up and coin presentation
    public decimal coinPresentTime; // time when coin identity is revealed to player in thought bubble
    public decimal playerChoiceTime; // time when player reacts to coin presentation

    public bool wentBackForCoin; // did player turn back for the coin at the door choice point
    public int coinIdentity; // was coin gold or silver (1=gold, 0=silver)

}


// Old round setup below:
// [System.Serializable]
// public class Round
// {
//     public int id;
//     public string participantEmail;
//     public int logId;
//     public int seed;
//     public int round;
//     public bool didCoinSpawn;
//     public bool pickedUpCoin;
//     public bool finished;
//     public string phase;
//     public string date;
//     public decimal totalDistance;
//     public decimal distanceCoinSpawn;
//     public decimal remainingTime;
//     public int totalRoundsFinished;
//     public int day;
//     public decimal coinPickupTime;
//     public decimal coinSpawnTime;
// }