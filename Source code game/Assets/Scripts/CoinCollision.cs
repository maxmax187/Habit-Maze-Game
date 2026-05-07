using UnityEngine;

public class CoinCollision : MonoBehaviour
{
    [SerializeField] private bool isGold = true;

    public AudioClip collisionClip;

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameManager.Instance.PickUpCoin(isGold);
        Destroy(gameObject);
    }
}
