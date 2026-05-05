using UnityEngine;

public class CoinCollision : MonoBehaviour
{
    [SerializeField] private bool isSilver = true;

    public AudioClip collisionClip;

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameManager.Instance.PickUpCoin();
        Destroy(gameObject);
    }
}
