using UnityEngine;

public class ThoughtBubble : MonoBehaviour
{
    public Sprite goldCoin;
    public Sprite silverCoin;

    private SpriteRenderer bubbleRenderer;
    
    void Awake()
    {
        bubbleRenderer = GetComponent<SpriteRenderer>();
        bubbleRenderer.enabled = false;
    }

    public void Show()
    {
        bubbleRenderer.enabled = true;
    }

    public void Hide()
    {
        bubbleRenderer.enabled = false;
    }
}
