using System.Collections;
using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    public enum DoorOrientation { Top, Bottom, Left, Right }

    [Header("Orientation")]
    public DoorOrientation orientation;

    [Header("Halves")]
    public Transform halfA; // Left half (horizontal) or Top half (vertical)
    public Transform halfB; // Right half (horizontal) or Bottom half (vertical)

    [Header("Animation")]
    public float slideDistance = 0.5f; // How far each half slides (match half-width/height)
    public float duration = 0.4f;
    public AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Vector3 halfA_closed;
    private Vector3 halfB_closed;
    private Vector3 halfA_open;
    private Vector3 halfB_open;

    private bool isOpen = false;
    private Coroutine activeCoroutine;

    void Awake()
    {
        // Record closed (resting) positions
        halfA_closed = halfA.localPosition;
        halfB_closed = halfB.localPosition;

        // Compute open positions based on orientation
        // Top/Bottom doors slide halves left and right
        // Left/Right doors slide halves up and down
        bool isHorizontalDoor = (orientation == DoorOrientation.Top || 
                                  orientation == DoorOrientation.Bottom);

        Vector3 axis = isHorizontalDoor ? Vector3.right : Vector3.up;

        halfA_open = halfA_closed - axis * slideDistance;
        halfB_open = halfB_closed + axis * slideDistance;
    }

    public void Open()
    {
        if (isOpen) return;
        isOpen = true;
        if (activeCoroutine != null) StopCoroutine(activeCoroutine);
        activeCoroutine = StartCoroutine(Slide(halfA_closed, halfA_open,
                                               halfB_closed, halfB_open));
    }

    public void Close()
    {
        if (!isOpen) return;
        isOpen = false;
        if (activeCoroutine != null) StopCoroutine(activeCoroutine);
        activeCoroutine = StartCoroutine(Slide(halfA_open, halfA_closed,
                                               halfB_open, halfB_closed));
    }

    public void Toggle() { if (isOpen) Close(); else Open(); }

    private IEnumerator Slide(Vector3 fromA, Vector3 toA, Vector3 fromB, Vector3 toB)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = easeCurve.Evaluate(Mathf.Clamp01(elapsed / duration));

            halfA.localPosition = Vector3.LerpUnclamped(fromA, toA, t);
            halfB.localPosition = Vector3.LerpUnclamped(fromB, toB, t);

            yield return null;
        }

        halfA.localPosition = toA;
        halfB.localPosition = toB;
    }
}