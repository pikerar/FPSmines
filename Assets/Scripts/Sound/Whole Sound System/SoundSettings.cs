using System;
using System.IO;
using UnityEngine;

/// <summary>
/// Хранит настройки громкости и сериализует их в JSON.
/// Путь: Application.persistentDataPath/sound_settings.json
/// </summary>
[Serializable]
public class SoundSettings
{
    // Значения от 0.0 до 1.0
    public float musicVolume     = 1f;
    public float environmentVolume = 1f;
    public float voiceVolume     = 1f;

    // ──────────────────────────────────────────────
    // Persistence
    // ──────────────────────────────────────────────

    private static string FilePath =>
        Path.Combine(Application.persistentDataPath, "sound_settings.json");

    public static SoundSettings Load()
    {
        if (!File.Exists(FilePath))
            return new SoundSettings(); // дефолтные значения

        try
        {
            string json = File.ReadAllText(FilePath);
            return JsonUtility.FromJson<SoundSettings>(json) ?? new SoundSettings();
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[SoundSettings] Не удалось загрузить настройки: {e.Message}");
            return new SoundSettings();
        }
    }

    public void Save()
    {
        try
        {
            string json = JsonUtility.ToJson(this, prettyPrint: true);
            File.WriteAllText(FilePath, json);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[SoundSettings] Не удалось сохранить настройки: {e.Message}");
        }
    }
}
