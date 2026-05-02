using UnityEngine;

[RequireComponent(typeof(DoorSlideAnimation))]
public class DoorTrigger : MonoBehaviour
{
    private DoorEventManager doorEventManager;
    private DoorSlideAnimation doorAnimation;
    private Collider2D triggerCollider;

    void Awake()
    {
        doorAnimation = GetComponent<DoorSlideAnimation>();
        doorEventManager = GetComponentInParent<DoorEventManager>();
        triggerCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        doorEventManager.OnDoorTriggered(doorAnimation, other.gameObject);
    }

    /// <summary>
    /// refreshes trigger collider to allow re-triggering if player was still inside collider
    /// </summary>
    public void Refresh()
    {
        triggerCollider.enabled = false;
        triggerCollider.enabled = true;
    }
}