using UnityEngine;

public class MoveInDirection : MonoBehaviour
{
    [Header("Movement Settings")]
    public Vector3 direction = Vector3.forward;
    public float speed = 5f;
    public float distance = 10f; // 0 = бесконечно

    private bool _moving = false;
    private float _traveled = 0f;
    private Vector3 _startPos;

    public void StartMoving()
    {
        _moving = true;
        _traveled = 0f;
        _startPos = transform.position;
    }

    public void StopMoving()
    {
        _moving = false;
    }

    void Update()
    {
        if (!_moving) return;

        Vector3 move = direction.normalized * speed * Time.deltaTime;
        transform.Translate(move, Space.World);

        if (distance > 0)
        {
            _traveled += move.magnitude;
            if (_traveled >= distance)
            {
                _moving = false;
            }
        }
    }
}