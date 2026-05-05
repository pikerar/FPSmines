using System.Collections;
using UnityEngine;

public class FootstepsSFX : MonoBehaviour
{
    public AudioClip FootStepSFX;

    private NewPlayerMovement movement;

    void Start()
    {
        movement = GetComponent<NewPlayerMovement>();
        StartCoroutine(PlayFootSteps());
    }

    IEnumerator PlayFootSteps()
    {
        while (true)
        {
            if (movement.move.magnitude > 0.1f && movement.isGrounded)
            {
                AudioManager.instance.PlaySFX(FootStepSFX);
            }

            yield return new WaitForSeconds(0.35f);
        }
    }
}
