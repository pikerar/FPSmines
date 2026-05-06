using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform cameraPosition;
    public Transform headBone;
    public Animator anim;

    [Header("Smoothing")]
    public float rotationSmoothing = 10f;
    public float blendSpeed = 5f; 

    private float blendWeight = 0f; 

    private void LateUpdate()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        bool isJumping = stateInfo.IsName("Jump"); 

        float targetWeight = isJumping ? 1f : 0f;
        blendWeight = Mathf.MoveTowards(blendWeight, targetWeight, Time.deltaTime * blendSpeed);

        Vector3 normalPos = cameraPosition.position;
        Vector3 jumpPos = new Vector3(
            headBone.position.x,
            headBone.position.y,
            headBone.position.z
        );

        transform.position = Vector3.Lerp(normalPos, jumpPos, blendWeight);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            cameraPosition.rotation,
            Time.deltaTime * rotationSmoothing
        );
    }
}