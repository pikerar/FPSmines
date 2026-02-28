using UnityEngine;

public class CopingScript : SampleScript
{
    [SerializeField] public GameObject prefab;
    [SerializeField] public int CopiesCount;
    [SerializeField] public float Steps;
    public override void Use()
    {
        if (prefab == null) return;
        for (int i = 1; i <= CopiesCount; i++)
        {
            Vector3 pos = transform.position + transform.forward * Steps * i;
            Instantiate(prefab, pos, transform.rotation);
        }
    }
}
