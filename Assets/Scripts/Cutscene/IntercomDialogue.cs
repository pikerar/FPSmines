using System.Collections;
using UnityEngine;

public class IntercomDialogue : MonoBehaviour
{
    public CameraFocus cameraFocus;
    public SubtitleDisplay3D subtitles;
    public Transform intercomTarget;

    [TextArea] public string[] lines;
    public float[] lineDurations;

    public void StartDialogue()
    {
        StartCoroutine(Run());
    }

    IEnumerator Run()
    {
        // Ждём пока камера доедет до интеркома
        bool ready = false;
        cameraFocus.OnFocusStarted = () => ready = true;
        cameraFocus.StartFocus(intercomTarget);

        yield return new WaitUntil(() => ready);

        // Субтитры
        subtitles.ShowSequence(lines, lineDurations);

        // Ждём пока всё прочитается
        float total = 0f;
        foreach (var d in lineDurations) total += d;
        yield return new WaitForSeconds(total);

        cameraFocus.StopFocus();
    }
}