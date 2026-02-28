using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("Клавиша для загрузки")]
    [SerializeField] private KeyCode additiveLoadingKey;

    [Header("Имя сцены для загрузки")]
    [SerializeField] private string sceneName;

    private bool loadingKey;

    private void Update()
    {
        if (Input.GetKeyDown(additiveLoadingKey) && !loadingKey)
        {
            loadingKey = true;
            LoadSceneAdditive();
        }
    }

    private void LoadSceneAdditive()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }
    }
}
