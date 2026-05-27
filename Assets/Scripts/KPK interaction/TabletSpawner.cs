using UnityEngine;

public class TabletSpawner : MonoBehaviour
{
    [Header("Объект TabletPlace (пустой, неактивный на сцене)")]
    [SerializeField] private GameObject tabletPlace;

    [Header("Моделька КПК (дочерний объект, изначально где-то на сцене)")]
    [SerializeField] private Transform tabletModel;

    [Header("Точка внутри TabletPlace, куда переместить модель")]
    [SerializeField] private Transform tabletModelHolder;

    [Header("Звук пиликанья (AudioSource на TabletPlace)")]
    [SerializeField] private AudioSource beepAudioSource;

    [Header("Звук подбора")]
    [SerializeField] private AudioSource pickupAudioSource;

    [Header("Точка удержания в руке персонажа")]
    [SerializeField] private Transform playerHoldPoint;

    [Header("Подсказка при наведении")]
    [SerializeField] private string interactHint = "E - подобрать";

    private bool isSpawned = false;
    private bool isPickedUp = false;

    void Awake()
    {
        // РЕЗЕРВНЫЙ ПОИСК: если в инспекторе ссылка сломалась (неактивный объект)
        if (tabletPlace == null)
        {
            // Ищем в сцене по имени (включая неактивные — через Transform.Find)
            var rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var root in rootObjects)
            {
                var found = FindInChildren(root.transform, "TabletPlace");
                if (found != null)
                {
                    tabletPlace = found;
                    Debug.Log($"[TabletSpawner] Найден TabletPlace: {tabletPlace.name}");
                    break;
                }
            }
        }

        // Если нашли TabletPlace — ищем внутри него остальное
        if (tabletPlace != null)
        {
            if (tabletModelHolder == null)
            {
                var holder = tabletPlace.transform.Find("TabletModelHolder");
                if (holder != null) tabletModelHolder = holder;
            }

            if (beepAudioSource == null)
            {
                beepAudioSource = tabletPlace.GetComponent < AudioSource > ();
            }

            if (pickupAudioSource == null)
            {
                var sources = tabletPlace.GetComponents < AudioSource > ();
                if (sources.Length > 1) pickupAudioSource = sources[1];
                else if (sources.Length > 0) pickupAudioSource = sources[0];
            }
        }

        // Поиск модели КПК
        if (tabletModel == null)
        {
            var kpkObj = GameObject.Find("[PLAYER] Kpk_obj");
            if (kpkObj != null) tabletModel = kpkObj.transform;
        }

        // Поиск точки удержания
        if (playerHoldPoint == null)
        {
            var holdPoint = GameObject.Find("Tablet_HoldPoint");
            if (holdPoint != null) playerHoldPoint = holdPoint.transform;
        }
    }

    GameObject FindInChildren(Transform parent, string name)
    {
        if (parent.name == name) return parent.gameObject;
        foreach (Transform child in parent)
        {
            var result = FindInChildren(child, name);
            if (result != null) return result;
        }
        return null;
    }

    void Start()
    {

    }

    public void SpawnTablet()
    {
        tabletModel.SetParent(tabletModelHolder, false);
        tabletModel.localPosition = Vector3.zero;
        tabletModel.localRotation = Quaternion.identity;

        tabletPlace.SetActive(true);

        if (beepAudioSource != null)
        {
            beepAudioSource.Play();
        }
        else
        {
            Debug.LogWarning("[TabletSpawner] Beep AudioSource не назначен.");
        }

        isSpawned = true;
    }

    public void PickUpTablet()
    {
        if (isPickedUp) return;


        if (beepAudioSource != null) beepAudioSource.Stop();
        if (pickupAudioSource != null) pickupAudioSource.Play();

        if (tabletModel != null && playerHoldPoint != null)
        {
            tabletModel.SetParent(playerHoldPoint, false);
            tabletModel.localPosition = Vector3.zero;
            tabletModel.localRotation = Quaternion.identity;
        }

        if (tabletPlace != null)
        {
            Destroy(tabletPlace);
        }

        KPKUnlockManager.Instance?.UnlockKPK();

        isPickedUp = true;
    }
}