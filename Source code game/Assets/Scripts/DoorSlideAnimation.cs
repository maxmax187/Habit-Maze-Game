using System.Collections;
using UnityEngine;

public class DoorSlideAnimation : MonoBehaviour
{
    [Header("Halves")]
    public Transform LeftHalf;
    public Transform RightHalf;

    [Header("Animation")]
    public float duration = 0.4f;
    public AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float slideDistanceMultiplier = 0.6f;

    public float Duration => duration;

    private Vector3 leftHalf_closed;
    private Vector3 rightHalf_closed;
    private Vector3 leftHalf_open;
    private Vector3 rightHalf_open;

    private bool isOpen = false;
    private Coroutine activeCoroutine;

    void Awake()
    {
        leftHalf_closed = LeftHalf.localPosition;
        rightHalf_closed = RightHalf.localPosition;

        // Slide direction is simply from RightHalf toward LeftHalf (and vice versa),
        // normalized — works for any orientation automatically
        Vector3 axis = (RightHalf.localPosition - LeftHalf.localPosition).normalized;

        // Slide distance is the local X scale of either half (they're identical)
        float slideDistance = LeftHalf.localScale.x * slideDistanceMultiplier;

        leftHalf_open  = leftHalf_closed  - axis * slideDistance;
        rightHalf_open = rightHalf_closed + axis * slideDistance;
    }

    public void Open()
    {
        if (isOpen) return;
        isOpen = true;
        if (activeCoroutine != null) StopCoroutine(activeCoroutine);
        activeCoroutine = StartCoroutine(Slide(leftHalf_closed, leftHalf_open,
                                               rightHalf_closed, rightHalf_open));
    }

    public void Close()
    {
        if (!isOpen) return;
        isOpen = false;
        if (activeCoroutine != null) StopCoroutine(activeCoroutine);
        activeCoroutine = StartCoroutine(Slide(leftHalf_open, leftHalf_closed,
                                               rightHalf_open, rightHalf_closed));
    }

    public void Toggle() { if (isOpen) Close(); else Open(); }

    private IEnumerator Slide(Vector3 fromA, Vector3 toA, Vector3 fromB, Vector3 toB)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = easeCurve.Evaluate(Mathf.Clamp01(elapsed / duration));
            LeftHalf.localPosition  = Vector3.LerpUnclamped(fromA, toA, t);
            RightHalf.localPosition = Vector3.LerpUnclamped(fromB, toB, t);
            yield return null;
        }

        LeftHalf.localPosition  = toA;
        RightHalf.localPosition = toB;
    }
}