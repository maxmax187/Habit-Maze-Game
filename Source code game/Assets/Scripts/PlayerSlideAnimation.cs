using System.Collections;
using UnityEngine;

public class PlayerSlideAnimation : MonoBehaviour
{
    [Header("Animation")]
    public float moveSpeed = 5f;
    public float arrivalThreshold = 0.05f;

    public IEnumerator SlideToTarget(Vector3 target)
    {
        target.z = transform.position.z;
        
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
    }
}