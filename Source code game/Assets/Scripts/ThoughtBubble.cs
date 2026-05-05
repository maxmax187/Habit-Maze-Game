using System.Collections;
using UnityEngine;

public class ThoughtBubble : MonoBehaviour
{
    [Header("References")]
    public SpriteRenderer coinRenderer;   
    public SpriteRenderer bubbleRenderer;

    [Header("Sprites")]
    public Sprite goldCoin;
    public Sprite silverCoin;

    private Coroutine showRoutine;

    void Awake()
    {
        SetVisible(false);
    }

    // Call this from your event, passing true for gold, false for silver
    public void Show(bool isGold, float bufferDelay = 1f)
    {
        if (showRoutine != null)
            StopCoroutine(showRoutine);

        showRoutine = StartCoroutine(ShowRoutine(isGold, bufferDelay));
    }

    public void Hide()
    {
        if (showRoutine != null)
        {
            StopCoroutine(showRoutine);
            showRoutine = null;
        }
        SetVisible(false);
    }

    private IEnumerator ShowRoutine(bool isGold, float bufferDelay)
    {
        // Show bubble immediately, coin hidden
        coinRenderer.enabled = false;
        bubbleRenderer.enabled = true;

        // Wait for buffer delay, then reveal coin
        yield return new WaitForSeconds(bufferDelay);
        coinRenderer.sprite = isGold ? goldCoin : silverCoin; //gold coin = 1, silver = 0
        coinRenderer.enabled = true;

        showRoutine = null;
    }

    private void SetVisible(bool visible)
    {
        bubbleRenderer.enabled = visible;
        coinRenderer.enabled = visible;
    }
}