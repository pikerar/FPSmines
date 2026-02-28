using UnityEngine;
using System.Collections;
using UnityEditorInternal;


public class MoveObjScript : SampleScript
{
     public Vector3 targetPosition;
    [SerializeField] public float speed;

    [ContextMenu("Сдвинуть")]
    public override void Use()
    {
        StopAllCoroutines();
        StartCoroutine(MoveToTarget());
    }

    private IEnumerator MoveToTarget()
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPosition;
    }
}
