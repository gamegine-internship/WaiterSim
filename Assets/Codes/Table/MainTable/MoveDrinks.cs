using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoveDrinks : MonoBehaviour
{
    [Header("Setup")]
    public string glassTag = "GlassTag0";
    public KeyCode loadKey = KeyCode.F;

    public Transform[] midPoints;     // Intermediate positions (lift targets)
    public Transform[] trayPoints;    // Final tray positions

    public PlayerMovement playerMovement;
    public TrayTilt trayTilt;
    public MouseLook mouseLook;

    public float liftDuration = 2f;
    public float moveDuration = 2f;

    [Header("Tray Movement Simulation")]
    public Transform trayTransform;
    public bool enableSliding = true;
    public bool enableRotation = true;

    public bool takenDrinks = false;
    
    [SerializeField] private float slideFollowSpeed = 5f;      // Base responsiveness
    [SerializeField] private float slideSensitivity = 0.05f;   // How far to slide per degree
    [SerializeField] private float minSlideSpeed = 2f;         // Always update, even if flat
    [SerializeField] private float maxSlideSpeed = 10f;        // Prevent extreme speed

    private List<GameObject> glassesToLoad = new List<GameObject>();
    private Dictionary<GameObject, Transform> glassToTrayPointMap = new Dictionary<GameObject, Transform>();

    private bool isPlayerInTrigger = false;
    private bool isLoading = false;

    void Update()
    {
        RefreshGlassList();

        if (isPlayerInTrigger && !isLoading && Input.GetKeyDown(loadKey))
        {
            StartCoroutine(LoadDrinksRoutine());
        }

        if (enableSliding) SimulateGlassSlide();
        if (enableRotation) RotateGlassesWithTray();
    }

    private void GlassesAreKinematic()
    {   //glasses will always be on the tray
        int spCount = 0;
        foreach (GameObject glass in glassesToLoad)
        {
            glass.transform.DOKill();
            glass.transform.DOMove(trayPoints[spCount].position, 0.001f);

            Rigidbody rb = glass.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;

            glassToTrayPointMap[glass] = trayPoints[spCount];
            spCount++;
        }
    }

    private void RefreshGlassList()
    {
        glassesToLoad.Clear();
        GameObject[] foundGlasses = GameObject.FindGameObjectsWithTag(glassTag);

        foreach (GameObject glass in foundGlasses)
        {
            if (glass != null && glass.activeInHierarchy)
                glassesToLoad.Add(glass);
        }
    }

    private IEnumerator LoadDrinksRoutine()
    {
        isLoading = true;

        playerMovement?.SetMovementEnabled(false);
        mouseLook?.UnlockCursor();

        yield return new WaitForSeconds(0.2f);

        int count = Mathf.Min(glassesToLoad.Count, Mathf.Min(midPoints.Length, trayPoints.Length));

        for (int i = 0; i < count; i++)
        {
            GameObject glass = glassesToLoad[i];
            if (glass == null) continue;

            Transform hatch = glass.transform.Find("Hatch");

            if (hatch != null)
                hatch.gameObject.SetActive(true);

            yield return glass.transform.DOMove(midPoints[i].position, liftDuration)
                .SetEase(Ease.InOutQuad)
                .WaitForCompletion();

            yield return glass.transform.DOMove(trayPoints[i].position, moveDuration)
                .SetEase(Ease.InOutQuad)
                .WaitForCompletion();

            glass.tag = "GlassTag1";

            if (hatch != null)
                hatch.gameObject.SetActive(false);
        }

        GlassesAreKinematic();

        playerMovement?.SetMovementEnabled(true);
        mouseLook?.LockCursor();

        isLoading = false;
        takenDrinks = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TrayTag"))
            isPlayerInTrigger = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("TrayTag"))
            isPlayerInTrigger = false;
    }

    private void SimulateGlassSlide()
    {
        foreach (var pair in glassToTrayPointMap)
        {
            GameObject glass = pair.Key;
            Transform trayPoint = pair.Value;

            float zTilt = trayTransform.localEulerAngles.z;
            if (zTilt > 180f) zTilt -= 360f;

            float slideOffset = -zTilt * slideSensitivity;
            Vector3 rightDir = trayTransform.right;
            Vector3 targetPos = trayPoint.position + rightDir * slideOffset;

            float dynamicSpeed = Mathf.Clamp(Mathf.Abs(zTilt) * slideFollowSpeed, minSlideSpeed, maxSlideSpeed);

            glass.transform.position = Vector3.Lerp(
                glass.transform.position,
                targetPos,
                Time.deltaTime * dynamicSpeed
            );
        }
    }

    private void RotateGlassesWithTray()
    {
        foreach (var pair in glassToTrayPointMap)
        {
            GameObject glass = pair.Key;
            glass.transform.rotation = trayTransform.rotation;
        }
    }
}
