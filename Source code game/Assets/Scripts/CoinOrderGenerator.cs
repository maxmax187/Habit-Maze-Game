using UnityEngine;
using Debug = UnityEngine.Debug;

public class CoinOrderGenerator
{
    /// <summary>
    /// Generate an integer array to determine which coin to spawn in a given round. Returs integer array of 0's and 1's in random order, with the amount of 1's determined by amt_gold.
    /// </summary>
    /// <param name="rounds"></param>
    /// <param name="amt_gold"></param>
    public static int[] GenerateRandomCoinOrder(int rounds, int amt_gold, int seed)
    {
        if(rounds <= 0) { 
            Debug.Log("Error in generating coin ordering - invalid rounds argument: " + rounds);
            return null; 
        }

        if(amt_gold < 0) { 
            Debug.Log("Error in generating coin ordering - invalid amt_gold argument: " + amt_gold);
            return null; 
        }

        if(amt_gold > rounds) { 
            Debug.Log("Error in generating coin ordering - argument amt_gold cannot be less than rounds");
            return null; 
        }

        // create array and fill with amt_gold 1's and the rest 0's
        int[] coinOrder = new int[rounds];
        for (int i = 0; i < rounds; i++)
        {
            coinOrder[i] = i < amt_gold ? 1 : 0;
        }

        // shuffle coinOrder array randomly, based on seed
        UnityEngine.Random.InitState(seed);

        for (int i = rounds - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (coinOrder[i], coinOrder[j]) = (coinOrder[j], coinOrder[i]);
        }

        return coinOrder;
    }

    // public CoinOrderGenerator()
    // {
    //     Debug.Log(GenerateRandomCoinOrder(5, 1, 10));
    // }
}