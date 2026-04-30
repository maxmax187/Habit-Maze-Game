using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventTriggerOnEnter : MonoBehaviour
{
    [Header("Door Cutscene Event")]
    public UnityEvent unityEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (unityEvent == null)
        {
            Debug.Log("EventTriggerOnEnter was triggered but unityEvent was null");
        }
        else
        {
            Debug.Log("EventTriggerOnEnter Activated. Triggering" + unityEvent);
            unityEvent.Invoke();
        }
    }
}