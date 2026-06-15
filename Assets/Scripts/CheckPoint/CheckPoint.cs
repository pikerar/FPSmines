using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] private float spawnHeightOffset = 1.5f; // настройка в инспекторе

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponentInParent<NewPlayerMovement>();
        if (player == null) return;

        RaycastHit hit;
        Vector3 spawnPos = player.transform.position;

        if (Physics.Raycast(player.transform.position, Vector3.down, out hit))
        {
            spawnPos = hit.point;
            spawnPos.y += spawnHeightOffset; // ← ВОТ ЗДЕСЬ: поднимаем на offset
            Debug.Log($"Checkpoint hit point: {hit.point}, spawn: {spawnPos}, collider: {hit.collider.name}");
        }
        else
        {
            spawnPos.y += spawnHeightOffset; // и здесь тоже, если рейкаст промахнулся
            Debug.Log("Raycast ничего не попал");
        }

        LevelManager.Instance.SaveCheckpoint(spawnPos);
        SceneStateManager.Instance.CaptureScene();
    }
}