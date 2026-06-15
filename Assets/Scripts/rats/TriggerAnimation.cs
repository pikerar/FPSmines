using UnityEngine;

[RequireComponent(typeof(Animator))]
public class NPCProximityAnimator : MonoBehaviour
{
    [SerializeField] private float triggerDistance = 5f;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string boolParameter = "IsPlayerNear";

    [SerializeField] private GameObject gun;
    [SerializeField] private bool lookAtPlayer = false;
    [SerializeField] private float rotationSpeed = 5f;

    private Animator animator;
    private Transform player;
    private bool wasNear = false; // запоминаем предыдущее состояние

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindWithTag(playerTag)?.transform;
        if (gun != null) gun.SetActive(false);
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        bool isNear = distance <= triggerDistance;

        // Смена состояния — включаем/выключаем пушку
        if (isNear != wasNear)
        {
            if (gun != null) gun.SetActive(isNear);
            wasNear = isNear;
        }

        animator.SetBool(boolParameter, isNear);

        if (isNear && lookAtPlayer)
        {
            Vector3 direction = player.position - transform.position;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, triggerDistance);
    }
}