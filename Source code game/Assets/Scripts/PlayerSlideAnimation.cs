using System.Collections;
using UnityEngine;

public class PlayerSlideAnimation : MonoBehaviour
{
    [Header("Animation")]
    public float moveSpeed = 5f;
    public float arrivalThreshold = 0.05f;


    private Coroutine activeSlide;

    public IEnumerator SlideToTarget(Vector3 target)
    {
        target.z = transform.position.z;
        Animator animator = GetComponent<Animator>();
        if (animator != null) animator.SetFloat("Speed", moveSpeed);

        activeSlide = StartCoroutine(SlideRoutine(target, animator));
        yield return activeSlide;
    }

    private IEnumerator SlideRoutine(Vector3 target, Animator animator)
    {
        while (Vector2.Distance(transform.position, target) > arrivalThreshold)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                target,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }
        transform.position = target;
        if (animator != null) animator.SetFloat("Speed", 0f);
    }

    public void StopSliding()
    {
        if (activeSlide != null) StopCoroutine(activeSlide);
        Animator animator = GetComponent<Animator>();
        if (animator != null) animator.SetFloat("Speed", 0f);
        activeSlide = null;
    }
}