using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class ScritpManager : MonoBehaviour
{
    public List<SampleScript> scripts = new List<SampleScript>();

    [ContextMenu("Старт")]
    public void ActivateAll()
    {
        foreach (var script in scripts)
        {
            if (script != null)
            {
                script.Use();
            }
        }
    }
}
