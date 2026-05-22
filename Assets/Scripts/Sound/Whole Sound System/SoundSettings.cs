using System;
using System.IO;
using UnityEngine;

[Serializable]
public class SoundSettings
{
    public float musicVolume     = 1f;
    public float environmentVolume = 1f;
    public float voiceVolume     = 1f;


    private static string FilePath =>
        Path.Combine(Application.persistentDataPath, "sound_settings.json");

    public static SoundSettings Load()
    {
        if (!File.Exists(FilePath))
        {
            Debug.Log("[SoundSettings] File not found, using defaults");
            return new SoundSettings(); 
        }

        try
        {
            string json = File.ReadAllText(FilePath);
            Debug.Log($"[SoundSettings] Loaded JSON: {json}");
            return JsonUtility.FromJson<SoundSettings>(json) ?? new SoundSettings();
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[SoundSettings] Load failed: {e.Message}");
            return new SoundSettings();
        }
    }

    public void Save()
    {
        try
        {
            string json = JsonUtility.ToJson(this, prettyPrint: true);
            Debug.Log($"[SoundSettings] Saving: {json}");
            File.WriteAllText(FilePath, json);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[SoundSettings] Save failed: {e.Message}");
        }
    }
}
