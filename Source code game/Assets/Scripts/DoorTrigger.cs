using UnityEngine;

[RequireComponent(typeof(DoorSlideAnimation))]
public class DoorTrigger : MonoBehaviour
{
    private DoorEventManager doorEventManager;
    private DoorSlideAnimation doorAnimation;

    void Awake()
    {
        doorAnimation = GetComponent<DoorSlideAnimation>();
        doorEventManager = GetComponentInParent<DoorEventManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        doorEventManager.OnDoorTriggered(doorAnimation, other.gameObject);
    }
}