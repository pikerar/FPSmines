using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class BuildVersionDisplay : MonoBehaviour
{
    [Header("Настройки версии")]
    [SerializeField] private string format = "v{0}"; // {0} = версия из Player Settings

    [Tooltip("Добавить суффикс для девбилда")]
    [SerializeField] private bool isDevBuild = false;

    [Tooltip("Суффикс для девбилда (по умолчанию 'db')")]
    [SerializeField] private string devSuffix = "db";

    private TMP_Text textComponent;

    void Awake()
    {
        textComponent = GetComponent<TMP_Text>();
        UpdateVersion();
    }

    void UpdateVersion()
    {
        string version = Application.version;

        // Добавляем суффикс если включён девбилд
        if (isDevBuild)
        {
            version += $"-{devSuffix}";
        }

        if (textComponent != null)
        {
            textComponent.text = string.Format(format, version);
        }
    }

    // Кнопка в инспекторе для обновления
    [ContextMenu("Обновить версию")]
    private void RefreshEditor()
    {
        UpdateVersion();
    }
}