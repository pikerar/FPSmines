using UnityEngine;
using System.Collections;

public class PlayerSpawner : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(SpawnAfterPhysics());
    }

    IEnumerator SpawnAfterPhysics()
    {
        yield return new WaitForFixedUpdate(); // ждём один физический тик

        CheckpointData data = LevelManager.Instance.LoadCheckpoint();

        if (data != null)
        {
            transform.position = data.Position;
            FlagInventory.Instance.RestoreFlags(data.currentFlags);
        }

        SceneStateManager.Instance.RestoreScene();
    }
}